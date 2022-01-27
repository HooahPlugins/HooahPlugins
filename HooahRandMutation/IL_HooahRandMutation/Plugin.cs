// todo: add check if the template exists.
// if there is no template, just use the current character as template for once.

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HooahRandMutation;
using KKAPI.Maker;

namespace HooahRandMutation
{
    public partial class HooahRandMutationPlugin : BaseUnityPlugin
    {
        public static HooahRandMutationPlugin instance;
        public ManualLogSource loggerInstance => Logger;

        private void Start()
        {
            /*
             * Thanks for 2155X for good code to start!
             */
            instance = this;
            Harmony.CreateAndPatchAll(typeof(HooahRandMutationPlugin));
            MakerAPI.RegisterCustomSubCategories += (sender, e) =>
            {
                // todo: maybe use dropdown for this one?
                RandomizerSection = new CharacterRandomizeSection(e, instance);
                BlendingCategorySection = new CharacterInterpolateSection(e, instance);
            };
        }

        public CharacterInterpolateSection BlendingCategorySection { get; set; }

        public CharacterRandomizeSection RandomizerSection { get; set; }
    }
}
