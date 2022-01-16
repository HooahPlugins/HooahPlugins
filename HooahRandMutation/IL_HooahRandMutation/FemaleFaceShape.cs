using System;
using System.Collections.Generic;
using System.Linq;
using AIChara;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HooahRandMutation.IL_HooahRandMutation
{
    public static class ShapeUtility
    {
        public static void Mutate(this ChaControl chara, float val)
        {
            chara.Mutate(-val, val);
        }

        public static void Mutate(this ChaControl chara, float min, float max)
        {
            chara.fileCustom.face.shapeValueFace = InterpolateShapeUtility.Templates[0].HeadSliders
                .Select(x => x + Random.Range(min, max)).ToArray();
        }

        public static void MutateRange(this ChaControl chara, int min, int max, float tVal)
        {
            MutateRange(chara, min, max, -tVal, tVal);
        }

        public static void MutateRange(this ChaControl chara, int min, int max, float tMin, float tMax)
        {
            chara.fileCustom.face.shapeValueFace = InterpolateShapeUtility.Templates[0].HeadSliders
                .Select((x, i) => i >= min && i <= max ? x + Random.Range(tMin, tMax) : x).ToArray();
        }

        public static void MutateHead(this ChaControl chara, float val) => chara.MutateRange(0, 4, val);
        public static void MutateChin(this ChaControl chara, float val) => chara.MutateRange(5, 12, val);
        public static void MutateCheek(this ChaControl chara, float val) => chara.MutateRange(13, 18, val);
        public static void MutateEyes(this ChaControl chara, float val) => chara.MutateRange(19, 31, val);
        public static void MutateNose(this ChaControl chara, float val) => chara.MutateRange(32, 46, val);
        public static void MutateMouth(this ChaControl chara, float val) => chara.MutateRange(47, 53, val);
        public static void MutateEar(this ChaControl chara, float val) => chara.MutateRange(54, 58, val);

        static bool IsInRange(int i, int a, int b) => i >= a && i <= b;

        public static void MutateRangeCombined(this ChaControl chara, float head, float chin, float cheek, float eyes,
            float eyeAng,
            float nose, float mouth, float ear)
        {
            var template = InterpolateShapeUtility.Templates[0].HeadSliders;
            chara.fileCustom.face.shapeValueFace = template.Select((x, i) =>
            {
                if (IsInRange(i, 0, 4)) return x + Random.Range(-head, head);
                if (IsInRange(i, 5, 12)) return x + Random.Range(-chin, chin);
                if (IsInRange(i, 13, 18)) return x + Random.Range(-cheek, cheek);
                // bruh...
                if (IsInRange(i, 19, 23)) return x + Random.Range(-eyes, eyes);
                if (IsInRange(i, 24, 25)) return x + Random.Range(-eyeAng, eyeAng);
                if (IsInRange(i, 26, 31)) return x + Random.Range(-eyes, eyes);
                // aahhh
                if (IsInRange(i, 32, 46)) return x + Random.Range(-nose, nose);
                if (IsInRange(i, 47, 53)) return x + Random.Range(-mouth, mouth);
                if (IsInRange(i, 54, 58)) return x + Random.Range(-ear, ear);
                return x;
            }).ToArray();
        }

        private static float GetInterpolatedFactor(float x, float y, float strength, bool useFactor, float factor)
        {
            var m = useFactor ? factor : Random.Range(0, 1);
            return Mathf.LerpUnclamped(x, y, m * strength);
        }

        public static void InterpolateTwoSliders(this ChaControl chara, float head, float chin, float cheek, float eyes,
            float eyeAng,
            float nose, float mouth, float ear, bool interpolate = false, float factor = 0)
        {
            var nodeA = InterpolateShapeUtility.Templates[0].HeadSliders;
            var nodeB = InterpolateShapeUtility.Templates[1].HeadSliders;

            chara.fileCustom.face.shapeValueFace = nodeA.Select((x, i) =>
            {
                var y = nodeB[i];

                if (IsInRange(i, 0, 4)) return GetInterpolatedFactor(x, y, head, interpolate, factor);
                if (IsInRange(i, 5, 12)) return GetInterpolatedFactor(x, y, chin, interpolate, factor);
                if (IsInRange(i, 13, 18)) return GetInterpolatedFactor(x, y, cheek, interpolate, factor);
                // bruh...
                if (IsInRange(i, 19, 23)) return GetInterpolatedFactor(x, y, eyes, interpolate, factor);
                if (IsInRange(i, 24, 25)) return GetInterpolatedFactor(x, y, eyeAng, interpolate, factor);
                if (IsInRange(i, 26, 31)) return GetInterpolatedFactor(x, y, eyes, interpolate, factor);
                // aahhh
                if (IsInRange(i, 32, 46)) return GetInterpolatedFactor(x, y, nose, interpolate, factor);
                if (IsInRange(i, 47, 53)) return GetInterpolatedFactor(x, y, mouth, interpolate, factor);
                if (IsInRange(i, 54, 58)) return GetInterpolatedFactor(x, y, ear, interpolate, factor);
                return x;
            }).ToArray();
        }
    }
}
