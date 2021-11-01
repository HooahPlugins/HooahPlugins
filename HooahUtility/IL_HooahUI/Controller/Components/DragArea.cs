using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HooahUtility.Controller.Components
{
    public class DragArea : MonoBehaviour, IDragHandler
    {
        public event Action<PointerEventData> onDrag;

        public void OnDrag(PointerEventData eventData)
        {
            onDrag?.Invoke(eventData);
        }
    }
}
