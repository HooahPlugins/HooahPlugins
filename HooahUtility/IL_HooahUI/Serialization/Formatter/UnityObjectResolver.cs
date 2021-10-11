using MessagePack;
using MessagePack.Formatters;
using UnityEngine;

namespace HooahUtility.Serialization.Formatter
{
    public class UnityObjectResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = (IFormatterResolver)new UnityObjectResolver();

        private UnityObjectResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>() => FormatterCache<T>.Formatter;

        private static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                if (typeof(Object).IsAssignableFrom(typeof(T)))
                    Formatter = (IMessagePackFormatter<T>)UnityObjectResolverGetFormatterHelper.GetFormatter<T>();
                else
                    Formatter = (IMessagePackFormatter<T>)null;
            }
        }
    }
}
