using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class Interpolation
    {
        public string prefix { get; set; }

        public string suffix { get; set; }

        public List<InterpolationTarget> targets { get; set; } = new List<InterpolationTarget>();
    }
}
