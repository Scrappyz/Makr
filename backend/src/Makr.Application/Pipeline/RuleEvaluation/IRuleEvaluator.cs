using Makr.Domain.Models;
using Makr.Domain.Models.Template;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Application.Pipeline.RuleEvaluation
{
    public interface IRuleEvaluator
    {
        bool EvaluateCondition(TemplateCondition condition, List<ParameterKeyValue> parameters);

        bool EvaluateCondition(TemplateCondition condition, Dictionary<string, object?> parameters);
    }
}
