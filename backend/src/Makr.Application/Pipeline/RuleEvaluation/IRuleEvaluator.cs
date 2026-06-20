using Makr.Domain.Models;
using Makr.Domain.Models.Template;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Application.Pipeline.RuleEvaluation
{
    public interface IRuleEvaluator
    {
        void ResolveRules(TemplateInitialization initConfig, List<TemplateRule> rules, List<ParameterKeyValue> parameters);

        void ResolveRules(TemplateInitialization initConfig, List<TemplateRule> rules, Dictionary<string, object?> parameters);

        bool EvaluateCondition(TemplateCondition condition, List<ParameterKeyValue> parameters);

        bool EvaluateCondition(TemplateCondition condition, Dictionary<string, object?> parameters);

        void ModifyInitialization(TemplateInitialization initConfig, TemplateModifierGroup modifiers);
    }
}
