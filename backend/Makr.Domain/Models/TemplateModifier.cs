using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class TemplateModifier
    {
        public List<PathSelect> Selection { get; set; } = new List<PathSelect>();

        public Interpolation Interpolation { get; set; }
    }
}
