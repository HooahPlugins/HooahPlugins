using System.Linq;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UniRx;

namespace HooahRandMutation.IL_HooahRandMutation
{
    // todo: this is a mess!
    public class CharacterInterpolateSection : EditorSubSection
    {
        protected readonly string DisplayName = "CharacterInterpolation";
        protected readonly string SubCategoryName = "CharacterInterpolation";

        protected readonly ABMXSliderValues AbmxSliderValues;
        protected readonly FaceSliderValues FaceSliderValues;

        private string SlotAName => InterpolateShapeUtility.Templates.ElementAtOrDefault(0).CharacterName ?? "Not Set";
        private string SlotBName => InterpolateShapeUtility.Templates.ElementAtOrDefault(1).CharacterName ?? "Not Set";


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
            var toggle = AddToggle("First Character is Default", true);
            var updateTick = AddToggle("Update Real-Time", false);

            // initialize sliders
            FaceSliderValues = new FaceSliderValues(in e, in targetInstance, in Category, true);

            e.AddControl(new MakerSeparator(Category, targetInstance));
            var min = AddSlider("Min Factor");
            var max = AddSlider("Max Factor", 0, 1, 1);
            var median = AddSlider("Median Value", 0, 1, 0.5f);
            var range = AddSlider("Random Range from Median", 0, 1, 0.1f);
            var mix = AddSlider("Mix Factor (Fixed)", 0, 1, 0.5f);
            e.AddControl(new MakerSeparator(Category, targetInstance));

            AddButton("Interpolate Head Sliders (Random)",
                () => FaceSliderValues.InterpolateHeadSliders(min.Value, max.Value, median.Value, range.Value));
            AddButton("Interpolate Head Sliders (Factor)",
                () => FaceSliderValues.InterpolateHeadSlidersWithFactor(min.Value, max.Value, median.Value, range.Value,
                    mix.Value));
            // only interpolate sliders

            e.AddControl(new MakerSeparator(Category, targetInstance));

            // initialize abmx values
            AbmxSliderValues = new ABMXSliderValues(in e, in targetInstance, in Category, true);
            AddButton("Interpolate ABMX (Random)",
                () => AbmxSliderValues.InterpolateAbmxSliders(toggle.Value ? 0 : 1, ABMXMutation.HeadBoneNames,
                    min.Value, max.Value, median.Value, range.Value));

            AddButton("Interpolate ABMX (Fixed)",
                () => AbmxSliderValues.InterpolateAbmxSliders(toggle.Value ? 0 : 1, ABMXMutation.HeadBoneNames,
                    min.Value, max.Value, median.Value, range.Value,
                    true, mix.Value));

            e.AddControl(new MakerSeparator(Category, targetInstance));

            AddButton("Interpolate Face ABMX & Slider (Random)",
                () =>
                {
                    FaceSliderValues.InterpolateHeadSliders(min.Value, max.Value, median.Value, range.Value);
                    AbmxSliderValues.InterpolateAbmxSliders(toggle.Value ? 0 : 1, ABMXMutation.HeadBoneNames, min.Value,
                        max.Value, median.Value, range.Value);
                });
            AddButton("Interpolate Face ABMX & Slider (Fixed)",
                () =>
                {
                    FaceSliderValues.InterpolateHeadSlidersWithFactor(min.Value, max.Value, median.Value, range.Value,
                        mix.Value);
                    AbmxSliderValues.InterpolateAbmxSliders(toggle.Value ? 0 : 1, ABMXMutation.HeadBoneNames, min.Value,
                        max.Value, median.Value, range.Value, true,
                        mix.Value);
                });

            void UpdateFactor(float x)
            {
                if (!updateTick.Value) return;
                FaceSliderValues.InterpolateHeadSlidersWithFactor(min.Value, max.Value, median.Value, range.Value,
                    mix.Value);
                AbmxSliderValues.InterpolateAbmxSliders(toggle.Value ? 0 : 1, ABMXMutation.HeadBoneNames, min.Value,
                    max.Value, median.Value, range.Value, true, mix.Value);
            }

            void UpdateRealTime(float x)
            {
                if (!updateTick.Value) return;
                FaceSliderValues.InterpolateHeadSliders(min.Value, max.Value, median.Value, range.Value);
                AbmxSliderValues.InterpolateAbmxSliders(toggle.Value ? 0 : 1, ABMXMutation.HeadBoneNames, min.Value,
                    max.Value, median.Value, range.Value);
            }

            min.ValueChanged.Subscribe(UpdateRealTime);
            max.ValueChanged.Subscribe(UpdateRealTime);
            range.ValueChanged.Subscribe(UpdateRealTime);
            median.ValueChanged.Subscribe(UpdateRealTime);
            mix.ValueChanged.Subscribe(UpdateFactor);
            // interpolate sliders and abmx
        }
    }
}
