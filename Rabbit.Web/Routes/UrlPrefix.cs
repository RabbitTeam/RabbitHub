using System;

namespace Rabbit.Web.Routes
{
    internal sealed class UrlPrefix
    {
        private readonly string _prefix;

        public UrlPrefix(string prefix)
        {
            _prefix = prefix.TrimStart('~').Trim('/');
        }

        public string RemoveLeadingSegments(string path)
        {
            var beginIndex = 0;
            if (path.Length > beginIndex && path[beginIndex] == '~')
                ++beginIndex;
            if (path.Length > beginIndex && path[beginIndex] == '/')
                ++beginIndex;

            var endIndex = beginIndex + _prefix.Length;
            if (path.Length == endIndex)
            {
            }
            else if (path.Length > endIndex && path[endIndex] == '/')
            {
                ++endIndex;
            }
            else
            {
                return path;
            }

            if (string.Compare(path, beginIndex, _prefix, 0, _prefix.Length, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return path.Substring(0, beginIndex) + path.Substring(endIndex);
            }

            return path;
        }

        public string PrependLeadingSegments(string path)
        {
            if (path == "~")
            {
                return "~/" + _prefix + "/";
            }

            var index = 0;
            if (path.Length > index && path[index] == '~')
                ++index;
            if (path.Length > index && path[index] == '/')
                ++index;

            return path.Substring(0, index) + _prefix + '/' + path.Substring(index);
        }
    }
}