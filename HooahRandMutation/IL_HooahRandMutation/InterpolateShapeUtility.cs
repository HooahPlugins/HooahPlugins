using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ADV.Commands.Effect;
using AIChara;
using KKABMX.Core;
using KKAPI.Utilities;
using MessagePack;
using Sirenix.Serialization;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HooahRandMutation.IL_HooahRandMutation
{
    public static class InterpolateShapeUtility
    {
        [MessagePackObject]
        public class ABMXValues
        {
            [Key("name")] public string Name;
            [Key("scale")] public Vector3 Scale;
            [Key("pos")] public Vector3 Position;
            [Key("ang")] public Vector3 VectorAngle;
            [Key("len")] public float RelativePosition;
        }

        [MessagePackObject]
        public struct CharacterSliders
        {
            [Key("version")] public int Version;
            [Key("name")] public string CharacterName;
            [Key("head")] public float[] HeadSliders;
            [Key("body")] public float[] BodySliders;
            [Key("abmx")] public Dictionary<string, ABMXValues> AbmxValuesMap;

            private static string GetDir() => Path.Combine(Application.dataPath, @"..\userdata\mutator");

            public void Save()
            {
                // make sure the target directory exists
                if (!Directory.Exists(GetDir())) Directory.CreateDirectory(GetDir());
                File.WriteAllBytes(
                    Path.Combine(GetDir(), $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}_{CharacterName}.preset"),
                    MessagePackSerializer.Serialize(this));
            }

            public const string Filter = "Json File (*.preset)|*.preset|All files|*.*";
            public const string FileExtension = ".preset";


            public static void TryLoad(Action<CharacterSliders> callback)
            {
                void OnAccept(string[] strings)
                {
                    var path = strings.FirstOrDefault();
                    if (path == null || path.IsNullOrWhiteSpace()) return;
                    try
                    {
                        // todo: accept json....
                        var jsonPayload = File.ReadAllBytes(path);
                        var result = MessagePackSerializer.Deserialize<CharacterSliders>(jsonPayload);
                        // todo: find strange cases.
                        callback(result);
                    }
                    catch (Exception e)
                    {
                        // todo: print message, failed to load json payload
                        // specify the reason in the message.
                    }
                }

                OpenFileDialog.Show(OnAccept, "Select Character Slider File", GetDir(), Filter, FileExtension);
            }
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
                CharacterName = control.fileParam.fullname,
                Version = 1, // just in case.
                HeadSliders = control.fileCustom.face.shapeValueFace.Select(x => x).ToArray(),
                BodySliders =
                    control.fileCustom.body.shapeValueBody.Select(x => x)
                        .ToArray(), // to copy the array. lmk if there is more better way.
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
