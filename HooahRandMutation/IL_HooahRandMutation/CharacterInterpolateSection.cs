using AIChara;
using KKAPI.Maker;
using UnityEngine;

namespace HooahRandMutation.IL_HooahRandMutation
{
    public class CharacterInterpolateSection : EditorSubSection
    {
        protected readonly string DisplayName = "CharacterInterpolation";
        protected readonly string SubCategoryName = "CharacterInterpolation";

        protected readonly ABMXSliderValues AbmxSliderValues;
        protected readonly FaceSliderValues FaceSliderValues;


        public CharacterInterpolateSection(RegisterSubCategoriesEvent e, HooahRandMutationPlugin targetInstance) : base(e, targetInstance)
        {
            Category = new MakerCategory(MakerConstants.Body.CategoryName, SubCategoryName);
            e.AddSubCategory(Category);

            AddButton("Set Current character as A", () => MakerChaControl.SetTemplate());
            AddButton("Set Current character as B", () => MakerChaControl.SetTemplate(1));
            var mix = AddSlider("Mix Factor");

            // initialize sliders
            FaceSliderValues = new FaceSliderValues(in e, in targetInstance, in Category);
            AddButton("Interpolate Head Sliders (Random)", () => FaceSliderValues.InterpolateHeadSliders());
            AddButton("Interpolate Head Sliders (Random)", () => FaceSliderValues.InterpolateHeadSlidersWithFactor(mix.Value));
            // only interpolate sliders

            // initialize abmx values
            AbmxSliderValues = new ABMXSliderValues(in e, in targetInstance, in Category);
            // only interpolate abmx

            // interpolate sliders and abmx
        }
    }
}
