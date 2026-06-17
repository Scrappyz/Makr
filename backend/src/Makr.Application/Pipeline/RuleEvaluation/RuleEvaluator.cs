using Makr.Domain.Helpers;
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

            return EvaluateCondition(condition, parameterMap);
        }

        public bool EvaluateCondition(TemplateCondition condition, Dictionary<string, object?> parameters)
        {
            bool hasAll = condition.All.Count > 0;
            bool hasAny = condition.Any.Count > 0;
            bool hasSingle = !string.IsNullOrWhiteSpace(condition.Parameter);

            if (hasSingle)
            {
                return EvaluateSingleCondition(condition, parameters);
            }
            else if (hasAll)
            {
                return EvaluateAllConditions(condition.All, parameters);
            }
            else if (hasAny)
            {
                return EvaluateAnyConditions(condition.Any, parameters);
            }

            return false;
        }

        private bool EvaluateSingleCondition(TemplateCondition condition, Dictionary<string, object> parameters)
        {
            if (!parameters.ContainsKey(condition.Parameter))
                return false;

            var actualValue = parameters[condition.Parameter];
            var expectedEquals = condition.Equals;
            var expectedNotEquals = condition.NotEquals;

            if (expectedEquals != null)
                return actualValue.Equals(expectedEquals);

            if (expectedNotEquals != null)
                return !actualValue.Equals(expectedNotEquals);

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
    }
}
