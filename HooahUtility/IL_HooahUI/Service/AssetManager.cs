using HooahUtility.Editor;
using UnityEngine;
using Object = UnityEngine.Object;
#if AI || HS2
using KKAPI.Utilities;
using System.Reflection;

#else
using System.IO;
#endif

namespace HooahUtility.Service
{
    public static class AssetManager
    {
        private const string PathUIAsset = "uiasset.assets";
        private const string UIAssetName = "HooahFormPrefabs";

        public static bool TryGetUIPrefab(string key, out GameObject asset)
        {
            asset = null;
            return HooahFormPrefabs != null && HooahFormPrefabs.TryGetForm(key, out asset);
        }

        public static bool TryMakeUIPrefab(string key, out GameObject instance, Transform parent = null)
        {
            instance = null;
            if (!TryGetUIPrefab(key, out var asset)) return false;
            instance = Object.Instantiate(asset, parent);
            return true;
        }

        private static AssetBundle _bundle;

        public static AssetBundle UIAssetBundle
        {
            get
            {
                if (!ReferenceEquals(null, _bundle)) return _bundle;
#if AI || HS2
                _bundle = AssetBundle.LoadFromMemory(
                    ResourceUtils.GetEmbeddedResource(PathUIAsset, Assembly.GetAssembly(typeof(AssetManager)))
                );
#else
                _bundle = AssetBundle.LoadFromFile(
                    Path.Combine(Application.dataPath, "AdvancedStudioUI/CompileResource", PathUIAsset)
                );
#endif
                return _bundle;
            }
        }

        private static FormPrefabs _prefabs;

        public static FormPrefabs HooahFormPrefabs
        {
            get
            {
                if (!ReferenceEquals(null, _prefabs)) return _prefabs;
                _prefabs = UIAssetBundle.LoadAsset<FormPrefabs>(UIAssetName);
                return _prefabs;
            }
        }

#if AI || HS2
        public static bool LoadAssetBundles()
        {
            _bundle = AssetBundle.LoadFromMemory(
                ResourceUtils.GetEmbeddedResource(PathUIAsset, Assembly.GetAssembly(typeof(AssetManager)))
            );
            return !ReferenceEquals(null, _bundle);
        }
#endif
    }
}