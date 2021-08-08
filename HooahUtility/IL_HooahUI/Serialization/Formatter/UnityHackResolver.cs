// Decompiled with JetBrains decompiler
// Type: HooahUtility.Serialization.Formatter.UnityHackResolver
// Assembly: HooahUtilityEditorCompatible, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 52B179E7-4743-4027-9930-3D3BFEF61A6C
// Assembly location: D:\himates\AIHSModding\Assets\Plugins\Hooah\HooahUtilityEditorCompatible.dll

using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using MessagePack.Unity;

namespace HooahUtility.Serialization.Formatter
{
  public class UnityHackResolver : IFormatterResolver
  {
    public static IFormatterResolver Instance = (IFormatterResolver) new UnityHackResolver();
    private static readonly IFormatterResolver[] resolvers = new IFormatterResolver[]
    {
      UnityObjectResolver.Instance,
      UnityResolver.Instance,
      DynamicGenericResolver.Instance, 
      StandardResolver.Instance
    };

    private UnityHackResolver()
    {
    }

    public IMessagePackFormatter<T> GetFormatter<T>() => UnityHackResolver.FormatterCache<T>.formatter;

    private static class FormatterCache<T>
    {
      public static readonly IMessagePackFormatter<T> formatter;

      static FormatterCache()
      {
        foreach (IFormatterResolver resolver in UnityHackResolver.resolvers)
        {
          IMessagePackFormatter<T> formatter = resolver.GetFormatter<T>();
          if (formatter != null)
          {
            UnityHackResolver.FormatterCache<T>.formatter = formatter;
            break;
          }
        }
      }
    }
  }
}
