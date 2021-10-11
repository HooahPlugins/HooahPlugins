using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HooahUtility.Utility
{
    public static class ColorPickerUtility
    {
        /// <summary>
        /// Santiive a given string so that it encodes a valid hex color string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="full">Insert zeroes to match #RRGGBB format </param>
        public static string GetSanitizedHex(string input, bool full)
        {
            if (string.IsNullOrEmpty(input))
                return "#";

            var toReturn = new List<char> {'#'};
            var i = 0;
            var chars = input.ToCharArray();
            while (toReturn.Count < 7 && i < input.Length)
            {
                var nextChar = char.ToUpper(chars[i++]);
                var validChar = char.IsNumber(nextChar);
                validChar |= nextChar >= 'A' && nextChar <= 'F';
                if (validChar)
                    toReturn.Add(nextChar);
            }

            while (full && toReturn.Count < 7)
                toReturn.Insert(1, '0');

            return new string(toReturn.ToArray());
        }

        /// <summary>
        /// Get normalized position of the given pointer event relative to the given rect.
        /// (e.g. return [0,1] for top left corner). This method correctly takes into 
        /// account relative positions, canvas render mode and general transformations, 
        /// including rotations and scale.
        /// </summary>
        /// <param name="canvas">parent canvas of the rect (and therefore the FCP)</param>
        /// <param name="rect">Rect to find relative position to</param>
        /// <param name="e">Pointer event for which to find the relative position</param>
        public static Vector2 GetNormalizedPointerPosition(Canvas canvas, RectTransform rect, BaseEventData e)
        {
            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceCamera:
                    return canvas.worldCamera == null
                        ? GetNormScreenSpace(rect, e)
                        : GetNormWorldSpace(canvas, rect, e);

                case RenderMode.ScreenSpaceOverlay:
                    return GetNormScreenSpace(rect, e);

                case RenderMode.WorldSpace:
                    if (canvas.worldCamera != null) return GetNormWorldSpace(canvas, rect, e);
                    Debug.LogError(
                        "FCP in world space render mode requires an event camera to be set up on the parent canvas!");
                    return Vector2.zero;

                default: return Vector2.zero;
            }
        }

        /// <summary>
        /// Get normalized position in the case of a screen space (overlay) 
        /// type canvas render mode
        /// </summary>
        private static Vector2 GetNormScreenSpace(RectTransform rect, BaseEventData e)
        {
            var screenPoint = ((PointerEventData) e).position;
            Vector2 localPos = rect.worldToLocalMatrix.MultiplyPoint(screenPoint);
            var x = Mathf.Clamp01((localPos.x / rect.rect.size.x) + rect.pivot.x);
            var y = Mathf.Clamp01((localPos.y / rect.rect.size.y) + rect.pivot.y);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Get normalized position in the case of a world space (or screen space camera) 
        /// type canvas render mode.
        /// </summary>
        private static Vector2 GetNormWorldSpace(Canvas canvas, RectTransform rect, BaseEventData e)
        {
            var screenPoint = ((PointerEventData) e).position;
            var pointerRay = canvas.worldCamera.ScreenPointToRay(screenPoint);
            var transform = canvas.transform;
            var canvasPlane = new Plane(transform.forward, transform.position);
            canvasPlane.Raycast(pointerRay, out var enter);
            var worldPoint = pointerRay.origin + enter * pointerRay.direction;
            Vector2 localPoint = rect.worldToLocalMatrix.MultiplyPoint(worldPoint);

            var x = Mathf.Clamp01((localPoint.x / rect.rect.size.x) + rect.pivot.x);
            var y = Mathf.Clamp01((localPoint.y / rect.rect.size.y) + rect.pivot.y);
            return new Vector2(x, y);
        }
    }
}