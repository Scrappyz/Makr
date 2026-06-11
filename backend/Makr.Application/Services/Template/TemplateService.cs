using Makr.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Application.Services.Template
{
    public sealed class TemplateService : ITemplateService
    {
        public List<string> GetDuplicateParameters(List<TemplateParameter> parameters)
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
