using System;
using System.Collections.Generic;

namespace HooahUtility.Serialization.Formatter
{
    internal static class UnityObjectResolverGetFormatterHelper
    {
        private static Dictionary<Type, object> formatters = new Dictionary<Type, object>();

        internal static object GetFormatter<T>()
        {
            if (formatters.TryGetValue(typeof(T), out var obj)) return obj as UnityObjectFormatter<T>;
            var unityObjectFormatter = new UnityObjectFormatter<T>();
            formatters[typeof(T)] = unityObjectFormatter;
            return unityObjectFormatter;
        }
    }
}
