using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Makr.Application.Pipeline.PathSelection
{
    public sealed class PathSelector : IPathSelector
    {
        public List<string> GetPaths(string rootPath, List<string> include, List<string> exclude)
        {
            // Remove duplicate patterns
            include = include.Distinct().ToList();
            exclude = exclude.Distinct().ToList();

            var matcher = new Matcher();

            matcher.AddIncludePatterns(include);
            matcher.AddExcludePatterns(exclude);

            var allEntries = Directory.GetFileSystemEntries(rootPath, "*", SearchOption.AllDirectories);
            List<string> result = new();

            foreach (var entry in allEntries)
            {
                string relativePath = Path.GetRelativePath(rootPath, entry)
                    .Replace('\\', '/');

                if (matcher.Match(relativePath).HasMatches)
                {
                    result.Add(relativePath);
                }
            }

            return result;
        }

        public List<string> FilterPaths(List<string> paths, List<string> include, List<string> exclude)
        {
            // Remove duplicate patterns
            include = include.Distinct().ToList();
            exclude = exclude.Distinct().ToList();

            var matcher = new Matcher();

            matcher.AddIncludePatterns(include);
            matcher.AddExcludePatterns(exclude);

            List<string> result = new();
            foreach (var path in paths)
            {
                if (matcher.Match(path).HasMatches)
                {
                    result.Add(path);
                }
            }

            return result;
        }

        public List<string> SortByDepth(List<string> paths, char separator)
        {
            List<(int, string)> sortedPaths = CountSeparator(paths, separator);
            return sortedPaths.OrderBy(p => p.Item1).Select(p => p.Item2).ToList();
        }

        public List<string> SortByDepth(List<string> paths)
        {
            return SortByDepth(paths, '\0');
        }

        public List<string> SortByDepthDescending(List<string> paths, char separator)
        {
            List<(int, string)> sortedPaths = CountSeparator(paths, separator);
            return sortedPaths.OrderByDescending(p => p.Item1).Select(p => p.Item2).ToList();
        }

        public List<string> SortByDepthDescending(List<string> paths)
        {
            return SortByDepthDescending(paths, '\0');
        }

        public string NormalizePath(string path)
        {
            path = Path.TrimEndingDirectorySeparator(path);
            return path
                    .Replace('\\', Path.DirectorySeparatorChar)
                    .Replace('/', Path.DirectorySeparatorChar);
        }

        // Splits the path by the specified separator and counts the number of segments
        private List<(int, string)> CountSeparator(List<string> paths, char separator)
        {
            char compareSeparator = Path.DirectorySeparatorChar;
            if (separator != '\0')
                compareSeparator = separator;

            List<(int, string)> sortedPaths = new List<(int, string)>();
            foreach (var path in paths)
            {
                int separatorCount = path.Count(c => c == compareSeparator);
                if (path.EndsWith(compareSeparator))
                    separatorCount--;
                sortedPaths.Add((separatorCount, path));
            }

            return sortedPaths;
        }
    }
}

