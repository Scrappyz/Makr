namespace Makr.Domain.Models.Template
{
    public class TemplateRule
    {
        public required TemplateCondition If { get; set; }

        public TemplateModifierGroup Then { get; set; }
    }
}
