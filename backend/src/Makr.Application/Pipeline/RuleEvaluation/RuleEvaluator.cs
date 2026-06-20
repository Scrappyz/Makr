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
        public void ResolveRules(TemplateInitialization initConfig, List<TemplateRule> rules, List<ParameterKeyValue> parameters)
        {
            Dictionary<string, object?> parameterMap = parameters.ToDictionary(p => p.Key, p => p.Value);
            ResolveRules(initConfig, rules, parameterMap);
        }

        public void ResolveRules(TemplateInitialization initConfig, List<TemplateRule> rules, Dictionary<string, object?> parameters)
        {
            foreach (var rule in rules)
            {
                TemplateCondition condition = rule.If;
                TemplateModifierGroup modifiers = rule.Then;

                if (condition == null || modifiers == null)
                    continue;

                bool willThen = EvaluateCondition(condition, parameters);

                if (!willThen)
                    continue;

                ModifyInitialization(initConfig, modifiers);
            }
        }

        public bool EvaluateCondition(TemplateCondition condition, List<ParameterKeyValue> parameters)
        {
            if (condition == null || parameters.Count < 1)
                return false;

            Dictionary<string, object?> parameterMap = parameters.ToDictionary(p => p.Key, p => p.Value);
            return EvaluateCondition(condition, parameterMap);
        }

        public bool EvaluateCondition(TemplateCondition condition, Dictionary<string, object?> parameters)
        {
            if (condition == null || parameters.Count < 1)
                return false;

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

        public void ModifyInitialization(TemplateInitialization initConfig, TemplateModifierGroup modifiers)
        {
            if (initConfig == null || modifiers == null)
                return;

            TemplateModifier? add = modifiers.Add;
            TemplateModifier? remove = modifiers.Remove;
            TemplateModifier? replace = modifiers.Replace;

            bool hasAdd = add != null;
            bool hasRemove = remove != null;
            bool hasReplace = replace != null;

            if (hasReplace)
            {
                // Selection
                initConfig.Selection = replace.Selection ?? new PathSelect();

                // Interpolation
                TemplateInterpolation prevInterpolation = initConfig.Interpolation;
                initConfig.Interpolation = replace.Interpolation ?? new TemplateInterpolation();
                initConfig.Interpolation.Prefix = prevInterpolation.Prefix;
                initConfig.Interpolation.Suffix = prevInterpolation.Suffix;
                
                if (initConfig.Interpolation.Targets == null)
                {
                    initConfig.Interpolation.Targets = prevInterpolation.Targets;
                }
            }

            if (hasAdd)
            {
                // Selection
                if (add.Selection != null)
                {
                    initConfig.Selection.include.AddRange(add.Selection.include);
                    initConfig.Selection.exclude.AddRange(add.Selection.exclude);
                }

                // Interpolation
                if (add.Interpolation != null && add.Interpolation.Targets != null)
                {
                    initConfig.Interpolation.Targets.Contents.include.AddRange(add.Interpolation.Targets.Contents.include);
                    initConfig.Interpolation.Targets.Contents.exclude.AddRange(add.Interpolation.Targets.Contents.exclude);
                    initConfig.Interpolation.Targets.Filenames.include.AddRange(add.Interpolation.Targets.Filenames.include);
                    initConfig.Interpolation.Targets.Filenames.exclude.AddRange(add.Interpolation.Targets.Filenames.exclude);
                }
            }

            if (hasRemove)
            {
                // Selection
                if (remove.Selection != null)
                {
                    RemoveListFromOther(initConfig.Selection.include, remove.Selection.include);
                    RemoveListFromOther(initConfig.Selection.exclude, remove.Selection.exclude);
                }

                // Interpolation
                if (remove.Interpolation != null && remove.Interpolation.Targets != null)
                {
                    RemoveListFromOther(initConfig.Interpolation.Targets.Contents.include, remove.Interpolation.Targets.Contents.include);
                    RemoveListFromOther(initConfig.Interpolation.Targets.Contents.exclude, remove.Interpolation.Targets.Contents.exclude);
                    RemoveListFromOther(initConfig.Interpolation.Targets.Filenames.include, remove.Interpolation.Targets.Filenames.include);
                    RemoveListFromOther(initConfig.Interpolation.Targets.Filenames.exclude, remove.Interpolation.Targets.Filenames.exclude);
                }
            }
        }

        private void RemoveListFromOther<T>(List<T> listA,  List<T> listB)
        {
            HashSet<T> lookup = new HashSet<T>(listB);
            listA.RemoveAll(item => lookup.Contains(item));
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
