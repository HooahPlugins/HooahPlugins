using System;
using UnityEngine;

namespace HooahUtility.Controller.ContentManagers
{
    [Serializable]
    public class ContentManager
    {
        public RectTransform uiRectTransformParent;
        public RectTransform uiRectTransformParentWrapper;

        public void SyncHeight()
        {
            var delta = uiRectTransformParentWrapper.sizeDelta;
            delta.y = uiRectTransformParent.sizeDelta.y;
            uiRectTransformParentWrapper.sizeDelta = delta;
        }
    }
}