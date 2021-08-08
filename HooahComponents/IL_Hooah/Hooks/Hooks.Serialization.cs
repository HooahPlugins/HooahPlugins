#if AI || HS2
using System;
using System.Collections.Generic;
using System.IO;
using SerializationUtility = Utility.SerializationUtility;
using ExtensibleSaveFormat;
using HooahComponents.Configuration;
using HooahUtility.Model;
using HooahUtility.Serialization;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Studio;
using Utility;
#endif

// TODO: Audit if this should be in the utilities.
namespace HooahComponents.Hooks
{
    public static class Serialization
    {
#if AI || HS2
        private const int ConfigDataVersion = 1; // config manager data version
        private const int NodeDataVersion = 1; // node data version

        public static string ConfigPath => Path.Combine(Path.GetFullPath("./UserData"), "Hooah", "config.dat");

        public class Controller : SceneCustomFunctionController
        {
            protected override void OnSceneLoad(SceneOperationKind operation,
                ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
            {
                var extendedData = GetExtendedData();
                if (extendedData == null) return;
                foreach (var keyValuePair in extendedData.data)
                {
                    if (loadedItems.TryGetValue(Convert.ToInt32(keyValuePair.Key), out var objectCtrlInfo))
                    {
                        switch (objectCtrlInfo)
                        {
                            case OCIItem ociItem:
                                SerializationUtility.DeserializeAndApply(
                                    SerializationUtility.GetSerializableComponent(ociItem.objectItem),
                                    keyValuePair.Value as byte[], extendedData.version);
                                break;
                            case OCILight ociLight:
                                SerializationUtility.DeserializeAndApply(
                                    SerializationUtility.GetSerializableComponent(ociLight.objectLight)
                                    , keyValuePair.Value as byte[], extendedData.version);
                                break;
                        }
                    }
                }
            }

            protected override void OnSceneSave()
            {
                Dictionary<string, object> bytesMap = new Dictionary<string, object>();

                foreach (var x in Studio.Studio.Instance.dicObjectCtrl)
                {
                    IFormData comp = null;

                    switch (x.Value)
                    {
                        case OCIItem ociItem:
                            comp = SerializationUtility.GetSerializableComponent(ociItem.objectItem);
                            break;
                        case OCILight ociLight:
                            comp = SerializationUtility.GetSerializableComponent(ociLight.objectLight);
                            break;
                    }

                    if (comp == null) continue;
                    var bytes = SerializationUtility.GetSerializedBytes(comp);
                    if (bytes == null) continue;
                    bytesMap[x.Key.ToString()] = bytes;
                }

                SetExtendedData(new PluginData
                {
                    data = bytesMap,
                    version = NodeDataVersion 
                });
            }
        }

        public static bool CanLoadAllConfigs()
        {
            return HooahConfigManger.Instance != null && AuraConfigManager.Instance != null;
        }

        public static void LoadAllConfigs()
        {
            if (!File.Exists(ConfigPath)) return;
            using (var b = new BinaryReader(File.OpenRead(ConfigPath)))
            {
                b.ReadInt32();
                while (b.BaseStream.Position != b.BaseStream.Length)
                {
                    var key = b.ReadString();
                    var len = b.ReadInt32();
                    var bytes = b.ReadBytes(len);
                    switch (key)
                    {
                        case "HC":
                            HooahConfigManger.Instance.DeserializeConfig(bytes);
                            break;
                        case "AU":
                            AuraConfigManager.Instance.DeserializeConfig(bytes);
                            break;
                    }
                }
            }
        }


        public static void SaveAllConfigs()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath) ?? String.Empty);
            if (File.Exists(ConfigPath)) File.Delete(ConfigPath);
            using (var b = new BinaryWriter(File.OpenWrite(ConfigPath)))
            {
                b.Write(ConfigDataVersion);

                var hooahConf = HooahConfigManger.Instance.SerializeConfig();
                b.Write("HC");
                b.Write(hooahConf.Length);
                b.Write(hooahConf);

                var auraConf = AuraConfigManager.Instance.SerializeConfig();
                b.Write("AU");
                b.Write(auraConf.Length);
                b.Write(auraConf);
            }
        }
#endif
    }
}