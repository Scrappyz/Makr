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

        public object? GreaterThan
        {
            get => _greaterThan;
            set => _greaterThan = JsonUtils.UnwrapJsonElement(value);
        }

        public object? LessThan
        {
            get => _lessThan;
            set => _lessThan = JsonUtils.UnwrapJsonElement(value);
        }

        public object? GreaterThanOrEqual
        {
            get => _greaterThanOrEqual;
            set => _greaterThanOrEqual = JsonUtils.UnwrapJsonElement(value);
        }

        public object? LessThanOrEqual
        {
            get => _lessThanOrEqual;
            set => _lessThanOrEqual = JsonUtils.UnwrapJsonElement(value);
        }

        // Recursive conditions for nested parameters
        public List<TemplateCondition> All { get; set; } = new List<TemplateCondition>();

        public List<TemplateCondition> Any { get; set; } = new List<TemplateCondition>();

        // Temporary fields
        private object? _equals;
        private object? _notEquals;
        private object? _greaterThan;
        private object? _lessThan;
        private object? _greaterThanOrEqual;
        private object? _lessThanOrEqual;
    }
}
