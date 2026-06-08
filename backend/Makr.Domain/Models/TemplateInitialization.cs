using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class TemplateInitialization
    {
        public List<TemplateParameter> Parameters { get; set; } = new List<TemplateParameter>();

        public List<PathMatch> Selection { get; set; } = new List<PathMatch>();

        public Interpolation Interpolation { get; set; }
    }
}
