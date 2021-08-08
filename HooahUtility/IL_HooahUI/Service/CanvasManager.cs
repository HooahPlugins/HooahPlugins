using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HooahUtility.Service
{
    public static class CanvasManager
    {
        public const float ScaleLevel = 1.5f;
        public const string CanvasObjectName = "HooahCanvas";

        public static GameObject CanvasGameObject { get; private set; }


#if AI || HS2
        public static Canvas Canvas { get; private set; }
#elif UNITY_EDITOR
        public static Canvas Canvas
        {
            get
            {
                return Object
                    .FindObjectsOfType<Canvas>()
                    .FirstOrDefault(x => x.transform.parent == null);
            }
        }
#else
        public static Canvas Canvas { get; private set; }
#endif
        public static CanvasScaler CanvasScaler { get; private set; }
        public static GraphicRaycaster GraphicRaycaster { get; private set; }

        public static bool InitializeCanvas()
        {
#if AI || HS2
            CanvasGameObject = new GameObject("HooahCanvas");
            var instance = Studio.Studio.Instance;
            Canvas = CanvasGameObject.AddComponent<Canvas>();
            Canvas.pixelPerfect = true;
            Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            Canvas.sortingOrder = 0;
            Canvas.targetDisplay = 0;
            Canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                                              AdditionalCanvasShaderChannels.Normal |
                                              AdditionalCanvasShaderChannels.Tangent;
#else
            // Studio instance existance must be ensured.
            CanvasGameObject = Canvas.gameObject;
#endif
            CanvasScaler = CanvasGameObject.AddComponent<CanvasScaler>();
            CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            CanvasScaler.scaleFactor = 1.4f;

            GraphicRaycaster = CanvasGameObject.AddComponent<GraphicRaycaster>();
            GraphicRaycaster.ignoreReversedGraphics = true;
            GraphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

            return true;
        }
    }
}