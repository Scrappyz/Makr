using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class InterpolationTarget
    {
        public PathMatch content { get; set; } = new PathMatch();

        public PathMatch filename { get; set; } = new PathMatch();
    }
}
