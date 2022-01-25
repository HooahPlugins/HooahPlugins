using System.Linq;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UnityEngine.Events;

namespace HooahRandMutation
{
    public class CharacterRandomizeSection : EditorSubSection
    {
        protected readonly string DisplayName = "MakerRandomMutation";
        protected readonly string SubCategoryName = "SliderMutation";

        private string SlotAName => CharacterData.Templates.ElementAtOrDefault(0).CharacterName ?? "Not Set";

        protected readonly ABMXSliderValues AbmxSliderValues;
        protected readonly FaceSliderValues FaceSliderValues;

        public CharacterRandomizeSection(RegisterSubCategoriesEvent e, HooahRandMutationPlugin targetInstance) : base(e,
            targetInstance)
        {
            Category = new MakerCategory(MakerConstants.Body.CategoryName, SubCategoryName);
            e.AddSubCategory(Category);

            AddButton("Set current character as template", () => MakerChaControl.SetTemplate());
            AddButton("Save Current Template", () => ABMXMutation.TrySaveSlot());
            AddButton("Load Template From File", () => ABMXMutation.TryLoadSlot());

            e.AddControl(new MakerSeparator(Category, targetInstance));

            e.AddControl(new MakerText(SlotAName, Category, targetInstance));
            // todo: display current undo buffer


            e.AddControl(new MakerSeparator(Category, targetInstance));

            FaceSliderValues = new FaceSliderValues(in e, in targetInstance, in Category);
            AddButton("Randomize Head Sliders", () => { FaceSliderValues.RandomizeHeadSliders(); });

            e.AddControl(new MakerSeparator(Category, targetInstance));

            // ABMX slider values will be used in most of categories
            AbmxSliderValues = new ABMXSliderValues(in e, in targetInstance, in Category);
            AddButton("Randomize All ABMX Values", () => AbmxSliderValues.RandomizeAbmxSliders(null));
            AddButton("Randomize Head ABMX Values",
                () => AbmxSliderValues.RandomizeAbmxSliders(ABMXMutation.HeadBoneNames));

            e.AddControl(new MakerSeparator(Category, targetInstance));

            // todo: customizable abmx filters
            // separate bones with ";"
            // maybe load from file?
            // predefined?

            void OnClick()
            {
                FaceSliderValues.RandomizeHeadSliders();
                AbmxSliderValues.RandomizeAbmxSliders(ABMXMutation.HeadBoneNames);
                CharacterData.Push(MakerChaControl);
            }

            AddButton("Undo", () =>
            {
                CharacterData.Undo();
                MakerChaControl.ApplySliders(CharacterData.Templates.FirstOrDefault());
            });
            AddButton("Redo", () =>
            {
                CharacterData.Redo();
                MakerChaControl.ApplySliders(CharacterData.Templates.FirstOrDefault());
            });
            AddButton("Randomize Head Slider & ABMX", OnClick);
        }
    }
}
