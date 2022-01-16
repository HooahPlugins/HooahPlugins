// todo: add check if the template exists.
// if there is no template, just use the current character as template for once.

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HooahRandMutation.IL_HooahRandMutation;
using KKAPI.Maker;

namespace HooahRandMutation
{
    public partial class HooahRandMutationPlugin : BaseUnityPlugin
    {
        private static ManualLogSource _logger;
        private static HooahRandMutationPlugin _instance;

        private void Start()
        {
            /*
             * Thanks for 2155X for good code to start!
             */
            _instance = this;
            _logger = Logger;
            Harmony.CreateAndPatchAll(typeof(HooahRandMutationPlugin));
            MakerAPI.RegisterCustomSubCategories += (sender, e) =>
            {
                // todo: maybe use dropdown for this one?
                var all = MakerConstants.Body.All;
                RandomizerSection = new CharacterRandomizeSection(e, all, _instance);
                BlendingCategorySection = new CharacterInterpolateSection(e, all, _instance);
            };
        }

        public CharacterInterpolateSection BlendingCategorySection { get; set; }

        public CharacterRandomizeSection RandomizerSection { get; set; }
    }
}
