using Makr.Domain.Models;
using Makr.Domain.Models.Template;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Makr.Application.Pipeline.RuleEvaluation
{
    public sealed class RuleEvaluator : IRuleEvaluator
    {
        public bool EvaluateCondition(TemplateCondition condition, List<ParameterKeyValue> parameters)
        {
            Dictionary<string, object?> parameterMap = parameters.ToDictionary(p => p.Key, p => p.Value);

            bool hasAll = condition.All.Count > 0;
            bool hasAny = condition.Any.Count > 0;
            bool hasSingle = !string.IsNullOrWhiteSpace(condition.Parameter);

            if (hasSingle)
            {
                return EvaluateSingleCondition(condition, parameterMap);
            }
            else if (hasAll)
            {
                return EvaluateAllConditions(condition.All, parameterMap);
            }
            else if (hasAny)
            {
                return EvaluateAnyConditions(condition.Any, parameterMap);
            }

            return false;
        }

        private bool EvaluateSingleCondition(TemplateCondition condition, Dictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey(condition.Parameter))
                return false;

            var actualValue = UnwrapJsonElement(parameters[condition.Parameter]);
            var expectedEquals = UnwrapJsonElement(condition.Equals);
            var expectedNotEquals = UnwrapJsonElement(condition.NotEquals);

            if (expectedEquals != null)
                return Equals(expectedEquals, actualValue);

            if (expectedNotEquals != null)
                return !Equals(expectedNotEquals, actualValue);

            return false;
        }

        private bool EvaluateAllConditions(List<TemplateCondition> conditions, Dictionary<string, object> parameters)
        {
            if (conditions.Count < 1)
                return false;

            List<bool> results = GetEvaluationResults(conditions, parameters);

            foreach (bool result in results)
            {
                if (!result)
                    return false;
            }

            return true;
        }

        private bool EvaluateAnyConditions(List<TemplateCondition> conditions, Dictionary<string, object> parameters)
        {
            if (conditions.Count < 1)
                return false;

            List<bool> results = GetEvaluationResults(conditions, parameters);

            foreach (bool result in results)
            {
                if (result)
                    return true;
            }

            return false;
        }

        private List<bool> GetEvaluationResults(List<TemplateCondition> conditions, Dictionary<string, object> parameters)
        {
            List<bool> results = new List<bool>();

            foreach (var condition in conditions)
            {
                bool hasAll = condition.All.Count > 0;
                bool hasAny = condition.Any.Count > 0;
                bool hasSingle = !string.IsNullOrWhiteSpace(condition.Parameter);

                if (hasAll)
                {
                    results.Add(EvaluateAllConditions(condition.All, parameters));
                }

                if (hasAny)
                {
                    results.Add(EvaluateAnyConditions(condition.Any, parameters));
                }

                if (hasSingle)
                {
                    results.Add(EvaluateSingleCondition(condition, parameters));
                }
            }

            return results;
        }

        private object? UnwrapJsonElement(object? value)
        {
            if (value is JsonElement element)
            {
                return element.ValueKind switch
                {
                    JsonValueKind.String => element.GetString(),  // ← THIS line fixes your case
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Number => element.GetDouble(),
                    JsonValueKind.Null => null,
                    _ => value
                };
            }
            return value;
        }
    }
}
