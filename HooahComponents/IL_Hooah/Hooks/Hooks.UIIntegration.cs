#if AI || HS2
using System.Linq;
using AdvancedStudioUI;
using HooahUtility.Model;
using HooahUtility.Utility;
using Studio;

#endif

namespace HooahComponents.Hooks
{
    public partial class Hooks
    {
#if AI || HS2

        #region Aliases

        // recalcuate the view?  hmm?
        public static void OnParentItem(TreeNodeObject from, TreeNodeObject to) => OnDeselectStudioItem();

        public static void OnSelectStudioItem(TreeNodeObject o) => OnSelectStudioItem();

        private static bool CheckStudioInstance(out Studio.Studio instance)
        {
            instance = Studio.Studio.Instance;
            return instance != null;
        }

        public static void OnDeselectStudioItem(TreeNodeObject o) => OnDeselectStudioItem();

        #endregion

        #region Actual Logic

        // todo: add state reset when scene reset invoked
        // todo: integrate with space no-ui shortcut
        public static void OnCreateItem()
        {
            if (!CheckStudioInstance(out var instance)) return;
            if (!Studio.Studio.optionSystem.autoSelect) return;
            var currentNode = instance.treeNodeCtrl.selectNode;
            OnSelectStudioItem(currentNode);
        }

        public static void OnSelectStudioItem()
        {
            if (!CheckStudioInstance(out var instance)) return;
            var selectedObjects = instance.treeNodeCtrl.selectObjectCtrl
                .Select(StudioReferenceUtility.GetOciEndNodeGameObject)
                .Where(x => x != null).ToArray();

            if (!StudioItemControl.TryGetInstance(out var itemControl)) return;
            itemControl.ClearForm();

            if (
                selectedObjects
                    .Select(x => x.GetComponent<IFormData>())
                    .Any(x => x != null)
            ) itemControl.MakeForm(selectedObjects);
        }

        public static void OnDeselectStudioItem()
        {
            if (!StudioItemControl.TryGetInstance(out var instance)) return;
            instance.ClearForm();
        }

        #endregion

#endif
    }
}
