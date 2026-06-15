namespace Makr.Domain.Models.Template
{
    public class TemplateRule
    {
        public required TemplateCondition Condition { get; set; }

        public TemplateModifier Add { get; set; }

        public TemplateModifier Remove { get; set; }

        public TemplateModifier Replace { get; set; }
    }
}
