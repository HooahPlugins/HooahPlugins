using KKAPI.Maker;

namespace HooahRandMutation.IL_HooahRandMutation
{
    public class CharacterInterpolateSection : SectionBase
    {
        protected readonly string DisplayName = "CharacterInterpolation";
        protected readonly string SubCategoryName = "CharacterInterpolation";

        protected readonly ABMXSliderValues AbmxSliderValues;
        protected readonly FaceSliderValues FaceSliderValues;

        public CharacterInterpolateSection(RegisterSubCategoriesEvent e, MakerCategory cat,
            HooahRandMutationPlugin targetInstance) : base(e, cat, targetInstance)
        {
            AddButton("Set Current character as A", () => MakerAPI.GetCharacterControl().SetTemplate());
            AddButton("Set Current character as B", () => MakerAPI.GetCharacterControl().SetTemplate(1));
            var mix = AddSlider("Mix Factor");

            // initialize sliders
            FaceSliderValues = new FaceSliderValues(Event, Category, targetInstance);
            AddButton("Interpolate Head Sliders (Random)", () => FaceSliderValues.InterpolateHeadSliders());
            AddButton("Interpolate Head Sliders (Random)",
                () => FaceSliderValues.InterpolateHeadSlidersWithFactor(mix.Value));
            // only interpolate sliders

            // initialize abmx values
            AbmxSliderValues = new ABMXSliderValues(Event, Category, targetInstance);
            // only interpolate abmx

            // interpolate sliders and abmx
        }
    }
}
