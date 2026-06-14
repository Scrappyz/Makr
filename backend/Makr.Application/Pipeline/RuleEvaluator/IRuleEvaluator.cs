using Makr.Domain.Models;
using Makr.Domain.Models.Template;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Application.Pipeline.RuleEvaluator
{
    public interface IRuleEvaluator
    {
        bool EvaluateCondition(TemplateCondition condition, List<ParameterKeyValue> parameters);
    }
}
