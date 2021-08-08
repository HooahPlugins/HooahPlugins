// Decompiled with JetBrains decompiler
// Type: HooahUtility.Serialization.Formatter.UnityObjectResolverGetFormatterHelper
// Assembly: HooahUtilityEditorCompatible, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 52B179E7-4743-4027-9930-3D3BFEF61A6C
// Assembly location: D:\himates\AIHSModding\Assets\Plugins\Hooah\HooahUtilityEditorCompatible.dll

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
