#if AI ||HS2
using AIChara;
using HarmonyLib;
using HooahComponents.Utility;
using Studio;
#endif

namespace HooahComponents.Hooks
{
    public partial class Hooks
    {
#if AI || HS2
        /// <summary>
        ///     Target: AI/HS2 Studio
        ///     Related: SkinnedAccessory
        ///     Remove cached ChaControl
        /// </summary>
        // Static, does not have to get instance.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Studio.Studio), "DeleteInfo")]
        public static void Delete(ObjectInfo _info, bool _delKey = true)
        {
            // Check invalid harmony calls.
            if (!(_info is OICharInfo)) return;
            var dictionaryKey = _info.dicKey;
            if (dictionaryKey < 0 || !Singleton<Studio.Studio>.Instance.dicObjectCtrl.TryGetValue(dictionaryKey, out var ctrlInfo)) return;

            // Check valid character control from the object info.
            ChaControl chaControl = null;

            // ReSharper disable once ConvertIfStatementToSwitchStatement - it's shorter and easy to understand.
            if (ctrlInfo is OCICharFemale female) chaControl = female.female;
            else if (ctrlInfo is OCICharMale male) chaControl = male.male;

            // remove character control.
            if (chaControl != null) SkinnedBones.CleanUpCache(chaControl);
        }
#endif
    }
}