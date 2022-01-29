using System.Collections.Generic;
using AIChara;
using UnityEngine;

namespace HooahRandMutation
{
    public static class Utility
    {
        public static float GetRandomNumber(float min, float max, float median, float range)
        {
            var randomValue = median + Random.Range(-range, range);
            return Mathf.Max(min, Mathf.Min(randomValue, max));
        }

        public static Vector3 ScaleAndReturn(this Vector3 inVector, Vector3 scale)
        {
            inVector.Scale(scale);
            return inVector;
        }

        public enum ABMXValueType { Position, Angle, Scale }

        public static Vector3 GetSaneValue(in CharacterData.ABMXValues value, ABMXValueType type,
            Vector3 defaultVector)
        {
            if (value == null) return defaultVector;
            switch (type)
            {
                case ABMXValueType.Position:
                    return value.Position;
                case ABMXValueType.Angle:
                    return value.VectorAngle;
                case ABMXValueType.Scale:
                    return value.Scale;
            }

            return defaultVector;
        }

        public static Vector3 LerpValue(IReadOnlyList<CharacterData.ABMXValues> values, ABMXValueType type,
            float lerpFactor)
        {
            var defaultVector = type == ABMXValueType.Scale ? Vector3.one : Vector3.zero;
            if (values == null) return defaultVector;
            if (values.Count < 2) return defaultVector;
            var first = values[0];
            var second = values[1];
            if (first == null && second == null) return defaultVector;

            return Vector3.Lerp(
                GetSaneValue(first, type, defaultVector),
                GetSaneValue(second, type, defaultVector),
                lerpFactor
            );
        }

        public static void AltFaceUpdate(this ChaControl chaControl)
        {
            var sliders = chaControl.fileCustom.face.shapeValueFace;
            for (var i = 0; i < sliders.Length; i++) chaControl.sibFace.ChangeValue(i, sliders[i]);
            chaControl.sibFace.Update();
        }

        public static void AltBodyUpdate(this ChaControl chaControl)
        {
            var sliders = chaControl.fileCustom.body.shapeValueBody;
            for (var i = 0; i < sliders.Length; i++) chaControl.sibBody.ChangeValue(i, sliders[i]);
            chaControl.sibBody.Update();
        }

        public static float GetInterpolatedFactor(float x, float y, float factor)
        {
            return Mathf.LerpUnclamped(x, y, factor);
        }

        public static bool IsInRange(int i, int a, int b) => i >= a && i <= b;

        public static float LerpValue(IReadOnlyList<CharacterData.ABMXValues> values, float lerpFactor)
        {
            if (values == null) return 1;
            if (values.Count < 2) return 1;
            var first = values[0];
            var second = values[1];
            if (first == null && second == null) return 1;

            return Mathf.Lerp(
                first?.RelativePosition ?? 1,
                second?.RelativePosition ?? 1,
                lerpFactor
            );
        }

        public static string PickName(IReadOnlyList<CharacterData.ABMXValues> values)
        {
            if (values == null) return "";
            if (values.Count < 2) return "";
            var first = values[0];
            var second = values[1];
            if (first == null && second == null) return "";
            return first == null || first.Name.IsNullOrWhiteSpace() ? second.Name : first.Name;
        }
    }
}
