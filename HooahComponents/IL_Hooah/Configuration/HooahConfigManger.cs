#if AI || HS2
using System;
using HooahComponents.Utility;
using HooahUtility.Model;
using HooahUtility.Model.Attribute;
using HooahUtility.Service;
using MessagePack;
using UnityEngine;
#endif

namespace HooahComponents.Configuration
{
#if AI || HS2
    [MessagePackObject]
    public class HooahConfig : IFormData
    {
        [FieldName("Window Width"), Key("ww")] public float Width = 500;

        [FieldName("Window Height"), Key("wh")]
        public float Height = 400;

        [Range(0.25f, 2f), FieldName("Window Scale"), Key("ws")]
        public float CanvasScale = 1.5f;

        // todo: implement color scheme configuration (dark mode, light mode)
    }

    public class HooahConfigManger : ConfigManager<HooahConfigManger, HooahConfig>
    {
        public void UpdateConfig()
        {
            if (Config == null) return;
            Config.Width = Math.Min(Math.Max(Config.Width, 300), 2000);
            Config.Height = Math.Min(Math.Max(Config.Height, 300), 2000);
            Config.CanvasScale = Mathf.Min(Mathf.Max(Config.CanvasScale, 0.25f), 3f);

            UIIntegration.WindowTransform.sizeDelta = new Vector2(Config.Width, Config.Height);
            CanvasManager.CanvasScaler.scaleFactor = Config.CanvasScale;
        }

        public override void PostDeserializeConfig()
        {
            UpdateConfig();
        }
    }
#endif
}