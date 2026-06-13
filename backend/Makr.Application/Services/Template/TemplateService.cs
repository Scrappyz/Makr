using Makr.Application.Interfaces;
using Makr.Application.Pipeline.Interpolator;
using Makr.Application.Pipeline.PathSelector;
using Makr.Domain.Helpers;
using Makr.Domain.Models;
using Makr.Domain.Models.Template;
using System.Text.Json;

namespace Makr.Application.Services.Template
{
    public sealed class TemplateService : ITemplateService
    {
        private readonly ITemplateSetting _templateSetting;
        private readonly IInterpolator _interpolator;
        private readonly IPathSelector _pathSelector;

        public TemplateService(ITemplateSetting templateSetting, IInterpolator interpolator, IPathSelector pathSelector)
        {
            _templateSetting = templateSetting;
            _interpolator = interpolator;
            _pathSelector = pathSelector;
        }

        public void InitializeTemplate(string templateId, List<ParameterKeyValue> parameters)
        {
            string templatePath = Path.GetFullPath(Path.Combine(_templateSetting.TemplateDirectory, templateId));
            string initPath = Path.GetFullPath(_templateSetting.TemplateInitializationDirectory);

            string templateJsonPath = Path.Combine(templatePath, ".makr", "template.json");
            string templateJsonContent = File.ReadAllText(templateJsonPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            TemplateConfig? config = JsonSerializer.Deserialize<TemplateConfig>(templateJsonContent, options);

            parameters = SetParameterValues(parameters, config.Initialization.Parameters);

            List<string> paths = _pathSelector.SortByDepth(_pathSelector.GetPaths(templatePath, ["**/*"], []), '/');

            List<string> templatePaths = paths.Select(p => Path.GetFullPath(Path.Combine(templatePath, p))).ToList();
            List<string> initPaths = paths.Select(p => Path.GetFullPath(Path.Combine(initPath, p))).ToList();

            FilesystemUtils.EmptyDirectory(initPath);

            for (int i = 0; i < paths.Count; i++)
            {
                FilesystemUtils.Copy(templatePaths[i], initPaths[i], true);
            }

            Dictionary<string, string> parameterInterpolation = GetInterpolationParameters(parameters, config.Initialization.Parameters)
                                                                .ToDictionary(p => p.Key, p => _interpolator.ToString(p.Value));

            _interpolator.InterpolateContents(initPaths, "!", "!", parameterInterpolation);
            _interpolator.InterpolatePaths(initPaths, "!", "!", parameterInterpolation);
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
