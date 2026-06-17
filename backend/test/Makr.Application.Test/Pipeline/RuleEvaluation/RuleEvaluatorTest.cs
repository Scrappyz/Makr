using Makr.Application.Pipeline.RuleEvaluation;
using Makr.Domain.Models;
using Makr.Domain.Models.Template;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Makr.Application.Test.Pipeline.RuleEvaluation
{
    public class RuleEvaluatorTest
    {
        public class EvaluateCondition
        {
            #region Complex_ReturnsCorrect_TestCases
            public static IEnumerable<object[]> Complex_ReturnsCorrect_TestCases()
            {
                yield return new object[]
                {
                    new TemplateCondition()
                    {
                        All = new List<TemplateCondition>()
                        {
                            new TemplateCondition()
                            {
                                Parameter = "someText",
                                Equals = "potato",
                            },
                            new TemplateCondition()
                            {
                                Any = new List<TemplateCondition>()
                                {
                                    new TemplateCondition()
                                    {
                                        Parameter = "withSomething",
                                        Equals = true
                                    },
                                    new TemplateCondition()
                                    {
                                        Parameter = "someNumber",
                                        NotEquals = 5
                                    }
                                }
                            }
                        }
                    },
                    new List<ParameterKeyValue>()
                    {
                        new ParameterKeyValue()
                        {
                            Key = "someText",
                            Value = "potato"
                        },
                        new ParameterKeyValue()
                        {
                            Key = "withSomething",
                            Value = true
                        },
                        new ParameterKeyValue()
                        {
                            Key = "someNumber",
                            Value = 1
                        }
                    },
                    true
                };

                yield return new object[]
                {
                    new TemplateCondition()
                    {
                        All = new List<TemplateCondition>()
                        {
                            new TemplateCondition()
                            {
                                Parameter = "someText",
                                Equals = "potato",
                            },
                            new TemplateCondition()
                            {
                                Any = new List<TemplateCondition>()
                                {
                                    new TemplateCondition()
                                    {
                                        Parameter = "withSomething",
                                        Equals = false
                                    },
                                    new TemplateCondition()
                                    {
                                        Parameter = "someNumber",
                                        NotEquals = 5
                                    }
                                }
                            }
                        }
                    },
                    new List<ParameterKeyValue>()
                    {
                        new ParameterKeyValue()
                        {
                            Key = "someText",
                            Value = "potato"
                        },
                        new ParameterKeyValue()
                        {
                            Key = "withSomething",
                            Value = true
                        },
                        new ParameterKeyValue()
                        {
                            Key = "someNumber",
                            Value = 5
                        }
                    },
                    false
                };
            }
            #endregion

            [Theory]
            [MemberData(nameof(Complex_ReturnsCorrect_TestCases))]
            public void Complex_ReturnsCorrect(TemplateCondition condition, List<ParameterKeyValue> parameters, bool expected)
            {
                var ruleEvaluator = new RuleEvaluator();

                bool actual = ruleEvaluator.EvaluateCondition(condition, parameters);

                Assert.Equal(expected, actual);
            }
        }
    }
}
