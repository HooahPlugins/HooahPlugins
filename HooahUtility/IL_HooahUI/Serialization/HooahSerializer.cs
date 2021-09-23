// Decompiled with JetBrains decompiler
// Type: HooahUtility.Serialization.HooahSerializer
// Assembly: HooahUtilityEditorCompatible, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 52B179E7-4743-4027-9930-3D3BFEF61A6C
// Assembly location: D:\himates\AIHSModding\Assets\Plugins\Hooah\HooahUtilityEditorCompatible.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HooahUtility.Serialization.Attributes;
using HooahUtility.Serialization.Formatter;
using MessagePack;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HooahUtility.Serialization
{
    public class HooahSerializer : MonoBehaviour, ISerializationCallbackReceiver
    {
        private const int HooahRevision = 1;
        [SerializeField] [HideInInspector] private byte[] serializedData;
        [SerializeField] [HideInInspector] private List<int> uObjectKey = new List<int>();
        [SerializeField] [HideInInspector] private List<Object> uObjectValue = new List<Object>();
        [SerializeField] [HideInInspector] private int serializeRevision = 1;
        private static MethodInfo _serialize;
        private static MethodInfo _deserialize;
        private static Dictionary<Type, MethodInfo> _serializeGenericCache = new Dictionary<Type, MethodInfo>();
        private static Dictionary<Type, MethodInfo> _deserializeGenericCache = new Dictionary<Type, MethodInfo>();

        public bool TryGetReference(int instanceID, out Object reference)
        {
            reference = null;
            if (instanceID < 0)
                return false;
            var index = uObjectKey.IndexOf(instanceID);
            if (index < 0 || index > uObjectValue.Count)
                return false;
            reference = uObjectValue[index];
            return true;
        }

        public int AddReference(Object o)
        {
            var instanceId = o.GetInstanceID();
            uObjectKey.Add(instanceId);
            uObjectValue.Add(o);
            return instanceId;
        }

        static HooahSerializer()
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

        private static MethodInfo GetGenericSerialize(Type t)
        {
            MethodInfo methodInfo1;
            if (_serializeGenericCache.TryGetValue(t, out methodInfo1))
                return methodInfo1;
            var methodInfo2 = _serialize.MakeGenericMethod(t);
            _serializeGenericCache[t] = methodInfo2;
            return methodInfo2;
        }

        private static MethodInfo GetGenericDeserialize(Type t)
        {
            MethodInfo methodInfo1;
            if (_deserializeGenericCache.TryGetValue(t, out methodInfo1))
                return methodInfo1;
            var methodInfo2 = _deserialize.MakeGenericMethod(t);
            _deserializeGenericCache[t] = methodInfo2;
            return methodInfo2;
        }

        private static byte[] SerializeType(FieldInfo fieldInfo, object instance) =>
            GetGenericSerialize(fieldInfo.FieldType)
                .Invoke(null, new object[2]
                {
                    fieldInfo.GetValue(instance),
                    UnityHackResolver.Instance
                }) as byte[];

        private IEnumerable<FieldInfo> GetTypeFields() =>
            GetType().GetFields().Where(
                x => x.GetCustomAttribute<HooahSerializeAttribute>() != null);

        private void Serialize()
        {
            uObjectKey.Clear();
            uObjectValue.Clear();
            UnityObjectFormatter.Context = this;
            try
            {
                serializedData = MessagePackSerializer.Serialize(GetTypeFields()
                    .ToDictionary(x => x.Name,
                        fieldInfo =>
                            SerializeType(fieldInfo, this)));
            }
            finally
            {
                UnityObjectFormatter.Context = null;
            }
        }

        public void OnBeforeSerialize() => Serialize();

        public void DeserializeData()
        {
            if (serializedData == null || serializedData.Length == 0)
                return;
            var dictionary =
                MessagePackSerializer.Deserialize<Dictionary<string, byte[]>>(serializedData,
                    UnityHackResolver.Instance);
            if (dictionary == null)
                return;
            UnityObjectFormatter.Context = this;
            try
            {
                foreach (var typeField in GetTypeFields())
                {
                    byte[] numArray;
                    if (!dictionary.TryGetValue(typeField.Name, out numArray)) continue;
                    var obj = GetGenericDeserialize(typeField.FieldType).Invoke(null,
                        new object[]
                        {
                            numArray,
                            UnityHackResolver.Instance
                        });
                    typeField.SetValue(this, obj);
                }
            }
            finally
            {
                UnityObjectFormatter.Context = null;
            }
        }

        public void OnAfterDeserialize() => DeserializeData();
    }
}