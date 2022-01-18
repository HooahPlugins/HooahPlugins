using System.Collections.Generic;
using AIChara;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UnityEngine.Events;

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

        public void InterpolateHeadSliders()
        {
            var chara = MakerAPI.GetCharacterControl();

            chara.InterpolateTwoSliders(
                HeadSliderValue,
                ChinSliderValue,
                CheekSliderValue,
                EyesSliderValue,
                EyesAngleSliderValue,
                NoseSliderValue,
                MouthSliderValue,
                EarSliderValue,
                true,
                UnityEngine.Random.Range(0, 1f)
            );

            chara.UpdateShapeFaceValueFromCustomInfo();
            // for now, there is only two point blending
        }

        public void InterpolateHeadSlidersWithFactor(float factor)
        {
            var chara = MakerAPI.GetCharacterControl();

            chara.InterpolateTwoSliders(
                HeadSliderValue,
                ChinSliderValue,
                CheekSliderValue,
                EyesSliderValue,
                EyesAngleSliderValue,
                NoseSliderValue,
                MouthSliderValue,
                EarSliderValue,
                true,
                factor
            );

            chara.UpdateShapeFaceValueFromCustomInfo();
            // for now, there is only two point blending
        }

        public FaceSliderValues(in RegisterSubCategoriesEvent e, in HooahRandMutationPlugin targetInstance,
            in MakerCategory makerCategory) : base(e, targetInstance)
        {
            Category = makerCategory;

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
            in MakerCategory category) : base(e,
            targetInstance)

        {
            Category = category;
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
    }
}
