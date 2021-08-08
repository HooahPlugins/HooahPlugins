using BepInEx;
using BepInEx.Configuration;
using BepInEx.Harmony;
using BepInEx.Logging;
using FuzzySharp;
using HarmonyLib;

namespace HooahLaunch
{
    [BepInPlugin(Constant.GUID, Constant.NAME, Constant.VERSION)]
    public class HooahLaunchPlugin : BaseUnityPlugin
    {
        private static ManualLogSource _logger;

        private void Start()
        {
            _logger = Logger;
            Harmony.CreateAndPatchAll(typeof(HooahLaunchPlugin));
            StartCoroutine(StudioDatabase.CreateDatabase());
        }
    }
}
