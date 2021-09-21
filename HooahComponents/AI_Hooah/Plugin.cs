using System.Collections;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HooahComponents.Configuration;
using HooahComponents.Hooks;
using HooahComponents.Utility;
using KKAPI;
using KKAPI.Chara;
using KKAPI.Studio.SaveLoad;
using UnityEngine;

namespace HooahComponents
{
    [BepInDependency("com.hooh.hooah.utility")]
    [BepInPlugin(PluginConstant.GUID, PluginConstant.NAME, PluginConstant.VERSION)]
    public class HooahPlugin : BaseUnityPlugin
    {
        public static HooahPlugin Instance { get; private set; }

        private IEnumerator Start()
        {
            HooahLogger = Logger;
            Instance = this;
            Harmony.CreateAndPatchAll(typeof(Hooks.Hooks));
            Harmony.CreateAndPatchAll(typeof(Hooks.IK));
            SkinnedAccessoryHook.Logger = Logger;
            SkinnedAccessoryHook.RegisterHook();

            if (KoikatuAPI.GetCurrentGameMode() != GameMode.Studio) yield break;
            yield return AuraConfigManager.Initialize();
            yield return UIIntegration.InitializeCoroutine();
            yield return new WaitUntil(() => UIIntegration.Loaded);
            StudioSaveLoadApi.RegisterExtraBehaviour<Serialization.Controller>(PluginConstant.GUID);
            CharacterApi.RegisterExtraBehaviour<HooahCharacterController>(PluginConstant.GUID);
        }

        public static ManualLogSource HooahLogger { get; private set; }
    }
}