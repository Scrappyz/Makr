using Makr.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Application.Services
{
    public class TemplateService
    {


        public List<string> GetDuplicateVariables(TemplateVariable[] vars)
        {
            var groupVars = vars.GroupBy(v => v.Name).ToList();
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
