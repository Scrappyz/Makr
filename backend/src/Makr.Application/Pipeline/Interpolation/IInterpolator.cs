using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Makr.Application.Pipeline.Interpolation
{
    public interface IInterpolator
    {
        string Interpolate(string str, string prefix, string suffix, Dictionary<string, string> parameters);

        string Interpolate(string str, string prefix, string suffix, Dictionary<string, string> parameters, Regex regex);

        void InterpolateContents(List<string> paths, string prefix, string suffix, Dictionary<string, string> parameters);

        void InterpolatePaths(List<string> paths, string prefix, string suffix, Dictionary<string, string> parameters);

        string ToString(object val);

        Regex CreateInterpolationRegex(string prefix, string suffix, IEnumerable<string> keys);

        Regex CreateInterpolationRegex(string prefix, string suffix, List<string> keys);

        bool IsValidPrefixSuffix(string prefix, string suffix);
    }
}
