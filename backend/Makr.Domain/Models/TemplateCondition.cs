using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class TemplateCondition
    {
        public string Parameter { get; set; }

        public object Equals { get; set; }

        public object NotEquals { get; set; }

        public object GreaterThan { get; set; }

        public object LessThan { get; set; }

        public object GreaterThanOrEqual { get; set; }

        public object LessThanOrEqual { get; set; }

        // Recursive conditions for nested parameters
        public List<TemplateCondition> All { get; set; } = new List<TemplateCondition>();

        public List<TemplateCondition> Any { get; set; } = new List<TemplateCondition>();
    }
}
