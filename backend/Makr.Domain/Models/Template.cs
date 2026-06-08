using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Domain.Models
{
    public class Template
    {
        public int ManifestVersion { get; set; }

        public TemplateMetadata Metadata { get; set; }

        public TemplateInitialization Initialization { get; set; }

        public List<TemplateRule> Rules { get; set; }
    }
}
