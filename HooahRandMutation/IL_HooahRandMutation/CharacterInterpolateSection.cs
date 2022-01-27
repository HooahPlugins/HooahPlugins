using System;
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
        protected readonly FaceSliderValues FaceSliderValues;

        private string SlotAName => CharacterData.Templates.ElementAtOrDefault(0).CharacterName ?? "Not Set";
        private string SlotBName => CharacterData.Templates.ElementAtOrDefault(1).CharacterName ?? "Not Set";

        public bool IsUpdating = false;

        public void UpdateFace(
            MakerSlider min, MakerSlider max, MakerSlider median, MakerSlider range, MakerSlider mix,
            MakerToggle defaultSide, MakerToggle fixWarp, bool sliders, bool abmx, bool useFactor = false,
            bool inverted = false, bool notRealtime = false)
        {
            if (notRealtime) return;
            if (IsUpdating) return;

            IsUpdating = true;

            try
            {
                if (sliders)
                {
                    var ctl = MakerChaControl.GetComponent<BoneController>();
                    ctl.enabled = false;
                    if (useFactor)
                        FaceSliderValues.InterpolateHeadSlidersWithFactor(MakerChaControl, min.Value, max.Value,
                            median.Value, range.Value, mix.Value, abmx);
                    else
                        FaceSliderValues.InterpolateHeadSliders(MakerChaControl, min.Value, max.Value, median.Value,
                            range.Value, abmx);
                    ctl.enabled = true;

                    if (!abmx) IsUpdating = false;
                }

                if (abmx)
                {
                    // todo: validate essential shits
                    try
                    {
                        MakerChaControl.InterpolateAbmx(defaultSide.Value ? 0 : 1,
                            ABMXMutation.HeadBoneNames,
                            min.Value, max.Value, median.Value, range.Value, useFactor, mix.Value,
                            inverted, fixWarp.Value);
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
                                if (sliders) MakerChaControl.AltFaceUpdate();
                            });
                    }
                }
            }
            catch (Exception e)
            {
                HooahRandMutationPlugin.instance.loggerInstance.LogError(e);
                IsUpdating = false;
            }
        }

        public CharacterInterpolateSection(RegisterSubCategoriesEvent e, HooahRandMutationPlugin targetInstance) : base(
            e, targetInstance)
        {
            Category = new MakerCategory(MakerConstants.Body.CategoryName, SubCategoryName);
            e.AddSubCategory(Category);

            AddButton("Set Current character as A", () => MakerChaControl.SetTemplate());
            AddButton("Set Current character as B", () => MakerChaControl.SetTemplate(1));
            AddButton("Save slider values of A", () => ABMXMutation.TrySaveSlot());
            AddButton("Save slider values of B", () => ABMXMutation.TrySaveSlot(1));
            AddButton("Load slider values of A", () => ABMXMutation.TryLoadSlot());
            AddButton("Load slider values of B", () => ABMXMutation.TryLoadSlot(1));

            e.AddControl(new MakerSeparator(Category, targetInstance));
            e.AddControl(new MakerText(SlotAName, Category, targetInstance));
            e.AddControl(new MakerText(SlotBName, Category, targetInstance));
            e.AddControl(new MakerSeparator(Category, targetInstance));
            var toggle = AddToggle("Use First Character's Body", true);
            var updateTick = AddToggle("Update when change", false);
            var preventFix = AddToggle("Remove face shifting", false);

            // initialize sliders
            FaceSliderValues = new FaceSliderValues(in e, in targetInstance, in Category, true);

            e.AddControl(new MakerSeparator(Category, targetInstance));
            var min = AddSlider("Random Minimum Bias");
            var max = AddSlider("Random Maximum Bias", 0, 1, 1);
            var median = AddSlider("Random Median", 0, 1, 0.5f);
            var range = AddSlider("Random Deviation", 0, 1, 0.1f);
            var mix = AddSlider("Mix Factor", 0, 1, 0.5f);
            e.AddControl(new MakerSeparator(Category, targetInstance));

            void UpdateFactor(float x)
            {
                UpdateFace(min, max, median, range, mix, toggle, preventFix, true, true, true, false,
                    !updateTick.Value);
            }

            void UpdateRealTime(float x)
            {
                UpdateFace(min, max, median, range, mix, toggle, preventFix, true, true, false, false,
                    !updateTick.Value);
            }

            AddButton("Mix Head Sliders with Randomized Factor",
                () => UpdateFace(min, max, median, range, mix, toggle, preventFix, true, false)
            );
            AddButton("Mix Head Sliders with Mix Factor",
                () => UpdateFace(min, max, median, range, mix, toggle, preventFix, true, false, true)
            );
            // only interpolate sliders

            e.AddControl(new MakerSeparator(Category, targetInstance));

            // initialize abmx values
            AbmxSliderValues = new ABMXSliderValues(in e, in targetInstance, in Category, true);
            AddButton("Mix Head ABMX with Randomized Factor",
                () => UpdateFace(min, max, median, range, mix, toggle, preventFix, false, true)
            );

            AddButton("Mix Head ABMX with Mix Factor",
                () => UpdateFace(min, max, median, range, mix, toggle, preventFix, false, true, true)
            );

            e.AddControl(new MakerSeparator(Category, targetInstance));

            AddButton("Mix Head Slider&ABMX with Randomized Factor",
                () => UpdateFace(min, max, median, range, mix, toggle, preventFix, true, true)
            );
            AddButton("Mix Head Slider&ABMX with Mix Factor",
                () => UpdateFace(min, max, median, range, mix, toggle, preventFix, true, true, true)
            );

            min.ValueChanged.Subscribe(UpdateRealTime);
            max.ValueChanged.Subscribe(UpdateRealTime);
            range.ValueChanged.Subscribe(UpdateRealTime);
            median.ValueChanged.Subscribe(UpdateRealTime);
            mix.ValueChanged.Subscribe(UpdateFactor);
            // interpolate sliders and abmx
        }
    }
}
