using MessagePack;
using MessagePack.Formatters;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HooahUtility.Serialization.Formatter
{
    public class UnityObjectFormatter<T> : IMessagePackFormatter<T>
    {
        public int Serialize(
            ref byte[] bytes,
            int offset,
            T value,
            IFormatterResolver formatterResolver)
        {
            return value == null || UnityObjectFormatter.Context == null
                ? MessagePackBinary.WriteNil(ref bytes, offset)
                : MessagePackBinary.WriteInt32(ref bytes, offset,
                    UnityObjectFormatter.Context.AddReference(value as Object));
        }

        T IMessagePackFormatter<T>.Deserialize(
            byte[] bytes,
            int offset,
            IFormatterResolver formatterResolver,
            out int readSize)
        {
            if (UnityObjectFormatter.Context == null || MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return default;
            }

            var instanceID = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
            Object reference;
            return (T) (UnityObjectFormatter.Context.TryGetReference(instanceID, out reference)
                ? Convert.ChangeType(reference, typeof(T))
                : null);
        }
    }
}