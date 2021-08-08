using System;
using UnityEngine;

namespace HooahUtility.AdvancedStudioUI
{
    // todo: add ui builder or some sort of thing?
    public static class UIUtility
    {
        public static RectTransform ScaleAsGrid(GameObject t, int i, float div, float margin = 0)
        {
            var tt = t.GetComponent<RectTransform>();
            return tt == null ? null : ScaleAsGrid(tt, i, div, margin);
        }

        public static RectTransform ScaleAsGrid(RectTransform t, int i, float div, float margin = 0)
        {
            t.anchorMin = new Vector2(Math.Max(0, i * div), 0f);
            t.anchorMax = new Vector2(Math.Min(1, (i + 1) * div), 1f);
            t.offsetMax = new Vector2(margin, 0f);
            t.offsetMin = Vector2.zero;
            return t;
        }
    }
}