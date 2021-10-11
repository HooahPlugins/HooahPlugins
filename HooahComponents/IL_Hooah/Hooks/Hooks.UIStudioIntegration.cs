#if AI || HS2
using HarmonyLib;
using HooahUtility.Utility;
using Studio;
#endif

// todo: support multi component structure.
namespace HooahComponents.Hooks
{
    // Collection of Game Intergration hooks and harmony hooks
    public partial class Hooks
    {
#if AI || HS2
        public static void RegisterEvents()
        {
            var treeNodeCtrl = Studio.Studio.Instance.treeNodeCtrl;
            if (treeNodeCtrl == null)
            {
                return;
            }

            treeNodeCtrl.onSelect += OnSelectStudioItem;
            treeNodeCtrl.onSelectMultiple += OnSelectStudioItem;
            treeNodeCtrl.onDelete += OnDeselectStudioItem;
            treeNodeCtrl.onDeselect += OnDeselectStudioItem;
            treeNodeCtrl.onParentage += OnParentItem;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.AddItem))]
        public static void OnAddItem(Studio.Studio __instance, int _group, int _category, int _no) => OnCreateItem();

        [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.AddLight))]
        public static void OnAddLight(Studio.Studio __instance, int _no) => OnCreateItem();

        [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.AddCamera))]
        public static void OnAddCamera(Studio.Studio __instance) => OnCreateItem();

        [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.AddFemale))]
        public static void OnAddFemale(Studio.Studio __instance) => OnCreateItem();

        [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.AddMale))]
        public static void OnAddMale(Studio.Studio __instance) => OnCreateItem();

        [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.AddFolder))]
        public static void OnAddFolder(Studio.Studio __instance) => OnCreateItem();

        [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.AddRoute))]
        public static void OnAddRoute(Studio.Studio __instance) => OnCreateItem();

        [HarmonyPostfix, HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.Duplicate))]
        public static void OnDuplicate(Studio.Studio __instance)
        {
            OnDeselectStudioItem();
            // please work
            foreach (var change in StudioReferenceUtility.GetItemChanges())
                StudioReferenceUtility.CopyComponentsData(change.FromOci, change.ToOci);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(SystemButtonCtrl), "OnSelectInitYes")]
        public static void OnResetScene() // Init
        {
            OnDeselectStudioItem();
        }
#endif
    }
}