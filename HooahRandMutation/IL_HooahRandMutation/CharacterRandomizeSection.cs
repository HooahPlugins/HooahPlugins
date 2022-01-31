using System.Linq;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using UniRx;
using UnityEngine.Events;

namespace HooahRandMutation
{
    public class CharacterRandomizeSection : EditorSubSection
    {
        protected readonly string DisplayName = "MakerRandomMutation";
        protected readonly string SubCategoryName = "SliderMutation";

        private string SlotAName => CharacterData.Templates.ElementAtOrDefault(0).CharacterName ?? "Not Set";

        protected readonly ABMXSliderValues AbmxSliderValues;
        protected readonly FaceSliderValues CharacterSliderValues;

        public CharacterRandomizeSection(RegisterSubCategoriesEvent e, HooahRandMutationPlugin targetInstance) : base(e,
            targetInstance)
        {
            Category = new MakerCategory(MakerConstants.Body.CategoryName, SubCategoryName);
            e.AddSubCategory(Category);

            // todo: make this buttons... more compact.
            // [set][save][load][?]
            AddButton("Set current character as template", () => MakerChaControl.SetTemplate());
            AddButton("Save Current Template", () => ABMXMutation.TrySaveSlot());
            AddButton("Load Template From File", () => ABMXMutation.TryLoadSlot());
            AddButton("Open Slider Preset Folder", CharacterData.CharacterSliders.OpenSlidePresetFolder);

            e.AddControl(new MakerSeparator(Category, targetInstance));

            e.AddControl(new MakerText(SlotAName, Category, targetInstance));
            // todo: display current undo buffer


            e.AddControl(new MakerSeparator(Category, targetInstance));

            CharacterSliderValues = new FaceSliderValues(in e, in targetInstance, in Category);
            AddButton("Randomize Head Sliders", () =>
            {
                CharacterSliderValues.RandomizeHeadSliders();
                CharacterData.Push(MakerChaControl);
            });
            AddButton("Randomize Body Sliders", () =>
            {
                CharacterSliderValues.RandomizeBodySliders();
                CharacterData.Push(MakerChaControl);
            });

            e.AddControl(new MakerSeparator(Category, targetInstance));

            // ABMX slider values will be used in most of categories
            AbmxSliderValues = new ABMXSliderValues(in e, in targetInstance, in Category);
            AddButton("Randomize All ABMX Values", () =>
            {
                AbmxSliderValues.RandomizeAbmxSliders(null, true);
                CharacterData.Push(MakerChaControl);
            });
            AddButton("Randomize Head ABMX Values",
                () =>
                {
                    AbmxSliderValues.RandomizeAbmxSliders(ABMXMutation.HeadBoneNames, true);
                    CharacterData.Push(MakerChaControl);
                });
            AddButton("Randomize Body ABMX Values",
                () =>
                {
                    AbmxSliderValues.RandomizeAbmxSliders(ABMXMutation.HeadBoneNames, true, true);
                    CharacterData.Push(MakerChaControl);
                });

            e.AddControl(new MakerSeparator(Category, targetInstance));

            // todo: customizable abmx filters
            // separate bones with ";"
            // maybe load from file?
            // predefined?

            void OnClick()
            {
                CharacterSliderValues.RandomizeHeadSliders();
                AbmxSliderValues.RandomizeAbmxSliders(ABMXMutation.HeadBoneNames, false);
                Observable.NextFrame(FrameCountType.EndOfFrame).Take(1)
                    .Subscribe(_ => CharacterData.Push(MakerChaControl));
            }

            AddButton("Undo", () =>
            {
                if (CharacterData.Undo()) MakerChaControl.ApplySliders(CharacterData.Templates.FirstOrDefault());
            });
            AddButton("Redo", () =>
            {
                if (CharacterData.Redo()) MakerChaControl.ApplySliders(CharacterData.Templates.FirstOrDefault());
            });
            AddButton("Randomize Head Slider & ABMX", OnClick);
            AddButton("Randomize Body Slider & ABMX", OnClick);
            AddButton("Randomize All!", () =>
            {
                CharacterSliderValues.RandomizeHeadSliders();
                CharacterSliderValues.RandomizeBodySliders();
                AbmxSliderValues.RandomizeAbmxSliders(null, false);
                Observable.NextFrame(FrameCountType.EndOfFrame).Take(1)
                    .Subscribe(_ => CharacterData.Push(MakerChaControl));
            });
        }
    }
}
