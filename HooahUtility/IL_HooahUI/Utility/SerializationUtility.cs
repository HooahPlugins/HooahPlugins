using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HooahUtility;
using HooahUtility.Model;
using HooahUtility.Serialization.Component;
using HooahUtility.Serialization.Formatter;
using HooahUtility.Serialization.StudioReference;
using MessagePack;
using UnityEngine;

namespace Utility
{
    public static class SerializationUtility
    {
        private static MethodInfo _serialize;
        private static MethodInfo _deserialize;

        public static void HandleError(string msg)
        {
#if AI || HS2
            HooahUtilityPlugin.Instance.Log.LogError($"Save/Load Error: {msg}");
#else
            Debug.LogWarning(msg);
#endif
        }

        public static void HandleError(bool isSer, string msg, int version)
        {
            var action = isSer ? "Save" : "Load";
            HandleError($"[{version}:{action}]: {msg}");
        }

        static SerializationUtility()
        {
            foreach (var method in typeof(MessagePackSerializer).GetMethods())
            {
                switch (method.Name)
                {
                    case "Deserialize":
                        var parameters1 = method.GetParameters();
                        if (parameters1.Length == 2 && parameters1[0].ParameterType == typeof(byte[]) &&
                            parameters1[1].ParameterType == typeof(IFormatterResolver))
                        {
                            _deserialize = method;
                        }

                        break;
                    case "Serialize":
                        var parameters2 = method.GetParameters();
                        if (parameters2.Length == 2 && parameters2[1].ParameterType == typeof(IFormatterResolver) &&
                            method.ReturnType == typeof(byte[]))
                        {
                            _serialize = method;
                        }

                        break;
                }
            }
        }

        public static IFormData GetSerializableComponent(GameObject gameObject) =>
            gameObject.GetComponent<IFormData>() ?? gameObject.GetComponentInChildren<IFormData>();

        public static HooahBehavior[] GetSerializableComponents(GameObject gameObject) =>
            gameObject.GetComponents<HooahBehavior>() ?? gameObject.GetComponentsInChildren<HooahBehavior>();

        public static Dictionary<string, HooahBehavior> GetSerializableComponentsInDictionary(GameObject gameObject) =>
            GetSerializableComponents(gameObject).ToDictionary(x => x.GetType().Name, x => x);


        public static Dictionary<object, MemberInfo> GetAllSerializableFields<T>(T component) where T : IFormData
        {
            if (component == null) return null;
            var fields = new Dictionary<object, MemberInfo>();

            foreach (var memberInfo in component.GetType().GetMembers())
            {
                var customAttribute = memberInfo.GetCustomAttribute<KeyAttribute>();
                if (customAttribute == null) continue;
                switch (memberInfo)
                {
                    case PropertyInfo propertyInfo when propertyInfo.CanRead && propertyInfo.CanWrite:
                    case FieldInfo _:
                        var intKey = customAttribute.IntKey;
                        if (intKey.HasValue) fields[intKey.Value.ToString()] = memberInfo;
                        if (!ReferenceEquals(null, customAttribute.StringKey) && customAttribute.StringKey.Length >= 0)
                            fields[customAttribute.StringKey] = memberInfo;
                        break;
                }
            }

            return fields;
        }

        public static Dictionary<Type, MethodInfo> SerializationMethodInfo = new Dictionary<Type, MethodInfo>();
        public static Dictionary<Type, MethodInfo> DeserializationMethodInfo = new Dictionary<Type, MethodInfo>();

        public static byte[] Serialize(Type type, object value)
        {
            var parameters = new[] { value, UnityHackResolver.Instance };
            if (SerializationMethodInfo.TryGetValue(type, out var method))
                return (byte[])method.Invoke(null, parameters);

            method = _serialize.MakeGenericMethod(type);
            SerializationMethodInfo[type] = method;
            return (byte[])method.Invoke(null, parameters);
        }

        public static object Deserialize(Type type, byte[] bytes)
        {
            var parameters = new object[] { bytes, UnityHackResolver.Instance };
            if (DeserializationMethodInfo.TryGetValue(type, out var method))
                return method.Invoke(null, parameters);

            method = _deserialize.MakeGenericMethod(type);
            DeserializationMethodInfo[type] = method;
            return method.Invoke(null, parameters);
        }


        public static Dictionary<object, object> GetSerializedData<T>(T component) where T : IFormData =>
            SerializeHelper.ComponentSerializeV2(component);

        public static byte[] GetSerializedBytes<T>(T component) where T : IFormData => component == null
            ? null
            : MessagePackSerializer.Serialize(
                GetSerializedData(component),
                UnityHackResolver.Instance
            );

        public static void DeserializeAndApply<T>(T component, byte[] bytes, int version) where T : IFormData
        {
            if (component == null)
            {
                HandleError("Failed to deserialize: component invalid");
                return;
            }

            if (bytes == null)
            {
                HandleError("Failed to deserialize: empty data");
                return;
            }

            switch (version)
            {
                case 0:
                    SerializeHelper.ComponentDeserializeV1(component, bytes);
                    break;
                default:
                    SerializeHelper.ComponentDeserializeV2(component, bytes);
                    break;
            }
        }

        public static bool TryCastMember<T>(this MemberInfo memberInfo, object reference, out T chaRef) where T : class
        {
            chaRef = default(T);

            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    chaRef = fieldInfo.GetValue(reference) as T;
                    break;
                case PropertyInfo propertyInfo:
                    chaRef = propertyInfo.GetValue(reference) as T;
                    break;
                default:
                    return false;
            }

            return (chaRef != null);
        }

        public static bool TryGetMemberValue(MemberInfo memberInfo, object instance, out object value)
        {
            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    value = propertyInfo.GetValue(instance);
                    return true;
                case FieldInfo fieldInfo:
                    value = fieldInfo.GetValue(instance);
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        public static void TrySetMemberValue(MemberInfo memberInfo, object instance, object value)
        {
            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(instance, value);
                    return;
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(instance, value);
                    return;
                default:
                    return;
            }
        }
    }
}
