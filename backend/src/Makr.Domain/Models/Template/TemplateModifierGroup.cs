using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models.Template
{
    public class TemplateModifierGroup
    {
        public TemplateModifier Add { get; set; }

        public TemplateModifier Remove { get; set; }

        public TemplateModifier Replace { get; set; }
    }
}
