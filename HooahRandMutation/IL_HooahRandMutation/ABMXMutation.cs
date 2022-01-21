using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AIChara;
using KKABMX.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HooahRandMutation.IL_HooahRandMutation
{
    public static class ABMXMutation
    {
        // todo: resolve this abomination
        //       ui? or just load from textfile?
        //       anyway in any method, gotta make this customizable
        //       and persistent.
        public static readonly HashSet<string> HorizontalFix = new HashSet<string>()
        {
            "p_cf_head_bone", "cf_J_FaceRoot", "cf_J_FaceBase", "cf_J_FaceLowBase", "cf_J_FaceLow_s",
            "cf_J_Chin_rs", "cf_J_ChinTip_s", "cf_J_ChinLow", "cf_J_MouthBase_tr",
            "N_Mouth", "cf_J_MouthMove", "cf_J_MouthLow", "cf_J_Mouthup",
            "cf_J_MouthBase_s", "cf_J_MouthCavity", "cf_J_FaceUp_ty",
            "cf_J_FaceUp_tz", "N_Head", "N_Head_top", "cf_J_NoseBase_trs", "cf_J_NoseBase_s",
            "cf_J_Nose_r", "cf_J_Nose_t", "cf_J_Nose_tip", "N_Nose",
            "cf_J_NoseBridge_t", "cf_J_megane", "N_Megane",
            "cf_J_NoseBridge_s", "N_Face", "cf_J_FaceRoot_s",
            "ct_head", "o_eyelashes", "o_eyeshadow", "o_head", "o_namida", "o_tang", "o_tooth"
        };

        public static readonly HashSet<string> HeadBoneNames = new HashSet<string>()
        {
            "p_cf_head_bone", "cf_J_FaceRoot", "cf_J_FaceBase", "cf_J_FaceLowBase", "cf_J_FaceLow_s", "cf_J_CheekLow_L",
            "cf_J_CheekUp_L", "cf_J_CheekUp_R", "cf_J_Chin_rs", "cf_J_ChinTip_s", "cf_J_ChinLow", "cf_J_MouthBase_tr",
            "N_Mouth", "cf_J_MouthMove", "cf_J_Mouth_L", "cf_J_Mouth_R", "cf_J_MouthLow", "cf_J_Mouthup",
            "cf_J_MouthBase_s", "cf_J_CheekLow_R", "cf_J_MouthCavity", "cf_J_FaceUp_ty",
            "cf_J_EarBase_s_L", "cf_J_EarLow_L", "cf_J_EarRing_L", "N_Earring_L",
            "cf_J_EarUp_L", "cf_J_EarBase_s_R", "cf_J_EarLow_R", "cf_J_EarRing_R",
            "N_Earring_R", "cf_J_EarUp_R", "cf_J_FaceUp_tz", "cf_J_Eye_t_L",
            "cf_J_Eye_s_L", "cf_J_Eye_r_L", "cf_J_Eye01_L", "cf_J_Eye01_s_L",
            "cf_J_Eye02_L", "cf_J_Eye02_s_L", "cf_J_Eye03_L", "cf_J_Eye03_s_L",
            "cf_J_Eye04_L", "cf_J_Eye04_s_L", "cf_J_EyePos_rz_L", "cf_J_look_L",
            "cf_J_eye_rs_L", "cf_J_pupil_s_L", "cf_J_Eye_t_R", "cf_J_Eye_s_R",
            "cf_J_Eye_r_R", "cf_J_Eye01_R", "cf_J_Eye01_s_R", "cf_J_Eye02_R",
            "cf_J_Eye02_s_R", "cf_J_Eye03_R", "cf_J_Eye03_s_R", "cf_J_Eye04_R",
            "cf_J_Eye04_s_R", "cf_J_EyePos_rz_R", "cf_J_look_R", "cf_J_eye_rs_R",
            "cf_J_pupil_s_R", "cf_J_Mayu_L", "cf_J_MayuMid_s_L", "cf_J_MayuTip_s_L",
            "cf_J_Mayu_R", "cf_J_MayuMid_s_R", "cf_J_MayuTip_s_R", "N_Hitai",
            "N_Head", "N_Head_top", "cf_J_NoseBase_trs", "cf_J_NoseBase_s", "cf_J_Neck",
            "cf_J_Nose_r", "cf_J_Nose_t", "cf_J_Nose_tip", "N_Nose",
            "cf_J_NoseWing_tx_L", "cf_J_NoseWing_tx_R", "cf_J_NoseBridge_t", "cf_J_megane",
            "N_Megane", "cf_J_NoseBridge_s", "N_Face", "cf_J_FaceRoot_s",
            "ct_head", "o_eyebase_L", "o_eyebase_R", "o_eyelashes",
            "o_eyeshadow", "o_head", "o_namida", "o_tang", "o_tooth"
        };

        public static void SetTemplate(this ChaControl control, int index = 0)
        {
            InterpolateShapeUtility.Templates[index] = control.GetCharacterSnapshot();
        }

        public static void RandomizeABMX(this ChaControl control,
            float pos, float ang,
            float scale, float length,
            bool useAbsolute = false,
            HashSet<string> filters = null
        )
        {
            control.RandomizeAbmx(
                -pos, pos,
                -ang, ang,
                -scale, scale,
                -length, length,
                useAbsolute,
                filters
            );
        }

        public static Vector3 RandomVector(float scale)
        {
            return new Vector3(
                Random.Range(-scale, scale),
                Random.Range(-scale, scale),
                Random.Range(-scale, scale)
            );
        }

        public static readonly Regex ptn = new Regex("^(.+_)([lr])($|_.+$)", RegexOptions.IgnoreCase);

        public static readonly Dictionary<string, string> invertDictionary = new Dictionary<string, string>()
        {
            { "l", "r" },
            { "r", "l" },
            { "L", "R" },
            { "R", "L" },
        };

        public static readonly Vector3 HFixVector = Vector3.one - Vector3.right;

        public static Vector3 ScaleAndReturn(this Vector3 inVector, Vector3 scale)
        {
            inVector.Scale(scale);
            return inVector;
        }

        public static void RandomizeAbmx(this ChaControl control,
            float minPos, float maxPos, float minAng, float maxAng,
            float minScale, float maxScale, float minLength, float maxLength,
            bool useAbsolute = false,
            HashSet<string> filters = null)
        {
            var controller = control.GetComponent<BoneController>();
            if (controller == null) return;
            var template = InterpolateShapeUtility.Templates[0];
            foreach (var mdf in controller.Modifiers) mdf.Reset();
            controller.Modifiers.Clear();

            var mirrorModifiers = new Dictionary<string, BoneModifier>();

            foreach (var x in template.AbmxValuesMap.Values)
            {
                var m = ptn.Match(x.Name);
                if (m.Success && mirrorModifiers.TryGetValue(x.Name, out var mirrorModifier))
                {
                    var mdfData = mirrorModifier.CoordinateModifiers.FirstOrDefault();
                    var flippedModifier = new BoneModifier(x.Name, new[]
                    {
                        new BoneModifierData
                        {
                            ScaleModifier = mdfData.ScaleModifier,
                            LengthModifier = mdfData.LengthModifier,
                            PositionModifier = (mdfData.PositionModifier * 1).ScaleAndReturn(new Vector3(-1, 1, 1)),
                            RotationModifier = Quaternion.Inverse(Quaternion.Euler(mdfData.RotationModifier))
                                .eulerAngles
                        }
                    });
                    controller.AddModifier(flippedModifier);
                    continue;
                }

                var modifier = filters == null || filters.Contains(x.Name)
                    ? new BoneModifier(x.Name, new[]
                    {
                        new BoneModifierData
                        {
                            ScaleModifier = x.Scale == Vector3.one
                                ? x.Scale
                                : x.Scale + RandomVector(useAbsolute ? 1 : x.Scale.magnitude) * maxScale,
                            LengthModifier = Math.Abs(x.RelativePosition - 1.0f) < 0.00001f
                                ? x.RelativePosition
                                : x.RelativePosition
                                  + (useAbsolute ? 1 : x.RelativePosition) * Random.Range(minLength, maxLength),
                            PositionModifier = x.Position == Vector3.zero
                                ? x.Position
                                : HorizontalFix.Contains(x.Name)
                                    ? (x.Position + RandomVector(useAbsolute ? 1 : x.Position.magnitude) * maxPos)
                                    .ScaleAndReturn(HFixVector)
                                    : x.Position + RandomVector(useAbsolute ? 1 : x.Position.magnitude) * maxPos,
                            RotationModifier = x.VectorAngle == Vector3.zero
                                ? x.VectorAngle
                                : x.VectorAngle + RandomVector(useAbsolute ? 1 : x.VectorAngle.magnitude) * maxAng
                        }
                    })
                    : new BoneModifier(x.Name, new[]
                        {
                            new BoneModifierData
                            {
                                ScaleModifier = x.Scale,
                                LengthModifier = x.RelativePosition,
                                PositionModifier = x.Position,
                                RotationModifier = x.VectorAngle
                            }
                        }
                    );
                controller.AddModifier(modifier);

                if (!m.Success || m.Groups.Count < 4) continue;
                var pos = invertDictionary.TryGetValue(m.Groups[2].Value, out var a) ? a : m.Captures[1].Value;
                mirrorModifiers.Add($"{m.Groups[1].Value}{pos}{m.Groups[3].Value}", modifier);
            }
        }

        public static void ApplyAbmxValues(this ChaControl control,
            in Dictionary<string, InterpolateShapeUtility.ABMXValues> valuesMap)
        {
            var controller = control.GetComponent<BoneController>();
            if (controller == null) return;
            foreach (var mdf in controller.Modifiers) mdf.Reset();
            controller.Modifiers.Clear();
            var mirrorModifiers = new Dictionary<string, BoneModifier>();

            foreach (var x in valuesMap.Values)
            {
                var m = ptn.Match(x.Name);
                if (m.Success && mirrorModifiers.TryGetValue(x.Name, out var mirrorModifier))
                {
                    var mdfData = mirrorModifier.CoordinateModifiers.FirstOrDefault();
                    if (mdfData == null) continue;
                    var flippedModifier = new BoneModifier(x.Name, new[]
                    {
                        new BoneModifierData
                        {
                            ScaleModifier = mdfData.ScaleModifier,
                            LengthModifier = mdfData.LengthModifier,
                            PositionModifier = (mdfData.PositionModifier * 1).ScaleAndReturn(new Vector3(-1, 1, 1)),
                            RotationModifier = Quaternion.Inverse(Quaternion.Euler(mdfData.RotationModifier))
                                .eulerAngles
                        }
                    });
                    controller.AddModifier(flippedModifier);
                    continue;
                }

                var modifier = new BoneModifier(x.Name, new[]
                {
                    new BoneModifierData
                    {
                        ScaleModifier = x.Scale,
                        LengthModifier = x.RelativePosition,
                        PositionModifier = x.Position,
                        RotationModifier = x.VectorAngle
                    }
                });
                controller.AddModifier(modifier);

                if (!m.Success || m.Groups.Count < 4) continue;
                var pos = invertDictionary.TryGetValue(m.Groups[2].Value, out var a) ? a : m.Captures[1].Value;
                mirrorModifiers.Add($"{m.Groups[1].Value}{pos}{m.Groups[3].Value}", modifier);
            }
        }
    }
}
