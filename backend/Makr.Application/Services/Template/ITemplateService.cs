using Makr.Domain.Models;
using Makr.Domain.Models.Template;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Application.Services.Template
{
    public interface ITemplateService
    {
        void InitializeTemplate(string templateId, List<ParameterKeyValue> parameters, bool force);

        void InitializeTemplate(string templateId, List<ParameterKeyValue> parameters);

        List<string> GetDuplicateParameters(List<ParameterKeyValue> parameters);

        List<ParameterKeyValue> TransformParameterRequest(List<TemplateParameterRequest> parameters);

        void CreateTemplateJson(TemplateConfig config, string toDirectory, string configFilename);

        void CreateTemplateJson(TemplateConfig config, string toDirectory);
    }
}
