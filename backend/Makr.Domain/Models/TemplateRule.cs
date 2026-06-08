using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class TemplateRule
    {
        public TemplateCondition Condition { get; set; }

        public TemplateModifier Add { get; set; }

        public TemplateModifier Remove { get; set; }

        public TemplateModifier Replace { get; set; }
    }
}
