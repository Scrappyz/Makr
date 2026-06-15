using Makr.Application.Pipeline.Interpolation;
using Makr.Application.Pipeline.PathSelection;
using Makr.Domain.Models.Template;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Makr.Application.Test.Pipeline.Interpolation
{
    public class InterpolatorTest
    {
        public static IEnumerable<object[]> InterpolationTestCases()
        {
            yield return new object[]
            {
                "I am {{name}} and I am {{age}} years old",
                "{{",
                "}}",
                new Dictionary<string, string>()
                {
                    { "name", "John Doe" },
                    { "age", "12" }
                },
                "I am John Doe and I am 12 years old"
            };
        }

        [Theory]
        [MemberData(nameof(InterpolationTestCases))]
        public void Interpolate_ValidOutput(string input, string prefix, string suffix, Dictionary<string, string> parameters, string expected)
        {
            // Arrange
            PathSelector pathSelector = new PathSelector(); // or provide a test double implementing IPathSelector
            Interpolator interpolator = new Interpolator(pathSelector);

            string output = interpolator.Interpolate(input, prefix, suffix, parameters);

            Assert.Equal(expected, output);
        }
    }
}
