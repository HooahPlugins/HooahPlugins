using System;
using System.Collections.Generic;
using System.Linq;
using ExtensibleSaveFormat;
using HooahUtility.Model;
using HooahUtility.Serialization.Component;
using HooahUtility.Utility;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using MessagePack;
using Studio;
using Utility;

namespace HooahComponents.Hooks
{
    public partial class Serialization
    {
        public partial class Controller
        {
            //region [V1] Hooah Deserializer
            protected void DeserializeSceneDataV1(ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems,
                PluginData extendedData)
            {
                foreach (var keyValuePair in extendedData.data)
                {
                    if (!loadedItems.TryGetValue(Convert.ToInt32(keyValuePair.Key), out var objectCtrlInfo)) continue;
                    if (!StudioReferenceUtility.TryGetOciEndNodeGameObject(objectCtrlInfo, out var target)) continue;
                    SerializationUtility.DeserializeAndApply(
                        SerializationUtility.GetSerializableComponent(target), keyValuePair.Value as byte[],
                        1);
                }
            }
            //endregion

            //region [V2] Hooah Deserializer
            protected void DeserializeSceneDataV2(ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems,
                PluginData extendedData)
            {
                foreach (var keyValuePair in extendedData.data)
                {
                    if (!loadedItems.TryGetValue(Convert.ToInt32(keyValuePair.Key), out var objectCtrlInfo))
                        continue;
                    if (!StudioReferenceUtility.TryGetOciEndNodeGameObject(objectCtrlInfo, out var target))
                        continue;

                    //map[STD INDEX](map[COMPNAME](map[FIELD]value))
                    var bytes = keyValuePair.Value as byte[];
                    var compToFieldMap =
                        SerializationUtility.Deserialize(typeof(Dictionary<string, byte[]>), bytes) as
                            Dictionary<string, byte[]>;

                    foreach (var hooahBehavior in SerializationUtility
                                 .GetSerializableComponentsInDictionary(target))
                    {
                        if (compToFieldMap != null && compToFieldMap.TryGetValue(hooahBehavior.Key, out var bts))
                            SerializationUtility.DeserializeAndApply((IFormData)hooahBehavior.Value, bts, 2);
                    }
                }
            }

            /*
             * v2: limitation
             *     you cannot save more than two same components at one game object.
             *     each game object must have unique name
             *     map[STD INDEX](map[COMPNAME](map[FIELD]value))
             */
            protected void SerializeSceneDataV2(Dictionary<string, object> bytesMap)
            {
                foreach (var x in Studio.Studio.Instance.dicObjectCtrl)
                {
                    if (!StudioReferenceUtility.TryGetOciEndNodeGameObject(x.Value, out var refObject)) continue;
                    var comps =
                        SerializationUtility.GetSerializableComponents(refObject)
                            ?.ToDictionary(y => y.GetType().Name,
                                y => SerializationUtility.GetSerializedBytes((IFormData)y)
                            );

                    var bytes = MessagePackSerializer.Serialize(comps);
                    if (bytes == null) continue;
                    bytesMap[x.Key.ToString()] = bytes;
                }
            }
            //endregion
        }
    }
}
