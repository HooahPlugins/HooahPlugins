using System.Collections.Generic;
using System.Linq;
using AIChara;
using Illusion.Extensions;
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
            var faceSliders = control.fileCustom.face.shapeValueFace;
            var bodySliders = control.fileCustom.body.shapeValueBody;
            var faceSlidersCopy = new float[faceSliders.Length];
            var bodySlidersCopy = new float[bodySliders.Length];
            faceSliders.CopyTo(faceSlidersCopy, 0);
            bodySliders.CopyTo(bodySlidersCopy, 0);

            var apiController = control.GetComponent<BoneController>();
            return new CharacterData.CharacterSliders
            {
                CharacterName = control.fileParam.fullname,
                Version = 1, // just in case.
                HeadSliders = faceSlidersCopy,
                BodySliders = bodySlidersCopy,
                BodyBreastSoft = control.fileCustom.body.bustSoftness,
                BodyBreastWeight = control.fileCustom.body.bustWeight,
                AbmxValuesMap = apiController == null
                    ? new Dictionary<string, CharacterData.ABMXValues>()
                    : apiController.Modifiers.ToDictionary(x => x.BoneName, GetValueFromModifier)
            };
        }
    }
}
