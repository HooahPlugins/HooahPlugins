using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AIChara;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HooahSmugFace.IL_HooahSmugFace;
using HooahSmugFace.IL_HooahSmugFace.Data;
using UnityEngine;

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
