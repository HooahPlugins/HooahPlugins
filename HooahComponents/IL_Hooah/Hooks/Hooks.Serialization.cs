#if AI || HS2
using System;
using System.Collections.Generic;
using System.IO;
using SerializationUtility = Utility.SerializationUtility;
using ExtensibleSaveFormat;
using HooahComponents.Configuration;
using HooahUtility.Utility;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Studio;
#endif

// TODO: Make the serialization process more modular
//       Main serialization process will begin in the hooah utility.
// TODO: also why this is in the hooah? what the fuck?
namespace HooahComponents.Hooks
{
    public static class Serialization
    {
#if AI || HS2
        private const int ConfigDataVersion = 1; // config manager data version
        private const int NodeDataVersion = 1; // node data version

        // the bruh moment
        public static string ConfigPath => Path.Combine(Path.GetFullPath("./UserData"), "Hooah", "config.dat");

        public class Controller : SceneCustomFunctionController
        {
            protected override void OnObjectsCopied(ReadOnlyDictionary<int, ObjectCtrlInfo> copiedItems)
            {
                foreach (var objectCtrlInfo in copiedItems)
                    if (Studio.Studio.Instance.dicObjectCtrl.TryGetValue(objectCtrlInfo.Key, out var src))
                        StudioReferenceUtility.CopyComponentsData(src, objectCtrlInfo.Value);
            }

            protected override void OnSceneLoad(SceneOperationKind operation,
                ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
            {
                var extendedData = GetExtendedData();
                if (extendedData == null) return;
                foreach (var keyValuePair in extendedData.data)
                {
                    if (!loadedItems.TryGetValue(Convert.ToInt32(keyValuePair.Key), out var objectCtrlInfo)) continue;
                    if (!StudioReferenceUtility.TryGetOciEndNodeGameObject(objectCtrlInfo, out var target)) continue;
                    SerializationUtility.DeserializeAndApply(
                        SerializationUtility.GetSerializableComponent(target), keyValuePair.Value as byte[],
                        extendedData.version);
                }
            }

            protected override void OnSceneSave()
            {
                var bytesMap = new Dictionary<string, object>();

                foreach (var x in Studio.Studio.Instance.dicObjectCtrl)
                {
                    if (!StudioReferenceUtility.TryGetOciEndNodeGameObject(x.Value, out var refObject)) continue;
                    var comp = SerializationUtility.GetSerializableComponent(refObject);
                    if (comp == null) continue;
                    var bytes = SerializationUtility.GetSerializedBytes(comp);
                    if (bytes == null) continue;
                    bytesMap[x.Key.ToString()] = bytes;
                }

                SetExtendedData(new PluginData { data = bytesMap, version = NodeDataVersion });
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

                // todo: make the data serialization writer modular 

                #region Write Configuration Data

                var hooahConf = HooahConfigManger.Instance.SerializeConfig();
                b.Write("HC");
                b.Write(hooahConf.Length);
                b.Write(hooahConf);

                #endregion

                #region Write Aura Data

                var auraConf = AuraConfigManager.Instance.SerializeConfig();
                b.Write("AU");
                b.Write(auraConf.Length);
                b.Write(auraConf);

                #endregion
            }
        }
#endif
    }
}
