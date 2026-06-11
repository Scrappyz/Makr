using Makr.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Application.Services.Template
{
    public interface ITemplateService
    {
        public List<string> GetDuplicateParameters(List<TemplateParameter> parameters);
    }
}
