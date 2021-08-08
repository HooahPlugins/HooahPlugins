using BepInEx;
using BepInEx.Configuration;
using BepInEx.Harmony;
using BepInEx.Logging;
using HarmonyLib;
using HooahSmugFace.IL_HooahSmugFace.Data;

namespace HooahSmugFace
{
    [BepInPlugin(Constant.GUID, Constant.NAME, Constant.VERSION)]
    public class HooahSmugFacePlugin : BaseUnityPlugin
    {
        public static ManualLogSource _logger;

        private void Start()
        {
            _logger = Logger;
            FaceData.Load();
        }
    }
}
