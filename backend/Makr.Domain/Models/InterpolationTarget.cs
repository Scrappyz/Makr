using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class InterpolationTarget
    {
        public PathSelect content { get; set; } = new PathSelect();

        public PathSelect filename { get; set; } = new PathSelect();
    }
}
