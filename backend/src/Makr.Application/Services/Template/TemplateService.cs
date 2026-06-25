using Makr.Application.Interfaces;
using Makr.Application.Pipeline.Interpolation;
using Makr.Application.Pipeline.PathSelection;
using Makr.Application.Pipeline.RuleEvaluation;
using Makr.Domain.Exceptions;
using Makr.Domain.Helpers;
using Makr.Domain.Models;
using Makr.Domain.Models.Template;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Makr.Domain.Enums;

namespace Makr.Application.Services.Template
{
    public sealed class TemplateService : ITemplateService
    {
        private readonly ITemplateSetting _templateSetting;
        private readonly IInterpolator _interpolator;
        private readonly IPathSelector _pathSelector;
        private readonly IRuleEvaluator _ruleEvaluator;

        public TemplateService(ITemplateSetting templateSetting, IInterpolator interpolator, IPathSelector pathSelector, IRuleEvaluator ruleEvaluator)
        {
            _templateSetting = templateSetting;
            _interpolator = interpolator;
            _pathSelector = pathSelector;
            _ruleEvaluator = ruleEvaluator;
        }

        public void InitializeTemplate(string templateId, List<ParameterKeyValue> parameters, bool force)
        {
            #region Declare variables
            string templatePath = GetTemplatePath(templateId);
            string initPath = GetTemplateInitializationPath();

            string templateJsonPath = GetTemplateConfigPath(templateId);

            if (!FilesystemUtils.Exists(templateJsonPath))
            {
                throw new ApiException(StatusCodes.Status404NotFound, ErrorCode.Template.NotFound, $"Template with ID `{templateId}` cannot be found");
            }
            
            string templateJsonContent = File.ReadAllText(templateJsonPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            TemplateConfig? config = JsonSerializer.Deserialize<TemplateConfig>(templateJsonContent, options);

            parameters = SetParameterValues(parameters, config.Initialization.Parameters);

            // Modify config via rules
            _ruleEvaluator.ResolveRules(config.Initialization, config.Initialization.Rules, parameters);

            List<string> paths = _pathSelector.SortByDepth(
                _pathSelector.GetPaths(templatePath, config.Initialization.Selection.include, config.Initialization.Selection.exclude)
            );

            List<string> templatePaths = paths.Select(p => Path.GetFullPath(Path.Combine(templatePath, p))).ToList();
            List<string> initPaths = paths.Select(p => Path.GetFullPath(Path.Combine(initPath, p))).ToList();
            #endregion

            #region Copy template files to initialization directory
            if (force)
            {
                FilesystemUtils.EmptyDirectory(initPath);
            }
            else if (Directory.Exists(initPath) && Directory.EnumerateFileSystemEntries(initPath).Any())
            {
                throw new Exception($"Initialization directory '{initPath}' is not empty. Use force option to overwrite.");
            }

            for (int i = 0; i < paths.Count; i++)
            {
                FilesystemUtils.Copy(templatePaths[i], initPaths[i], true);
            }
            #endregion

            #region Interpolate files in initialization directory
            Dictionary<string, string> parameterInterpolation = GetInterpolationParameters(parameters, config.Initialization.Parameters)
                                                                .ToDictionary(p => p.Key, p => _interpolator.ToString(p.Value));

            TemplateInterpolationTarget interpolationTarget = config.Initialization.Interpolation.Targets;
            List<string> contentInterpolationPaths = _pathSelector.FilterPaths(paths, interpolationTarget.Contents.include, interpolationTarget.Contents.exclude)
                                                    .Select(p => Path.GetFullPath(Path.Combine(initPath, p))).ToList();
            List<string> filenameInterpolationPaths = _pathSelector.SortByDepthDescending(
                                                    _pathSelector.FilterPaths(paths, interpolationTarget.Filenames.include, interpolationTarget.Filenames.exclude)
                                                    .Select(p => Path.GetFullPath(Path.Combine(initPath, p))).ToList()
                                                  );

            string interpolationPrefix = config.Initialization.Interpolation.Prefix;
            string interpolationSuffix = config.Initialization.Interpolation.Suffix;

            _interpolator.InterpolateContents(contentInterpolationPaths, interpolationPrefix, interpolationSuffix, parameterInterpolation);
            _interpolator.InterpolatePaths(filenameInterpolationPaths, interpolationPrefix, interpolationSuffix, parameterInterpolation);
            #endregion
        }

        public void InitializeTemplate(string templateId, List<ParameterKeyValue> parameters)
        {
            InitializeTemplate(templateId, parameters, false);
        }

        public TemplateConfig? GetTemplateConfig(string templateId)
        {
            string configPath = GetTemplateConfigPath(templateId);

            if (!FilesystemUtils.Exists(configPath))
            {
                return null;
            }

            string configContent = File.ReadAllText(configPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            TemplateConfig? config = JsonSerializer.Deserialize<TemplateConfig>(configContent, options);

            return config;
        }

        public string GetTemplatePath(string templateId)
        {
            return Path.GetFullPath(Path.Combine(_templateSetting.TemplateDirectory, templateId));
        }

        public string GetTemplateInitializationPath()
        {
            return Path.GetFullPath(_templateSetting.TemplateInitializationDirectory);
        }

        public string GetTemplateConfigPath(string templateId)
        {
            return Path.GetFullPath(Path.Combine(_templateSetting.TemplateDirectory, templateId, ".makr", "template.json"));
        }

        public bool TemplateExists(string templateId)
        {
            string templatePath = GetTemplatePath(templateId);
            string templateConfigPath = GetTemplateConfigPath(templateId);
            return FilesystemUtils.Exists(templateConfigPath);
        }

        public List<string> GetDuplicateParameters(List<ParameterKeyValue> parameters)
        {
            var groupVars = parameters.GroupBy(v => v.Key).ToList();
            List<string> result = new List<string>();

            foreach (var v in groupVars)
            {
                if (v.Count() > 1)
                    result.Add(v.Key);
            }

            return result;
        }

        public List<ParameterKeyValue> TransformParameterRequest(List<TemplateParameterRequest> parameters)
        {
            return parameters.Select(p => new ParameterKeyValue
            {
                Key = p.Key,
                Value = p.Value
            }).ToList();
        }

        private List<ParameterKeyValue> GetInterpolationParameters(List<ParameterKeyValue> parameters, List<TemplateParameter> configParameters)
        {
            List<ParameterKeyValue> result = new List<ParameterKeyValue>();
            Dictionary<string, bool> configParameterMap = configParameters.ToDictionary(p => p.Key, p => p.Interpolate);

            foreach (var parameter in parameters)
            {
                if (configParameterMap.ContainsKey(parameter.Key) && configParameterMap[parameter.Key])
                {
                    result.Add(parameter);
                }
            }

            return result;
        }

        private List<ParameterKeyValue> SetParameterValues(List<ParameterKeyValue> parameters, List<TemplateParameter> configParameters)
        {
            // <key, value>
            Dictionary<string, object?> parameterMap = parameters.ToDictionary(p => p.Key, p => p.Value);

            List<ParameterKeyValue> newParameters = new List<ParameterKeyValue>();

            foreach (var configParameter in configParameters)
            {
                // Not provided, use default value
                if (!parameterMap.ContainsKey(configParameter.Key) || parameterMap[configParameter.Key] == null)
                {
                    newParameters.Add(new ParameterKeyValue
                    {
                        Key = configParameter.Key,
                        Value = configParameter.DefaultValue
                    });
                }
                else
                {
                    newParameters.Add(new ParameterKeyValue
                    {
                        Key = configParameter.Key,
                        Value = parameterMap[configParameter.Key]
                    });
                }
            }

            return newParameters;
        }

        public void CreateTemplateJson(TemplateConfig config, string toDirectory, string configFilename)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string configStr = JsonSerializer.Serialize(config, options);
            string destination = Path.GetFullPath(Path.Combine(toDirectory, configFilename));

            File.WriteAllText(destination, configStr);
        }

        public void CreateTemplateJson(TemplateConfig config, string toDirectory)
        {
            CreateTemplateJson(config, toDirectory, "template.json");
        }
    }
}
