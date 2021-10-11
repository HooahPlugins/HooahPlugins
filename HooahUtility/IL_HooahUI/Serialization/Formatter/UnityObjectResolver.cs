using MessagePack;
using MessagePack.Formatters;
using UnityEngine;

namespace HooahUtility.Serialization.Formatter
{
  public class UnityObjectResolver : IFormatterResolver
  {
    public static IFormatterResolver Instance = (IFormatterResolver) new UnityObjectResolver();

    private UnityObjectResolver()
    {
    }

    public IMessagePackFormatter<T> GetFormatter<T>() => UnityObjectResolver.FormatterCache<T>.formatter;

    private static class FormatterCache<T>
    {
      public static readonly IMessagePackFormatter<T> formatter;

      static FormatterCache()
      {
        if (typeof (Object).IsAssignableFrom(typeof (T)))
          UnityObjectResolver.FormatterCache<T>.formatter = (IMessagePackFormatter<T>) UnityObjectResolverGetFormatterHelper.GetFormatter<T>();
        else
          UnityObjectResolver.FormatterCache<T>.formatter = (IMessagePackFormatter<T>) null;
      }
    }
  }
}
