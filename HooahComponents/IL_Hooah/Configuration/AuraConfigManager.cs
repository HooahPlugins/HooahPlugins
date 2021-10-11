using System.Linq;
#if AI || HS2
using System.Collections;
using Aura2API;
using HooahUtility.Model;
using HooahUtility.Model.Attribute;
using MessagePack;
using Studio;
using UnityEngine;

#endif

namespace HooahComponents.Configuration
{
#if AI || HS2
    [MessagePackObject]
    public class AuraConfig : IFormData
    {
        [FieldName("Use Density"), Key("bBDen")]
        public bool UseDensity = true;

        [Range(0, 1), FieldName("Base Density"), Key("bFDen")]
        public float BaseDensity = 0.001f;

        [FieldName("Use Scattering"), Key("bBSca")]
        public bool UseScattering = true;

        [Range(0, 1), FieldName("Base Scattering"), Key("bFSca")]
        public float BaseScattering = 0.5f;

        [FieldName("Use Ambient Lighting"), Key("bBAli")]
        public bool UseAmbientLighting = true;

        [Range(0, 15), FieldName("Ambient Lighting Intensity"), Key("bFAli")]
        public float BaseAmbientLighting = 1f;
    }

    public class AuraConfigManager : ConfigManager<AuraConfigManager, AuraConfig>
    {
        public AuraCamera AuraCamera;
        public AuraVolume AuraVolume;
        public FrustumSettings Settings;

        public void UpdateConfig()
        {
#if DEBUG
            if (Settings == null) HooahPlugin.HooahLogger.LogError("No setting found");
            if (Config == null) HooahPlugin.HooahLogger.LogError("No configuration found");
#endif
            if (Settings == null || Config == null) return;
            if (Settings.baseSettings == null)
            {
                Settings.baseSettings = ScriptableObject.CreateInstance<AuraBaseSettings>();
                Settings.baseSettings.name = "Hooah Aura2 Configurations";
            }

            Settings.baseSettings.useDensity = Config.UseDensity;
            Settings.baseSettings.density = Config.BaseDensity;
            Settings.baseSettings.useScattering = Config.UseScattering;
            Settings.baseSettings.scattering = Config.BaseScattering;
            Settings.baseSettings.useAmbientLighting = Config.UseAmbientLighting;
            Settings.baseSettings.ambientLightingStrength = Config.BaseAmbientLighting;
        }

        public override void PostDeserializeConfig()
        {
            UpdateConfig();
        }

        public const string AuraBundleName = "hooh/studio/lights.unity3d";

        public static IEnumerator Initialize()
        {
            yield return new WaitUntil(() => Camera.main != null);

            var resources = AssetBundleManager.LoadAsset(AuraBundleName, "AuraResourcesCollection",
                typeof(AuraResourcesCollection));
            var collection = resources.GetAsset<AuraResourcesCollection>();

            if (collection != null)
            {
                Aura.ResourcesCollection = collection;
            }
            else
            {
#if DEBUG
                HooahPlugin.HooahLogger.LogError("Invalid asset. What the hell?");
#endif
                HooahPlugin.HooahLogger.LogMessage("Failed to Initialize Volumetric Lighting Assets");
                yield break;
            }

#if DEBUG
            HooahPlugin.HooahLogger.LogWarning("Successfully did the job");
#endif

            var cam = Camera.main;
            var auraCamera = cam.GetOrAddComponent<AuraCamera>();
            yield return new WaitUntil(() => auraCamera.frustumSettings != null);

            var volume = HooahPlugin.Instance.GetOrAddComponent<AuraVolume>();
            volume.volumeShape = new VolumeInjectionShape {shape = VolumeType.Global};
            yield return new WaitUntil(() => volume != null);

            var configManager = new AuraConfigManager
            {
                AuraCamera = auraCamera,
                AuraVolume = volume,
                Settings = auraCamera.frustumSettings
            };
            configManager.UpdateConfig();
            RegisterLightItems();
        }

        public static void RegisterLightItems()
        {
            //lazy
            var names = new[]
            {
                "Volumetric All Dir", "Volumetric All Point", "Volumetric All Spot", "Volumetric Map Dir",
                "Volumetric Map Point", "Volumetric Map Spot", "Volumetric Char Dir", "Volumetric Char Point",
                "Volumetric Char Spot",
            };
            var lights = new[]
            {
                "volumetric_all_dir", "volumetric_all_point", "volumetric_all_spot", "volumetric_map_dir",
                "volumetric_map_point", "volumetric_map_spot", "volumetric_char_dir", "volumetric_char_point",
                "volumetric_char_spot",
            };
            var targets = new[]
            {
                Info.LightLoadInfo.Target.All, Info.LightLoadInfo.Target.All, Info.LightLoadInfo.Target.All,
                Info.LightLoadInfo.Target.Map, Info.LightLoadInfo.Target.Map, Info.LightLoadInfo.Target.Map,
                Info.LightLoadInfo.Target.Chara, Info.LightLoadInfo.Target.Chara, Info.LightLoadInfo.Target.Chara,
            };


            var index = 10;
            for (var i = 0; i < lights.Length; i++)
            {
                // Register Aura Lights 
                Info.Instance.dicLightLoadInfo.Add(index, new Info.LightLoadInfo
                {
                    bundlePath = AuraBundleName,
                    fileName = lights[i],
                    manifest = "abdata",
                    name = names[i],
                    no = index,
                    target = targets[i]
                });
                index++;
            }
        }
    }
#endif
}