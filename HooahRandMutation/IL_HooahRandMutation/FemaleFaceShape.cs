﻿using System;
using System.Collections.Generic;
using System.Linq;
using AIChara;
using Random = UnityEngine.Random;

namespace HooahRandMutation.IL_HooahRandMutation
{
    public static class ShapeUtility
    {
        private static float[] _template;

        public static void Remember(this ChaControl chara)
        {
            _template = chara.fileCustom.face.shapeValueFace.Select(x => x).ToArray();
        }

        public static void Mutate(this ChaControl chara, float val)
        {
            chara.Mutate(-val, val);
        }

        public static void Mutate(this ChaControl chara, float min, float max)
        {
            chara.fileCustom.face.shapeValueFace = _template.Select(x => x + Random.Range(min, max)).ToArray();
        }

        public static void MutateRange(this ChaControl chara, int min, int max, float tVal)
        {
            MutateRange(chara, min, max, -tVal, tVal);
        }

        public static void MutateRange(this ChaControl chara, int min, int max, float tMin, float tMax)
        {
            chara.fileCustom.face.shapeValueFace = _template
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

        public static void MutateRangeCombined(this ChaControl chara, float head, float chin, float cheek, float eyes, float eyeAng,
            float nose, float mouth, float ear)
        {
            chara.fileCustom.face.shapeValueFace = _template.Select((x, i) =>
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
    }
}