using System;
using System.Collections.Generic;
using System.Text;

namespace Makr.Application.Pipeline.PathSelector
{
    public interface IPathSelector
    {
        List<string> GetPaths(string rootPath, List<string> include, List<string> exclude);

        List<string> FilterPaths(List<string> paths, List<string> include, List<string> exclude);

        List<string> SortByDepth(List<string> paths, char separator);

        List<string> SortByDepth(List<string> paths);

        List<string> SortByDepthDescending(List<string> paths, char separator);

        List<string> SortByDepthDescending(List<string> paths);

        string NormalizePath(string path);
    }
}
