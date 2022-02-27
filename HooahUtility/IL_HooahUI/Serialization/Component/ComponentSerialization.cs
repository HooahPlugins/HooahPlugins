using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HooahUtility.Model;
using HooahUtility.Serialization.Formatter;
using MessagePack;
using Utility;

namespace HooahUtility.Serialization.Component
{
    public static class SerializeHelper
    {
        public static Dictionary<object, object> ComponentSerializeV2<T>(T component) where T : IFormData
        {
            return component == null
                ? null
                : SerializationUtility.GetAllSerializableFields(component)
                    .Select(x => new KeyValuePair<object, object>(x.Key, FieldSerializeV2(x.Value, component)))
                    .ToDictionary(x => x.Key, x => x.Value);
        }

        //region Component Serializer
        public static void ComponentDeserializeV1<T>(T component, byte[] bytes) where T : IFormData
        {
            var serializableFields = SerializationUtility.GetAllSerializableFields(component);

            foreach (var keyValuePair in MessagePackSerializer.Deserialize<Dictionary<object, object>>(bytes,
                         UnityHackResolver.Instance))
            {
                if (!serializableFields.TryGetValue(keyValuePair.Key, out var memberInfo)) continue;

                try
                {
                    FieldDeserializeV1(memberInfo, component, keyValuePair.Value);
                }
                catch (Exception e)
                {
                    SerializationUtility.HandleError(true,
                        $"Failed to deserialize some data from field {component.GetType().Name}::{memberInfo.Name}.\n{e}",
                        2);
                }
            }
        }

        public static void ComponentDeserializeV2<T>(T component, byte[] bytes) where T : IFormData
        {
            if (component == null) return;
            var serializableFields = SerializationUtility.GetAllSerializableFields(component);
            foreach (var keyValuePair in MessagePackSerializer.Deserialize<Dictionary<object, object>>(bytes,
                         UnityHackResolver.Instance))
            {
                if (!serializableFields.TryGetValue(keyValuePair.Key, out var memberInfo)) continue;

                try
                {
                    FieldDeserializeV2(memberInfo, component, keyValuePair.Value);
                }
                catch (Exception e)
                {
                    SerializationUtility.HandleError(true,
                        $"Failed to deserialize some data from field {component.GetType().Name}::{memberInfo.Name}.\n{e}",
                        2);
                }
            }
        }
        //endregion

        public static object FieldSerializeV2<T>(MemberInfo memberInfo, T component) where T : IFormData
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    return SerializationUtility.Serialize(fieldInfo.FieldType, fieldInfo.GetValue(component));
                case PropertyInfo propertyInfo:
                    return SerializationUtility.Serialize(propertyInfo.PropertyType, propertyInfo.GetValue(component));
                default:
                    return null;
            }
        }

        public static void FieldDeserializeV1<T>(MemberInfo memberInfo, T component, object value) where T : IFormData
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(component, value);
                    break;
                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(component, value);
                    break;
            }
        }

        public static void FieldDeserializeV2<T>(MemberInfo memberInfo, T component, object value)
            where T : IFormData
        {
            if (!(value is byte[] valueBytes)) return;

            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(component,
                        SerializationUtility.Deserialize(fieldInfo.FieldType, valueBytes));
                    return;
                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(component,
                        SerializationUtility.Deserialize(propertyInfo.PropertyType, valueBytes));
                    return;
            }
        }
    }
}
