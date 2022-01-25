using System.Collections.Generic;
using System.Linq;
using AIChara;
using KKABMX.Core;
using UnityEngine;

namespace HooahRandMutation
{
    public static class InterpolateShapeUtility
    {

        public static CharacterData.ABMXValues GetValueFromModifier(BoneModifier modifier)
        {
            var charModifier = modifier.CoordinateModifiers.FirstOrDefault();
            return new CharacterData.ABMXValues
            {
                Name = modifier.BoneName,
                Position = charModifier?.PositionModifier ?? Vector3.zero,
                Scale = charModifier?.ScaleModifier ?? Vector3.one,
                VectorAngle = charModifier?.RotationModifier ?? Vector3.zero,
                RelativePosition = charModifier?.LengthModifier ?? 1f
            };
        }

        public static CharacterData.CharacterSliders GetCharacterSnapshot(this ChaControl control)
        {
            var apiController = control.GetComponent<BoneController>();
            return new CharacterData.CharacterSliders
            {
                CharacterName = control.fileParam.fullname,
                Version = 1, // just in case.
                HeadSliders = control.fileCustom.face.shapeValueFace.Select(x => x).ToArray(),
                BodySliders =
                    control.fileCustom.body.shapeValueBody.Select(x => x)
                        .ToArray(), // to copy the array. lmk if there is more better way.
                AbmxValuesMap = apiController == null
                    ? new Dictionary<string, CharacterData.ABMXValues>()
                    : apiController.Modifiers.ToDictionary(x => x.BoneName, GetValueFromModifier)
            };
        }
    }
}
