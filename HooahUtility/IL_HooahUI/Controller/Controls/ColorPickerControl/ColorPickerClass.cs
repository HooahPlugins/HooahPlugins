using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdvancedStudioUI
{
    public partial class ColorPickerControl
    {
        private static readonly int HSV = Shader.PropertyToID("_HSV");
        private static readonly int DoubleMode = Shader.PropertyToID("_DoubleMode");
        private static readonly int Color2 = Shader.PropertyToID("_Color2");
        private static readonly int Color1 = Shader.PropertyToID("_Color1");
        private static readonly int Mode = Shader.PropertyToID("_Mode");
        
        private const float HUE_LOOP = 5.9999f;
        
        public enum MainPickingMode
        {
            HS, HV, SH, SV, VH,
            VS
        }

        public enum PickerDirection { Vertical, Horizontal, Universal, None }

        public enum PickerType
        {
            Main, R, G, B, H,
            S, V, A, Preview, PreviewAlpha
        }
        
        public struct PickerData
        {
            public PickerType PickerType;
            public PickerDirection Direction;
            public Image SliderImage;
            public EventTrigger EventTrigger;
            public RectTransform Transform;
            public RectTransform MarkerTransform;
            public bool IsAlpha;
            public bool IsPreview;
        }
    }
}