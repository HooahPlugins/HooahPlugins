using System;
using System.Collections.Generic;
using System.Linq;
using KKABMX.Core;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UniRx;

namespace HooahRandMutation
{
    // todo: this is a mess!
    public class CharacterInterpolateSection : EditorSubSection
    {
        protected readonly string DisplayName = "CharacterInterpolation";
        protected readonly string SubCategoryName = "CharacterInterpolation";

        protected readonly ABMXSliderValues AbmxSliderValues;
        protected readonly FaceSliderValues SliderValues;

        public bool IsUpdating = false;

        private readonly MakerSlider maxSlider;
        private readonly MakerSlider medianSlider;
        private readonly MakerSlider rangeSlider;
        private readonly MakerSlider mixSlider;
        private readonly MakerSlider minSlider;
        private readonly MakerToggle defaultSide;
        private readonly MakerToggle fixWarp;
        private readonly MakerToggle updateTick;
        private readonly MakerDropdown mixType;

        private int mixTypeValue => mixType.Value;
        private float maxSliderValue => maxSlider.Value;
        private float medianSliderValue => medianSlider.Value;
        private float rangeSliderValue => rangeSlider.Value;
        private float mixSliderValue => mixSlider.Value;
        private float minSliderValue => minSlider.Value;
        private int defaultSideValue => defaultSide.Value ? 0 : 1;
        private bool fixWarpValue => fixWarp.Value;


        public void InterpolateHead(bool abmx, bool useFactor = false)
        {
            if (useFactor)
                SliderValues.InterpolateHeadSlidersWithFactor(MakerChaControl, minSliderValue,
                    maxSliderValue, medianSliderValue, rangeSliderValue, mixSliderValue, abmx);
            else
                SliderValues.InterpolateHeadSliders(MakerChaControl, minSliderValue, maxSliderValue,
                    medianSliderValue, rangeSliderValue, abmx);
        }

        public void InterpolateBody(bool abmx, bool useFactor = false)
        {
            if (useFactor)
                SliderValues.InterpolateBodySlidersWithFactor(MakerChaControl, minSliderValue,
                    maxSliderValue, medianSliderValue, rangeSliderValue, mixSliderValue, abmx);
            else
                SliderValues.InterpolateBodySliders(MakerChaControl, minSliderValue, maxSliderValue,
                    medianSliderValue, rangeSliderValue, abmx);
        }

        public void InterpolateAbmx(HashSet<string> filters, bool sliders, bool abmx, bool useFactor = false,
            bool inverted = false)
        {
            if (!abmx) return;
            try
            {
                MakerChaControl.InterpolateAbmx(defaultSideValue, filters,
                    minSliderValue, maxSliderValue, medianSliderValue, rangeSliderValue,
                    useFactor, mixSliderValue,
                    inverted, fixWarpValue, !sliders);
            }
            catch (Exception e)
            {
                HooahRandMutationPlugin.instance.loggerInstance.LogError(e);
            }
            finally
            {
                Observable.NextFrame(FrameCountType.EndOfFrame).Take(1)
                    .Subscribe(__ =>
                    {
                        IsUpdating = false;
                        if (!sliders) return;
                        MakerChaControl.AltFaceUpdate();
                        MakerChaControl.AltBodyUpdate();
                    });
            }
        }

        public void InterpolateHeadAndBody(bool sliders, bool abmx, bool useFactor = false)
        {
            var ctl = MakerChaControl.GetComponent<BoneController>();
            try
            {
                if (!sliders) return;
                ctl.enabled = false;
                if (mixTypeValue == 0 || mixTypeValue == 2) InterpolateHead(abmx, useFactor);
                if (mixTypeValue == 1 || mixTypeValue == 2) InterpolateBody(abmx, useFactor);
                ctl.enabled = true;
            }
            catch (Exception e)
            {
                HooahRandMutationPlugin.instance.loggerInstance.LogError(e);
            }
            finally
            {
                if (!abmx) IsUpdating = false;
            }
        }

        public void UpdateCallback(HashSet<string> filters,
            bool sliders, bool abmx, bool useFactor = false, bool inverted = false)
        {
            if (IsUpdating) return;
            IsUpdating = true;
            InterpolateHeadAndBody(sliders, abmx, useFactor);
            InterpolateAbmx(filters, sliders, abmx, useFactor, inverted);
        }

        public CharacterInterpolateSection(RegisterSubCategoriesEvent e, HooahRandMutationPlugin targetInstance) : base(
            e, targetInstance)
        {
            Category = new MakerCategory(MakerConstants.Body.CategoryName, SubCategoryName);
            e.AddSubCategory(Category);
            var textA = new MakerText(Constant.NotLoaded, Category, targetInstance);
            var textB = new MakerText(Constant.NotLoaded, Category, targetInstance);

            AddButton("Open Slider Preset Folder", CharacterData.CharacterSliders.OpenSlidePresetFolder);
            AddSeparator();
            AddButton("Set Current character as A", () =>
            {
                MakerChaControl.SetTemplate();
                if (textA.Exists) textA.Text = CharacterData.Templates.ElementAtOrDefault(0).CharacterName;
            });
            AddButton("Set Current character as B", () =>
            {
                MakerChaControl.SetTemplate(1);
                if (textB.Exists) textB.Text = CharacterData.Templates.ElementAtOrDefault(1).CharacterName;
            });
            AddButton("Save slider values of A", () => ABMXMutation.TrySaveSlot());
            AddButton("Save slider values of B", () => ABMXMutation.TrySaveSlot(1));
            AddButton("Load slider values of A", () =>
            {
                ABMXMutation.TryLoadSlot(0, () =>
                {
                    if (textA.Exists) textA.Text = CharacterData.Templates.ElementAtOrDefault(0).CharacterName;
                });
            });
            AddButton("Load slider values of B", () =>
            {
                ABMXMutation.TryLoadSlot(1, () =>
                {
                    if (textB.Exists) textB.Text = CharacterData.Templates.ElementAtOrDefault(1).CharacterName;
                });
            });

            AddSeparator();
            e.AddControl(textA);
            e.AddControl(textB);
            AddSeparator();

            // initialize sliders
            SliderValues = new FaceSliderValues(in e, in targetInstance, in Category, true);
            minSlider = AddSlider("Random Minimum Bias");
            maxSlider = AddSlider("Random Maximum Bias", 0, 1, 1);
            medianSlider = AddSlider("Random Median", 0, 1, 0.5f);
            rangeSlider = AddSlider("Random Deviation", 0, 1, 0.1f);
            mixSlider = AddSlider("Mix Factor", 0, 1, 0.5f);

            AddSeparator();

            mixType = new MakerDropdown("Mix Type", new[] { "Mix Face", "Mix Body", "Mix All" }, Category, 0,
                targetInstance);
            e.AddControl(mixType);
            defaultSide = AddToggle("Use First Character's Body", true);
            updateTick = AddToggle("Update when change");
            fixWarp = AddToggle("Remove face shifting");

            AddSeparator();

            var mixRandomized = AddToggle("Randomize Between Two Template");
            var mixSliders = AddToggle("Mix Sliders", true);
            var mixAbmx = AddToggle("Mix ABMX", true);

            void UpdateFactorCallback(float x)
            {
                if (updateTick.Value) UpdateSliders(true, mixSliders.Value, mixAbmx.Value);
            }

            void UpdateRandomCallback(float x)
            {
                if (updateTick.Value) UpdateSliders(false, mixSliders.Value, mixAbmx.Value);
            }

            void UpdateSliders(bool useFactor, bool sliders, bool abmx)
            {
                switch (mixTypeValue)
                {
                    case 0:
                        UpdateCallback(ABMXMutation.HeadBoneNames, sliders, abmx, useFactor);
                        break;
                    case 1:
                        UpdateCallback(ABMXMutation.HeadBoneNames, sliders, abmx, useFactor, true);
                        break;
                    case 2:
                        UpdateCallback(null, sliders, abmx, useFactor);
                        break;
                }
            }

            AddButton("Mix Sliders", () => UpdateSliders(!mixRandomized.Value, mixSliders.Value, mixAbmx.Value));
            // only interpolate sliders
            minSlider.ValueChanged.Subscribe(UpdateRandomCallback);
            maxSlider.ValueChanged.Subscribe(UpdateRandomCallback);
            rangeSlider.ValueChanged.Subscribe(UpdateRandomCallback);
            medianSlider.ValueChanged.Subscribe(UpdateRandomCallback);
            mixSlider.ValueChanged.Subscribe(UpdateFactorCallback);
            // interpolate sliders and abmx
        }
    }
}
