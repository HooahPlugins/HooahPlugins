using KKAPI.Maker;

namespace HooahRandMutation.IL_HooahRandMutation
{
    public class CharacterRandomizeSection : SectionBase
    {
        protected readonly string DisplayName = "MakerRandomMutation";
        protected readonly string SubCategoryName = "SliderMutation";

        protected readonly ABMXSliderValues AbmxSliderValues;
        protected readonly FaceSliderValues FaceSliderValues;

        public CharacterRandomizeSection(RegisterSubCategoriesEvent e, MakerCategory cat,
            HooahRandMutationPlugin targetInstance) : base(e, cat, targetInstance)
        {
            AddButton("Set current character as template", () => MakerAPI.GetCharacterControl().SetTemplate());

            FaceSliderValues = new FaceSliderValues(Event, Category, targetInstance);
            AddButton("Randomize Head Sliders", FaceSliderValues.RandomizeHeadSliders);

            // ABMX slider values will be used in most of categories
            AbmxSliderValues = new ABMXSliderValues(Event, Category, targetInstance);
            AddButton("Randomize All ABMX Values", () => AbmxSliderValues.RandomizeAbmxSliders(null));
            AddButton("Randomize Head ABMX Values",
                () => AbmxSliderValues.RandomizeAbmxSliders(ABMXMutation.HeadBoneNames));
            AddButton("Randomize Head Slider&ABMX", () =>
            {
                FaceSliderValues.RandomizeHeadSliders();
                AbmxSliderValues.RandomizeAbmxSliders(ABMXMutation.HeadBoneNames);
            });
        }
    }
}
