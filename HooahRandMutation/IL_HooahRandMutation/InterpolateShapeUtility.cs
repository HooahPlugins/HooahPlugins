using System;
using System.Collections.Generic;
using System.Linq;
using AIChara;
using KKABMX.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HooahRandMutation.IL_HooahRandMutation
{
    public static class InterpolateShapeUtility
    {
        public struct ABMXValues
        {
            public string Name;
            public Vector3 Scale;
            public Vector3 Position;
            public Vector3 VectorAngle;
            public float RelativePosition;
        }

        public struct CharacterSliders
        {
            public float[] HeadSliders;
            public float[] BodySliders;
            public Dictionary<string, ABMXValues> AbmxValuesMap;
        }

        public static CharacterSliders[] Templates = new CharacterSliders[2];

        static InterpolateShapeUtility()
        {
        }

        public static void Interpolate(this ChaControl control)
        {
            // First of all, change the face.
            // ensure that the update is done.
            // Lerp all abmx values after the process.
        }

        public static ABMXValues GetValueFromModifier(BoneModifier modifier)
        {
            var charModifier = modifier.CoordinateModifiers.FirstOrDefault();
            return new ABMXValues
            {
                Name = modifier.BoneName,
                Position = charModifier?.PositionModifier ?? Vector3.zero,
                Scale = charModifier?.ScaleModifier ?? Vector3.one,
                VectorAngle = charModifier?.RotationModifier ?? Vector3.zero,
                RelativePosition = charModifier?.LengthModifier ?? 1f
            };
        }

        public static CharacterSliders GetCharacterSnapshot(this ChaControl control)
        {
            var apiController = control.GetComponent<BoneController>();
            return new CharacterSliders
            {
                HeadSliders = control.fileCustom.face.shapeValueFace.Select(x => x).ToArray(),
                BodySliders = Array.Empty<float>(),
                AbmxValuesMap = apiController == null
                    ? new Dictionary<string, ABMXValues>()
                    : apiController.Modifiers.ToDictionary(x => x.BoneName, GetValueFromModifier)
            };
        }

        /*
         * INTERPOLATE CHARACTER PARAMETERS
         * LEFT ----------- RIGHT
         * minLeft
         * maxRight
         * median
         * deviation
         */
        public static void Interpolate(this ChaControl control, float deviation, float median, float min, float max)
        {
            var weight = Mathf.Min(Mathf.Max(median + Random.Range(-deviation, deviation), Mathf.Min(1, min)),
                Mathf.Max(0, max));

            // lerp the shape between first and last.
            control.fileCustom.face.shapeValueFace = Templates[0].HeadSliders
                .Select((x, i) => Mathf.Lerp(x, Templates[1].HeadSliders.ElementAtOrDefault(i), weight))
                .ToArray();
            control.UpdateShapeFaceValueFromCustomInfo();

            // list of head bones to interpolate
            var apiController = control.GetComponent<BoneController>();

            // get all combination of modifiers
            var set = new HashSet<string>();
            for (var i = 0; i <= 1; i++)
                foreach (var key in Templates[i].AbmxValuesMap.Keys.Where(key => !set.Contains(key)))
                    set.Add(key);

            apiController.Modifiers.Clear();
            foreach (var bone in set)
            {
                var left = Templates[0].AbmxValuesMap.TryGetValue(bone, out var leftAbmx);
                var right = Templates[1].AbmxValuesMap.TryGetValue(bone, out var rightAbmx);
                if (!left)
                    leftAbmx = new ABMXValues
                    {
                        Name = bone, Position = Vector3.zero, Scale = Vector3.one, RelativePosition = 1f,
                        VectorAngle = Vector3.zero
                    };
                if (!right)
                    rightAbmx = new ABMXValues
                    {
                        Name = bone, Position = Vector3.zero, Scale = Vector3.one, RelativePosition = 1f,
                        VectorAngle = Vector3.zero
                    };

                apiController.AddModifier(new BoneModifier(bone, new[]
                    {
                        new BoneModifierData
                        {
                            LengthModifier = Mathf.Lerp(leftAbmx.RelativePosition, rightAbmx.RelativePosition, weight),
                            PositionModifier = Vector3.Lerp(leftAbmx.Position, rightAbmx.Position, weight),
                            ScaleModifier = Vector3.Lerp(leftAbmx.Scale, rightAbmx.Scale, weight),
                            RotationModifier = Vector3.Lerp(leftAbmx.VectorAngle, rightAbmx.VectorAngle, weight),
                        }
                    })
                );
            }
        }
    }
}
