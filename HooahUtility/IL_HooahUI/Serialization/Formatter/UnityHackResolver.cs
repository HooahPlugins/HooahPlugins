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
