using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using AIChara;
using KKABMX.Core;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace HooahRandMutation
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
        private readonly MakerSlider BodySlider;
        private readonly MakerSlider BodyHeadSlider;
        private readonly MakerSlider BreastSlider;
        private readonly MakerSlider TorsoSlider;
        private readonly MakerSlider PelvisSlider;
        private readonly MakerSlider ArmSlider;
        private readonly MakerSlider LegSlider;

        private float CheekSliderValue => CheekSlider.Value;
        private float ChinSliderValue => ChinSlider.Value;
        private float EarSliderValue => EarSlider.Value;
        private float EyesSliderValue => EyesSlider.Value;
        private float EyesAngleSliderValue => EyesAngleSlider.Value;
        private float NoseSliderValue => NoseSlider.Value;
        private float HeadSliderValue => HeadSlider.Value;
        private float MouthSliderValue => MouthSlider.Value;

        private float BodySliderValue => BodySlider.Value;
        private float BodyHeadSliderValue => BodyHeadSlider.Value;
        private float BreastSliderValue => BreastSlider.Value;
        private float TorsoSliderValue => TorsoSlider.Value;
        private float PelvisSliderValue => PelvisSlider.Value;
        private float ArmSliderValue => ArmSlider.Value;
        private float LegSliderValue => LegSlider.Value;

        public void RandomizeBodySliders()
        {
            var ctl = MakerChaControl.GetComponent<BoneController>();
            ctl.enabled = false;
            CharacterData.Templates.FirstOrDefault().RandomizeBodySliders(
                MakerChaControl,
                BodySliderValue,
                BodyHeadSliderValue,
                BreastSliderValue,
                TorsoSliderValue,
                PelvisSliderValue,
                ArmSliderValue,
                LegSliderValue
            );
            ctl.enabled = true;
        }

        public void RandomizeHeadSliders()
        {
            var ctl = MakerChaControl.GetComponent<BoneController>();
            ctl.enabled = false;
            CharacterData.Templates.FirstOrDefault().RandomizeHeadSliders(
                MakerChaControl,
                HeadSliderValue,
                ChinSliderValue,
                CheekSliderValue,
                EyesSliderValue,
                EyesAngleSliderValue,
                NoseSliderValue,
                MouthSliderValue,
                EarSliderValue
            );
            ctl.enabled = true;
        }

        public void InterpolateHeadSliders(ChaControl chara, float min, float max, float median, float range,
            bool stopUpdate = false)
        {
            chara.InterpolateFaceSliders(min, max, median, range);

            if (!stopUpdate) chara.AltFaceUpdate();
            // for now, there is only two point blending
        }

        public void InterpolateHeadSlidersWithFactor(ChaControl chara, float min, float max, float median, float range,
            float factor,
            bool stopUpdate = false)
        {
            chara.InterpolateFaceSliders(min, max, median, range, true, factor);

            if (!stopUpdate) chara.AltFaceUpdate();
            // for now, there is only two point blending
        }

        public FaceSliderValues(in RegisterSubCategoriesEvent e, in HooahRandMutationPlugin targetInstance,
            in MakerCategory makerCategory, bool noSlider = false) : base(e, targetInstance)
        {
            Category = makerCategory;

            if (noSlider) return;
            e.AddControl(new MakerText("Body Randomizer Ranges", Category, targetInstance));
            BodySlider = AddSlider("Body Deviation");
            BodyHeadSlider = AddSlider("Head Deviation");
            BreastSlider = AddSlider("Breast Deviation");
            TorsoSlider = AddSlider("Torso Deviation");
            PelvisSlider = AddSlider("Pelvis Deviation");
            ArmSlider = AddSlider("Arms Deviation");
            LegSlider = AddSlider("Legs Deviation");


            e.AddControl(new MakerSeparator(Category, targetInstance));
            e.AddControl(new MakerText("Face Randomizer Ranges", Category, targetInstance));
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
        private readonly MakerSlider AbmxMultiplier;

        private float AbmxPositionSliderValue => AbmxPositionSlider.Value * AbmxMutiplierValue;
        private float AbmxAngleSliderValue => AbmxAngleSlider.Value * AbmxMutiplierValue;
        private float AbmxScaleSliderValue => AbmxScaleSlider.Value * AbmxMutiplierValue;
        private float AbmxLengthSliderValue => AbmxLengthSlider.Value * AbmxMutiplierValue;
        private bool AbmxUseAbsolute => AbmxAbsoluteScale.Value;

        private float AbmxMutiplierValue => AbmxMultiplier.Value;

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
            // this is for more precise randomizer control for abmx.
            AbmxMultiplier = AddSlider("Randomize Multiplier", 0.01f, 2, 1f);
            AbmxAbsoluteScale = AddToggle("Use Absolute instead of Relative");
        }

        public void RandomizeAbmxSliders(HashSet<string> filters, bool justAbmx, bool inverted = false)
        {
            CharacterData.Templates.FirstOrDefault().RandomizeAbmx(
                MakerChaControl,
                AbmxPositionSliderValue, AbmxAngleSliderValue,
                AbmxScaleSliderValue, AbmxLengthSliderValue, AbmxUseAbsolute,
                filters,
                inverted,
                justAbmx
            );
        }
    }
}
