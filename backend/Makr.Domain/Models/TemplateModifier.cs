using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class TemplateModifier
    {
        public List<PathMatch> Selection { get; set; } = new List<PathMatch>();

        public Interpolation Interpolation { get; set; }
    }
}
