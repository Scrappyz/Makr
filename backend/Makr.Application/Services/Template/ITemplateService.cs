using Makr.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Application.Services.Template
{
    public interface ITemplateService
    {
        void InitializeTemplate(string templateId, Dictionary<string, string> parameters);

        List<string> GetDuplicateParameters(List<TemplateParameterRequest> parameters);
    }
}
