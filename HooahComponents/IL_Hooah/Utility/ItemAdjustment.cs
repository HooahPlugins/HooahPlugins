using AdvancedStudioUI;

namespace HooahComponents.Utility
{
    public static class ItemAdjustment
    {
        public static void InitializeAdjustmentTab(StudioItemControl self, TabbedContentControl content)
        {
            var form = self.CreateTab("item", "Object");
            // todo: convert this to cached forms to reduce the initialization cost.

            /*
             * # TODO: Add Item adjustment feature
             * ## Character Specific Adjustment
             * - hair wetness adjustment : adjust all the wetness of the hair (INCLUDING ACCESSORY HAIRS)
             * - serializable eye/neck adjustment
             *   1. angle range
             *   2. near target adjustment
             *   3. tracking speed?
             *   4. tracking give up threshold
             *   5. tracking root adjustment (the anchor point of the direction calculation)
             * ## Studio Item Adjustment
             * ## Folder and Light adjustment ...
             * - light adjustment should be integrated with graphics plugin
             */
        }
    }
}