using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Dwaas.Param.Documentation.Analyzer
{
    internal static class Lookup
    {
        private static Dictionary<string, string> _lookup = null;
        private static int _lookupReload = 0;

        public static bool Load(ImmutableArray<AdditionalText> additionalFiles)
        {
            _lookupReload = (_lookupReload + 1) % 10;
            if (_lookup == null || _lookupReload == 0)
            {
                var additionalFile = additionalFiles.FirstOrDefault(file => file.Path.Contains(Constants.DiagnosticFile));
                if (additionalFile == null)
                {
                    return false;
                }

                string json = additionalFile.GetText().ToString();

                _lookup =
                    new Dictionary<string, string>(
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(json),
                        StringComparer.OrdinalIgnoreCase);
            }
            return true;
        }

        public static bool TryGetValue(string key, out string value)
        {
            return _lookup.TryGetValue(key, out value);
        }
    }
}