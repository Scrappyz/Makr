using Makr.Domain.Helpers;

namespace Makr.Domain.Models.Template
{
    public class TemplateCondition
    {
        public string Parameter { get; set; }

        public object? Equals
        {
            get => _equals;
            set => _equals = JsonUtils.UnwrapJsonElement(value);
        }

        public object? NotEquals
        {
            get => _notEquals;
            set => _notEquals = JsonUtils.UnwrapJsonElement(value);
        }

        public object? GreaterThan { get; set; }

        public object? LessThan { get; set; }

        public object? GreaterThanOrEqual { get; set; }

        public object? LessThanOrEqual { get; set; }

        // Recursive conditions for nested parameters
        public List<TemplateCondition> All { get; set; } = new List<TemplateCondition>();

        public List<TemplateCondition> Any { get; set; } = new List<TemplateCondition>();

        // Temporary fields
        private object? _equals;
        private object? _notEquals;
    }
}
