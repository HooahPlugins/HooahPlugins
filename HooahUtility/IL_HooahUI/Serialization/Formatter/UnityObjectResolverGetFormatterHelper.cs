using System;
using System.Collections.Generic;

namespace HooahUtility.Serialization.Formatter
{
  internal static class UnityObjectResolverGetFormatterHelper
  {
    private static Dictionary<Type, object> formatters = new Dictionary<Type, object>();

    internal static object GetFormatter<T>()
    {
      object obj;
      if (UnityObjectResolverGetFormatterHelper.formatters.TryGetValue(typeof (T), out obj))
        return (object) (obj as UnityObjectFormatter<T>);
      UnityObjectFormatter<T> unityObjectFormatter = new UnityObjectFormatter<T>();
      UnityObjectResolverGetFormatterHelper.formatters[typeof (T)] = (object) unityObjectFormatter;
      return (object) unityObjectFormatter;
    }
  }
}
