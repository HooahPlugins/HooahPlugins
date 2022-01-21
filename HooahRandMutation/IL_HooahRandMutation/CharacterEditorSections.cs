using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using AIChara;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace HooahRandMutation.IL_HooahRandMutation
{
    public abstract class EditorSubSection
    {
        public MakerCategory Category;
        public HooahRandMutationPlugin TargetInstance;
        public RegisterSubCategoriesEvent Event;

        public ChaControl MakerChaControl => MakerAPI.GetCharacterControl();

        protected void AddButton(string title, UnityAction onClick)
        {
            var btn = new MakerButton(title, Category, TargetInstance);
            btn.OnClick.AddListener(onClick);
            Event.AddControl(btn);
        }

        protected MakerSlider AddSlider(string title, float min = 0f, float max = 1f, float defaultValue = 0f)
        {
            return Event.AddControl(new MakerSlider(Category, title, min, max, defaultValue, TargetInstance));
        }

        protected MakerToggle AddToggle(string title, bool defaultValue = false)
        {
            return Event.AddControl(new MakerToggle(Category, title, defaultValue, TargetInstance));
        }

        public EditorSubSection(RegisterSubCategoriesEvent e, HooahRandMutationPlugin targetInstance)
        {
            Event = e;
            TargetInstance = targetInstance;
        }
    }

    public class FaceSliderValues : EditorSubSection
    {
        private readonly MakerSlider CheekSlider;
        private readonly MakerSlider ChinSlider;
        private readonly MakerSlider EarSlider;
        private readonly MakerSlider EyesSlider;
        private readonly MakerSlider EyesAngleSlider;
        private readonly MakerSlider NoseSlider;
        private readonly MakerSlider HeadSlider;
        private readonly MakerSlider MouthSlider;

        private float CheekSliderValue => CheekSlider.Value;
        private float ChinSliderValue => ChinSlider.Value;
        private float EarSliderValue => EarSlider.Value;
        private float EyesSliderValue => EyesSlider.Value;
        private float EyesAngleSliderValue => EyesAngleSlider.Value;
        private float NoseSliderValue => NoseSlider.Value;
        private float HeadSliderValue => HeadSlider.Value;
        private float MouthSliderValue => MouthSlider.Value;

        public void RandomizeHeadSliders()
        {
            var chara = MakerAPI.GetCharacterControl();

            chara.MutateRangeCombined(
                HeadSliderValue,
                ChinSliderValue,
                CheekSliderValue,
                EyesSliderValue,
                EyesAngleSliderValue,
                NoseSliderValue,
                MouthSliderValue,
                EarSliderValue
            );

            chara.UpdateShapeFaceValueFromCustomInfo();
        }

        public void InterpolateHeadSliders(float min, float max, float median, float range, bool stopUpdate = false)
        {
            var chara = MakerAPI.GetCharacterControl();

            chara.InterpolateTwoSliders(min, max, median, range);

            if (!stopUpdate) chara.UpdateShapeFaceValueFromCustomInfo();
            // for now, there is only two point blending
        }

        public void InterpolateHeadSlidersWithFactor(float min, float max, float median, float range, float factor,
            bool stopUpdate = false)
        {
            var chara = MakerAPI.GetCharacterControl();

            chara.InterpolateTwoSliders(min, max, median, range, true, factor);

            if (!stopUpdate) chara.UpdateShapeFaceValueFromCustomInfo();
            // for now, there is only two point blending
        }

        public FaceSliderValues(in RegisterSubCategoriesEvent e, in HooahRandMutationPlugin targetInstance,
            in MakerCategory makerCategory, bool noSlider = false) : base(e, targetInstance)
        {
            Category = makerCategory;

            if (noSlider) return;
            CheekSlider = AddSlider("Cheek Deviation");
            ChinSlider = AddSlider("Chin Deviation");
            EarSlider = AddSlider("Ear Deviation");
            EyesSlider = AddSlider("Eyes Deviation");
            EyesAngleSlider = AddSlider("EyesAngle Deviation");
            NoseSlider = AddSlider("Nose Deviation");
            HeadSlider = AddSlider("Head Deviation");
            MouthSlider = AddSlider("Mouth Deviation");
        }
    }

    public class ABMXSliderValues : EditorSubSection
    {
        private readonly MakerSlider CheekSlider;
        private readonly MakerSlider AbmxPositionSlider;
        private readonly MakerSlider AbmxAngleSlider;
        private readonly MakerSlider AbmxScaleSlider;
        private readonly MakerSlider AbmxLengthSlider;
        private readonly MakerToggle AbmxAbsoluteScale;

        private float AbmxPositionSliderValue => AbmxPositionSlider.Value;
        private float AbmxAngleSliderValue => AbmxAngleSlider.Value;
        private float AbmxScaleSliderValue => AbmxScaleSlider.Value;
        private float AbmxLengthSliderValue => AbmxLengthSlider.Value;
        private bool AbmxUseAbsolute => AbmxAbsoluteScale.Value;

        public ABMXSliderValues(in RegisterSubCategoriesEvent e, in HooahRandMutationPlugin targetInstance,
            in MakerCategory category, bool noSlider = false) : base(e,
            targetInstance)

        {
            Category = category;
            if (noSlider) return;
            AbmxPositionSlider = AddSlider("Abmx Position Deviation");
            AbmxAngleSlider = AddSlider("Abmx Angle Deviation");
            AbmxScaleSlider = AddSlider("Abmx Scale Deviation");
            AbmxLengthSlider = AddSlider("Abmx Length Deviation");
            AbmxAbsoluteScale = AddToggle("Use Absolute Scale");
        }

        public void RandomizeAbmxSliders(HashSet<string> filters)
        {
            MakerAPI.GetCharacterControl().RandomizeABMX(AbmxPositionSliderValue, AbmxAngleSliderValue,
                AbmxScaleSliderValue, AbmxLengthSliderValue, AbmxUseAbsolute, filters);
        }

        enum ABMXValueType
        {
            Position,
            Angle,
            Scale,
        }

        private static Vector3 GetSaneValue(in InterpolateShapeUtility.ABMXValues value, ABMXValueType type,
            Vector3 defaultVector)
        {
            if (value == null) return defaultVector;
            switch (type)
            {
                case ABMXValueType.Position:
                    return value.Position;
                case ABMXValueType.Angle:
                    return value.VectorAngle;
                case ABMXValueType.Scale:
                    return value.Scale;
            }

            return defaultVector;
        }

        private static Vector3 LerpValue(IReadOnlyList<InterpolateShapeUtility.ABMXValues> values, ABMXValueType type,
            float lerpFactor)
        {
            var defaultVector = type == ABMXValueType.Scale ? Vector3.one : Vector3.zero;
            if (values == null) return defaultVector;
            if (values.Count < 2) return defaultVector;
            var first = values[0];
            var second = values[1];
            if (first == null && second == null) return defaultVector;

            return Vector3.Lerp(
                GetSaneValue(first, type, defaultVector),
                GetSaneValue(second, type, defaultVector),
                lerpFactor
            );
        }

        private static float LerpValue(IReadOnlyList<InterpolateShapeUtility.ABMXValues> values, float lerpFactor)
        {
            if (values == null) return 1;
            if (values.Count < 2) return 1;
            var first = values[0];
            var second = values[1];
            if (first == null && second == null) return 1;

            return Mathf.Lerp(
                first?.RelativePosition ?? 1,
                second?.RelativePosition ?? 1,
                lerpFactor
            );
        }

        private static string PickName(IReadOnlyList<InterpolateShapeUtility.ABMXValues> values)
        {
            if (values == null) return "";
            if (values.Count < 2) return "";
            var first = values[0];
            var second = values[1];
            if (first == null && second == null) return "";
            return first == null || first.Name.IsNullOrWhiteSpace() ? second.Name : first.Name;
        }

        private static float GetRandomNumber(float min, float max, float median, float range)
        {
            var randomValue = median + Random.Range(-range, range);
            return Mathf.Max(min, Mathf.Min(randomValue, max));
        }

        // todo: filter is not working yet.
        public void InterpolateAbmxSliders(int defaultIndex, HashSet<string> filters,
            float min, float max, float median, float range,
            bool uniformFactor = false, float factor = 0.5f)
        {
            var le = InterpolateShapeUtility.Templates.Length;
            var dictMaps = new Dictionary<string, InterpolateShapeUtility.ABMXValues[]>();
            var processedMaps = new Dictionary<string, InterpolateShapeUtility.ABMXValues>();

            // assign all references just in case ...
            // for when you want to mix more than 2 characters.
            for (var i = 0; i < le; i++)
            {
                foreach (var kv in InterpolateShapeUtility.Templates[i].AbmxValuesMap)
                {
                    if (dictMaps.TryGetValue(kv.Key, out var arr)) arr[i] = kv.Value;
                    else
                    {
                        dictMaps[kv.Key] = new InterpolateShapeUtility.ABMXValues[le];
                        dictMaps[kv.Key][i] = kv.Value;
                    }
                }
            }

            // because im lazy as fuck
            var rnd = new Func<float>(() => GetRandomNumber(min, max, median, range));
            foreach (var kv in dictMaps)
            {
                var filterTarget = filters != null && !filters.Contains(kv.Key);
                var def = kv.Value[defaultIndex];

                // todo: this can be changed later to support multi point lerp
                // uniformFactor = use same lerp factor for all values.
                //          else = random factor for all
                processedMaps[kv.Key] = new InterpolateShapeUtility.ABMXValues
                {
                    Name = PickName(kv.Value),
                    Position = filterTarget
                        ? def?.Position ?? Vector3.zero
                        : LerpValue(kv.Value, ABMXValueType.Position, uniformFactor ? factor : rnd()),
                    Scale = filterTarget
                        ? def?.Scale ?? Vector3.one
                        : LerpValue(kv.Value, ABMXValueType.Scale, uniformFactor ? factor : rnd()),
                    VectorAngle = filterTarget
                        ? def?.VectorAngle ?? Vector3.one
                        : LerpValue(kv.Value, ABMXValueType.Angle, uniformFactor ? factor : rnd()),
                    RelativePosition = filterTarget
                        ? def?.RelativePosition ?? 1
                        : LerpValue(kv.Value, uniformFactor ? factor : rnd())
                };
            }

            MakerChaControl.ApplyAbmxValues(processedMaps);
        }
    }
}
