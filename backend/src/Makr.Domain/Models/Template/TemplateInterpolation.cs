namespace Makr.Domain.Models.Template
{
    public class TemplateInterpolation
    {
        public string Prefix { get; set; }

        public string Suffix { get; set; }

        public TemplateInterpolationTarget Targets { get; set; }
    }
}
