using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace HooahRandMutation
{
    [BepInPlugin(Constant.GUID, Constant.NAME, Constant.VERSION)]
    public class HooahRandMutationPlugin : BaseUnityPlugin
    {
        private static ManualLogSource _logger;

        private void Start()
        {
            _logger = Logger;
            Harmony.CreateAndPatchAll(typeof(HooahRandMutationPlugin));
        }
    }
}
