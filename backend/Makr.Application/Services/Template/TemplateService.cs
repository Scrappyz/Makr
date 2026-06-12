using Makr.Application.Interfaces;
using Makr.Application.Pipeline.Interpolator;
using Makr.Application.Pipeline.PathSelector;
using Makr.Domain.Helpers;
using Makr.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void InitializeTemplate(string templateId, Dictionary<string, string> parameters)
        {
            string templatePath = Path.GetFullPath(Path.Combine(_templateSetting.TemplateDirectory, templateId));
            string initPath = Path.GetFullPath(_templateSetting.TemplateInitializationDirectory);

            List<string> paths = _pathSelector.SortByDepth(_pathSelector.GetPaths(templatePath, ["**/*"], []), '/');

            List<string> templatePaths = paths.Select(p => Path.GetFullPath(Path.Combine(templatePath, p))).ToList();
            List<string> initPaths = paths.Select(p => Path.GetFullPath(Path.Combine(initPath, p))).ToList();

            FilesystemUtils.EmptyDirectory(initPath);

            for (int i = 0; i < paths.Count; i++)
            {
                FilesystemUtils.Copy(templatePaths[i], initPaths[i], true);
            }

            _interpolator.InterpolateContents(initPaths, "!", "!", parameters);
            _interpolator.InterpolatePaths(initPaths, "!", "!", parameters);
        }

        public List<string> GetDuplicateParameters(List<TemplateParameterRequest> parameters)
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
    }
}
