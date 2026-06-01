using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class TemplateParameterInfo
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string InputType { get; set; }

        public string DefaultValue { get; set; } = string.Empty;

        public bool Required { get; set; } = false;
    }
}
