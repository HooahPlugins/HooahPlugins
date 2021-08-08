using System.Linq;
using AdvancedStudioUI;
using HooahUtility.Controller.Components;
using HooahUtility.Utility;
using UnityEngine;
#if AI || HS2
using Studio;

#endif

namespace HooahComponents.Utility
{
    public static class StudioMacros
    {
#if AI || HS2
        public static void RandomizeRotation(bool add, bool xAxis, bool yAxis, bool zAxis)
        {
            var manager = GuideObjectManager.Instance;
            if (manager.selectObjects == null || manager.selectObjects.Length <= 0) return;

            foreach (var managerSelectObject in manager.selectObjects)
            {
                if (add) managerSelectObject.changeAmount.rot += RandVectorAxis(xAxis, yAxis, zAxis);
                else managerSelectObject.changeAmount.rot = RandVectorAxis(xAxis, yAxis, zAxis);
            }
        }

        public static void IterateFkAxis(bool add, bool xAxis, bool yAxis, bool zAxis)
        {
            var manager = GuideObjectManager.Instance;
            if (manager.selectObjects == null || manager.selectObjects.Length <= 0) return;
            var fkControls = Studio.Studio.GetSelectObjectCtrl()
                .OfType<OCIItem>()
                .Where(x => x.isFK && x.itemInfo.enableFK)
                .SelectMany(ObjectControlInfoUtility.GetChangeAmounts);

            foreach (var changeAmount in fkControls)
            {
                if (add) changeAmount.rot += RandVectorAxis(xAxis, yAxis, zAxis);
                else changeAmount.rot = RandVectorAxis(xAxis, yAxis, zAxis);
            }
        }

        public static void IterateRandomColor(bool r, bool g, bool b, bool a)
        {
            foreach (var ociItem in Studio.Studio.GetSelectObjectCtrl().OfType<OCIItem>())
            {
                for (var i = 0; i < ociItem.useColor.Length; i++)
                {
                    if (ociItem.useColor[i])
                        ociItem.SetColor(new Color(
                            r ? Random.value : 0f,
                            g ? Random.value : 0f,
                            b ? Random.value : 0f
                        ), i);
                }

                ociItem.SetAlpha(a ? Random.value : 1f);
                ociItem.UpdateColor();
            }
        }

        [Range(0, 360f)] public static float RandomizeRange = 360f;

        private static float Rand => Random.Range(-RandomizeRange, RandomizeRange);
        private static Vector3 RandVector => new Vector3(Rand, Rand, Rand);

        private static Vector3 RandVectorAxis(bool x, bool y, bool z) =>
            new Vector3(x ? Rand : 0, y ? Rand : 0, z ? Rand : 0);

        public static void InitializeMacroTab(StudioItemControl self, TabbedContentControl content)
        {
            var form = self.CreateTab("macro", "Macros");
            form.AddField<SliderComponent>("slider", typeof(StudioMacros), "RandomizeRange");
            form.AddOptionButton(
                "Object",
                new[] {"+", "X", "Y", "Z"},
                options => RandomizeRotation(options[0], options[1], options[2], options[3]));
            form.AddOptionButton(
                "FK",
                new[] {"+", "X", "Y", "Z"},
                options => IterateFkAxis(options[0], options[1], options[2], options[3]));
            form.AddOptionButton(
                "Color",
                new[] {"R", "G", "B", "A"},
                options => IterateRandomColor(options[0], options[1], options[2], options[3]));
        }
#endif
    }
}