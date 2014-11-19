using System;
using System.Collections.Generic;

namespace NetTriple
{
    public static class ConverterSnippets
    {
        private static readonly Dictionary<string, Func<string, object>> _snippets = new Dictionary<string, Func<string, object>>();

        public static string AddSnippet(Func<string, object> snippet)
        {
            var key = Guid.NewGuid().ToString().Substring(0, 8);
            _snippets.Add(key, snippet);
            return key;
        }

        public static T Invoke<T>(string key, string input)
        {
            if (!_snippets.ContainsKey(key))
            {
                throw new InvalidOperationException("No such snippet");
            }

            var r = _snippets[key].Invoke(input);
            return (T) r;
        }
    }
}
