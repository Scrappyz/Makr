using System;
namespace Makr.Domain.Models.Template
{
    public class TemplateConfig
    {
        public int ManifestVersion { get; set; }

        public required TemplateMetadata Metadata { get; set; }

        public TemplateInitialization Initialization { get; set; }
    }
}
