using AIChara;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UnityEngine;

namespace HooahRandMutation.IL_HooahRandMutation
{
    public class CharacterInterpolateSection : EditorSubSection
    {
        protected readonly string DisplayName = "CharacterInterpolation";
        protected readonly string SubCategoryName = "CharacterInterpolation";

        protected readonly ABMXSliderValues AbmxSliderValues;
        protected readonly FaceSliderValues FaceSliderValues;


        public CharacterInterpolateSection(RegisterSubCategoriesEvent e, HooahRandMutationPlugin targetInstance) : base(
            e, targetInstance)
        {
            Category = new MakerCategory(MakerConstants.Body.CategoryName, SubCategoryName);
            e.AddSubCategory(Category);

            AddButton("Set Current character as A", () => MakerChaControl.SetTemplate());
            AddButton("Set Current character as B", () => MakerChaControl.SetTemplate(1));
            var toggle = AddToggle("First Character is Default", true);

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

            AddButton("Interpolate ABMX & Slider (Random)",
                () =>
                {
                    FaceSliderValues.InterpolateHeadSliders(min.Value, max.Value, median.Value, range.Value);
                    AbmxSliderValues.InterpolateAbmxSliders(toggle.Value ? 0 : 1, ABMXMutation.HeadBoneNames, min.Value,
                        max.Value, median.Value, range.Value);
                });
            AddButton("Interpolate ABMX & Slider (Fixed)",
                () =>
                {
                    FaceSliderValues.InterpolateHeadSlidersWithFactor(min.Value, max.Value, median.Value, range.Value,
                        mix.Value);
                    AbmxSliderValues.InterpolateAbmxSliders(toggle.Value ? 0 : 1, ABMXMutation.HeadBoneNames, min.Value,
                        max.Value, median.Value, range.Value, true,
                        mix.Value);
                });

            // interpolate sliders and abmx
        }
    }
}
