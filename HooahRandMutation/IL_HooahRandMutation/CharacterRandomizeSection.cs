using KKAPI.Maker;

namespace HooahRandMutation.IL_HooahRandMutation
{
    public class CharacterRandomizeSection : EditorSubSection
    {
        protected readonly string DisplayName = "MakerRandomMutation";
        protected readonly string SubCategoryName = "SliderMutation";

        protected readonly ABMXSliderValues AbmxSliderValues;
        protected readonly FaceSliderValues FaceSliderValues;

        public CharacterRandomizeSection(RegisterSubCategoriesEvent e, HooahRandMutationPlugin targetInstance) : base(e,
            targetInstance)
        {
            Category = new MakerCategory(MakerConstants.Body.CategoryName, SubCategoryName);
            e.AddSubCategory(Category);

            AddButton("Set current character as template", () => MakerChaControl.SetTemplate());
            FaceSliderValues = new FaceSliderValues(in e, in targetInstance, in Category);
            AddButton("Randomize Head Sliders", () => { FaceSliderValues.RandomizeHeadSliders(); });

            // ABMX slider values will be used in most of categories
            AbmxSliderValues = new ABMXSliderValues(in e, in targetInstance, in Category);
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
