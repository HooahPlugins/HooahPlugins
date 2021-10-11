using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdvancedStudioUI
{
    public partial class ColorPickerControl
    {
        private EventTrigger.Entry FocusEventEntry(Image targetComponent, PickerType pickerType)
        {
            var entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerDown};
            entry.callback.AddListener(delegate(BaseEventData eventData)
            {
                SetPointerFocus(eventData, targetComponent, pickerType);
            });
            return entry;
        }

        private EventTrigger.Entry DragUpdateEntry()
        {
            var entry = new EventTrigger.Entry {eventID = EventTriggerType.Drag};
            entry.callback.AddListener(PointerUpdate);
            return entry;
        }
        
        private EventTrigger.Entry ScrollUpdateEntry()
        {
            var entry = new EventTrigger.Entry {eventID = EventTriggerType.Scroll};
            entry.callback.AddListener(ScrollUpdate);
            return entry;
        }
    }
}