namespace Makr.Domain.Models.Template
{
    public class TemplateInitialization
    {
        public List<TemplateParameter> Parameters { get; set; } = new List<TemplateParameter>();

        public PathSelect Selection { get; set; }

        public Interpolation Interpolation { get; set; }

        public List<TemplateRule> Rules { get; set; } = new List<TemplateRule>();
    }
}
