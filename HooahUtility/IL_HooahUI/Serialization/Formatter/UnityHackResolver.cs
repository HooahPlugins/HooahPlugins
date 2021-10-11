using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using MessagePack.Unity;

namespace HooahUtility.Serialization.Formatter
{
    public class UnityHackResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new UnityHackResolver();

        private static readonly IFormatterResolver[] Resolvers =
        {
            UnityObjectResolver.Instance,
            UnityResolver.Instance,
            DynamicGenericResolver.Instance,
            StandardResolver.Instance
        };

        private UnityHackResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>() => FormatterCache<T>.Formatter;

        private static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                foreach (IFormatterResolver resolver in Resolvers)
                {
                    var formatter = resolver.GetFormatter<T>();
                    if (formatter == null) continue;
                    Formatter = formatter;
                    break;
                }
            }
        }
    }
}
