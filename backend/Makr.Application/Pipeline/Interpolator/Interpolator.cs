using Makr.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Makr.Domain.Helpers;
using Makr.Application.Pipeline.PathSelector;

namespace Makr.Application.Pipeline.Interpolator
{
    public sealed class Interpolator : IInterpolator
    {
        private readonly IPathSelector _pathSelector;

        public Interpolator(IPathSelector pathSelector)
        {
            _pathSelector = pathSelector;
        }

        public string Interpolate(string str, string prefix, string suffix, Dictionary<string, string> parameters)
        {
            if (parameters.Count == 0) 
                return str;

            Regex regex = CreateInterpolationRegex(prefix, suffix, parameters.Keys);

            return regex.Replace(str, match => parameters[RemovePrefixSuffix(match.Value, prefix, suffix)]);
        }

        public string Interpolate(string str, string prefix, string suffix, Dictionary<string, string> parameters, Regex regex)
        {
            if (parameters.Count == 0) 
                return str;

            return regex.Replace(str, match => parameters[RemovePrefixSuffix(match.Value, prefix, suffix)]);
        }

        public void InterpolateContents(List<string> paths, string prefix, string suffix, Dictionary<string, string> parameters)
        {
            if (paths.Count == 0 || 
                string.IsNullOrEmpty(prefix) || 
                string.IsNullOrEmpty(suffix) ||
                parameters.Count == 0
            )
                return;

            Regex regex = CreateInterpolationRegex(prefix, suffix, parameters.Keys);

            foreach (string path in paths)
            {
                PathType? pathType = FilesystemUtils.GetPathType(path);

                if (!pathType.HasValue || pathType.Value == PathType.Directory)
                    continue;

                FileType? fileType = FilesystemUtils.GetFileType(path);

                if (!fileType.HasValue || fileType.Value != FileType.Text)
                    continue;

                string content = File.ReadAllText(path);

                string interpolatedContent = Interpolate(content, prefix, suffix, parameters, regex);
                File.WriteAllText(path, interpolatedContent);
            }
        }

        public void InterpolatePaths(List<string> paths, string prefix, string suffix, Dictionary<string, string> parameters)
        {
            if (paths.Count == 0 || 
                string.IsNullOrEmpty(prefix) || 
                string.IsNullOrEmpty(suffix) ||
                parameters.Count == 0
            )
                return;

            paths = _pathSelector.SortByDepthDescending(paths);

            Regex regex = CreateInterpolationRegex(prefix, suffix, parameters.Keys);

            foreach (string path in paths)
            {
                if (!FilesystemUtils.Exists(path))
                    continue;

                string fileName = Path.GetFileName(path);
                string newFileName = Interpolate(fileName, prefix, suffix, parameters, regex);

                if (fileName != newFileName)
                    FilesystemUtils.Rename(path, newFileName);
            }
        }

        public string ToString(object val)
        {
            if (val == null)
                return "";

            switch (val)
            {
                case string s: return s;
                case bool b: return b ? "true" : "false";
                case char c: return c.ToString();
                case IFormattable formattable:
                    return formattable.ToString(null, CultureInfo.InvariantCulture);
                default:
                    return val.ToString() ?? "";
            }
        }

        public Regex CreateInterpolationRegex(string prefix, string suffix, IEnumerable<string> keys)
        {
            string pattern = string.Join("|", keys
                .OrderByDescending(k => k.Length)
                .Select(k => Regex.Escape($"{prefix}{k}{suffix}"))
            );
            return new Regex(pattern);
        }

        public Regex CreateInterpolationRegex(string prefix, string suffix, List<string> keys)
        {
            string pattern = string.Join("|", keys
                .OrderByDescending(k => k.Length)
                .Select(k => Regex.Escape($"{prefix}{k}{suffix}"))
            );
            return new Regex(pattern);
        }

        private string RemovePrefixSuffix(string str, string prefix, string suffix)
        {
            if (str.StartsWith(prefix) && str.EndsWith(suffix))
            {
                return str[(prefix.Length)..(str.Length - suffix.Length)];
            }
            return str;
        }
    }
}
