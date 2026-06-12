using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class TemplateInitialization
    {
        public List<TemplateParameterRequest> Parameters { get; set; } = new List<TemplateParameterRequest>();

        public List<PathSelect> Selection { get; set; } = new List<PathSelect>();

        public Interpolation Interpolation { get; set; }
    }
}
