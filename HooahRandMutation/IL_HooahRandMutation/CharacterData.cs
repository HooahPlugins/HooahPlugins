using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AIChara;
using KKABMX.Core;
using KKAPI.Utilities;
using MessagePack;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HooahRandMutation
{
    /// <summary>
    /// A static class for storing mutation data.
    /// </summary>
    public static class CharacterData
    {
        [MessagePackObject]
        public struct CharacterSliders
        {
            [Key("version")] public int Version;
            [Key("name")] public string CharacterName;
            [Key("head")] public float[] HeadSliders;
            [Key("body")] public float[] BodySliders;
            [Key("body-soft")] public float BodyBreastSoft;
            [Key("body-weight")] public float BodyBreastWeight;
            [Key("abmx")] public Dictionary<string, ABMXValues> AbmxValuesMap;

            // windows. smh.
            private static string GetDir() =>
                Path.Combine(Application.dataPath, @"..\userdata\mutator\").Replace("/", "\\");

            public void Save()
            {
                // make sure the target directory exists
                if (!Directory.Exists(GetDir())) Directory.CreateDirectory(GetDir());
                File.WriteAllBytes(
                    Path.Combine(GetDir(),
                        $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}_{CharacterName}.{FileExtension}"),
                    MessagePackSerializer.Serialize(this));
            }

            public static readonly string Filter =
                $"Slide Preset File (*{FileExtension})|*{FileExtension}|All files|*.*";

            public const string FileExtension = ".silders";

            public static void OpenSlidePresetFolder()
            {
                // sorry, I don't run this game in linux.
                Process.Start("explorer.exe", $"/select, \"{GetDir()}\"");
            }

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

            #region Randomizer Features

            public void RandomizeBodySliders(
                ChaControl control,
                float body, float head, float breasts, float torso, float pelvis, float arm, float leg
            )
            {
                control.fileCustom.body.bustSoftness = BodyBreastSoft + Random.Range(-breasts, breasts);
                control.fileCustom.body.bustWeight = BodyBreastWeight + Random.Range(-breasts, breasts);
                control.fileCustom.body.shapeValueBody =
                    GetRandomBodySliders(body, head, breasts, torso, pelvis, arm, leg);
                control.AltBodyUpdate();
            }

            public float[] GetRandomBodySliders(
                float body, float head, float breasts, float torso, float pelvis, float arm, float leg
            )
            {
                return BodySliders.Select((x, i) =>
                {
                    if (i == 0) return x + Random.Range(-body, body);
                    if (Utility.IsInRange(i, 1, 8) || i == 32) return x + Random.Range(-breasts, breasts);
                    if (i == 9) return x + Random.Range(-head, head);
                    if (Utility.IsInRange(i, 10, 17)) return x + Random.Range(-torso, torso);
                    if (Utility.IsInRange(i, 18, 24)) return x + Random.Range(-pelvis, pelvis);
                    if (Utility.IsInRange(i, 25, 28)) return x + Random.Range(-leg, leg);
                    if (Utility.IsInRange(i, 29, 31)) return x + Random.Range(-arm, arm);
                    return x;
                }).ToArray();
            }

            public float[] GetRandomizedHeadSliders(
                float head, float chin, float cheek, float eyes, float eyeAng, float nose, float mouth, float ear
            )
            {
                return HeadSliders.Select((x, i) =>
                {
                    if (Utility.IsInRange(i, 0, 4)) return x + Random.Range(-head, head);
                    if (Utility.IsInRange(i, 5, 12)) return x + Random.Range(-chin, chin);
                    if (Utility.IsInRange(i, 13, 18)) return x + Random.Range(-cheek, cheek);
                    // bruh...
                    if (Utility.IsInRange(i, 19, 23)) return x + Random.Range(-eyes, eyes);
                    if (Utility.IsInRange(i, 24, 25)) return x + Random.Range(-eyeAng, eyeAng);
                    if (Utility.IsInRange(i, 26, 31)) return x + Random.Range(-eyes, eyes);
                    // aahhh
                    if (Utility.IsInRange(i, 32, 46)) return x + Random.Range(-nose, nose);
                    if (Utility.IsInRange(i, 47, 53)) return x + Random.Range(-mouth, mouth);
                    if (Utility.IsInRange(i, 54, 58)) return x + Random.Range(-ear, ear);
                    return x;
                }).ToArray();
            }

            public void RandomizeHeadSliders(
                ChaControl control,
                float head, float chin, float cheek, float eyes, float eyeAng, float nose, float mouth, float ear
            )
            {
                control.fileCustom.face.shapeValueFace =
                    GetRandomizedHeadSliders(head, chin, cheek, eyes, eyeAng, nose, mouth, ear);
                control.AltFaceUpdate();
            }

            public static Vector3 RandomVector(float scale)
            {
                return new Vector3(
                    Random.Range(-scale, scale),
                    Random.Range(-scale, scale),
                    Random.Range(-scale, scale)
                );
            }

            public static readonly Vector3 HFixVector = Vector3.one - Vector3.right;

            public void RandomizeAbmx(ChaControl control,
                float maxPos, float maxAng, float maxScale, float maxLength,
                bool useAbsolute = false, HashSet<string> filters = null, bool inverted = false,
                bool fullUpdate = false)

            {
                var controller = control.GetComponent<BoneController>();
                if (controller == null) return;
                controller.UpdateModifiers(AbmxValuesMap, true, x =>
                (
                    inverted
                        ? !(filters == null || filters.Contains(x.Name))
                        : filters == null || filters.Contains(x.Name)
                )
                    ? new BoneModifierData
                    {
                        ScaleModifier = x.Scale == Vector3.one
                            ? x.Scale
                            : x.Scale + RandomVector(useAbsolute ? 1 : x.Scale.magnitude) * maxScale,
                        LengthModifier = Math.Abs(x.RelativePosition - 1.0f) < 0.001f
                            ? x.RelativePosition
                            : x.RelativePosition
                              + (useAbsolute ? 1 : x.RelativePosition) * Random.Range(-maxLength, maxLength),
                        PositionModifier = x.Position == Vector3.zero
                            ? x.Position
                            : Constant.HorizontalFix.Contains(x.Name)
                                ? (x.Position + RandomVector(useAbsolute ? 1 : x.Position.magnitude) * maxPos)
                                .ScaleAndReturn(HFixVector)
                                : x.Position + RandomVector(useAbsolute ? 1 : x.Position.magnitude) * maxPos,
                        RotationModifier = x.VectorAngle == Vector3.zero
                            ? x.VectorAngle
                            : x.VectorAngle + RandomVector(useAbsolute ? 1 : x.VectorAngle.magnitude) * maxAng
                    }
                    : new BoneModifierData
                    {
                        ScaleModifier = x.Scale,
                        LengthModifier = x.RelativePosition,
                        PositionModifier = x.Position,
                        RotationModifier = x.VectorAngle
                    }, fullUpdate);
            }

            #endregion
        }

        [MessagePackObject]
        public class ABMXValues
        {
            [Key("name")] public string Name;
            [Key("scale")] public Vector3 Scale;
            [Key("pos")] public Vector3 Position;
            [Key("ang")] public Vector3 VectorAngle;
            [Key("len")] public float RelativePosition;
        }

        #region Template Assignment And Merge

        // todo: create "shape" feature
        public static CharacterSliders[] Templates = new CharacterSliders[2];

        #endregion

        #region Randomizer Undo/Redo Mechanism

        // this is only for the randomizer!
        public static List<CharacterSliders> UndoBuffer = new List<CharacterSliders>(10);
        public static List<CharacterSliders> RedoBuffer = new List<CharacterSliders>(10);

        public static void Push(ChaControl chaControl)
        {
            var cnt = UndoBuffer.Count;
            if (cnt == UndoBuffer.Capacity) UndoBuffer.RemoveAt(cnt - 1);

            UndoBuffer.Add(chaControl.GetCharacterSnapshot());
            RedoBuffer.Clear();
        }

        public static bool Undo()
        {
            try
            {
                var tmp = UndoBuffer.ElementAt(0);
                UndoBuffer.RemoveAt(0);
                Templates[0] = tmp;
                RedoBuffer.Add(tmp);

                return true;
            }
            catch (InvalidOperationException)
            {
                // todo: undobuffer is empty
            }

            return false;
        }

        public static bool Redo()
        {
            try
            {
                var redo = RedoBuffer.ElementAt(0);
                RedoBuffer.RemoveAt(0);
                Templates[0] = redo;
                UndoBuffer.Add(redo);

                return true;
            }
            catch (InvalidOperationException)
            {
                // todo: undobuffer is empty
            }

            return false;
        }

        #endregion

        #region Static BoneController Methods

        public static void ClearModifiers(this BoneController controller)
        {
            if (controller == null) return;
            foreach (var modifier in controller.Modifiers) modifier.Reset();
            controller.Modifiers.Clear();
        }

        public static readonly Regex ptn = new Regex("^(.+_)([lr])($|_.+$)", RegexOptions.IgnoreCase);

        public static readonly Dictionary<string, string> invertDictionary = new Dictionary<string, string>()
        {
            { "l", "r" },
            { "r", "l" },
            { "L", "R" },
            { "R", "L" },
        };

        public static BoneModifierData GetMirroredBoneModiferData(BoneModifierData mdfData)
        {
            return new BoneModifierData
            {
                ScaleModifier = mdfData.ScaleModifier,
                LengthModifier = mdfData.LengthModifier,
                PositionModifier = (mdfData.PositionModifier * 1).ScaleAndReturn(new Vector3(-1, 1, 1)),
                RotationModifier = (mdfData.RotationModifier * 1).ScaleAndReturn(new Vector3(1, -1, -1))
            };
        }

        public static BoneModifierData GetIdentityModifierData(ABMXValues x)
        {
            return new BoneModifierData
            {
                ScaleModifier = x.Scale,
                LengthModifier = x.RelativePosition,
                PositionModifier = x.Position,
                RotationModifier = x.VectorAngle
            };
        }

        // todo: modular?
        public static void UpdateModifiers(this BoneController controller,
            in Dictionary<string, ABMXValues> valuesMap,
            bool mirrored, Func<ABMXValues, BoneModifierData> generate, bool fullUpdate = false)
        {
            controller.ClearModifiers();
            var mirrorModifiers = mirrored ? new Dictionary<string, BoneModifier>() : null;

            foreach (var x in valuesMap.Values)
            {
                Match m = null;
                if (mirrored)
                {
                    m = ptn.Match(x.Name);
                    if (m.Success && mirrorModifiers.TryGetValue(x.Name, out var mirrorModifier))
                    {
                        var mdfData = mirrorModifier.CoordinateModifiers.FirstOrDefault();
                        if (mdfData == null) continue;
                        var flippedModifier = new BoneModifier(x.Name, new[] { GetMirroredBoneModiferData(mdfData) });
                        controller.Modifiers.Add(flippedModifier);
                        continue;
                    }
                }

                var modifier = new BoneModifier(x.Name, new[] { generate(x) });
                controller.Modifiers.Add(modifier);

                if (!mirrored) continue;
                if (!m.Success || m.Groups.Count < 4) continue;
                var pos = invertDictionary.TryGetValue(m.Groups[2].Value, out var a) ? a : m.Captures[1].Value;
                mirrorModifiers.Add($"{m.Groups[1].Value}{pos}{m.Groups[3].Value}", modifier);
            }

            if (fullUpdate) controller.NeedsFullRefresh = true;
            else controller.NeedsBaselineUpdate = true;
        }

        public static void UpdateModifiers(this BoneController controller, in Dictionary<string, ABMXValues> valuesMap,
            bool fullUpdate = false)
        {
            controller.UpdateModifiers(valuesMap, true, GetIdentityModifierData, fullUpdate);
        }

        #endregion

        #region Static Character Controller Methods

        public static void ApplySliders(this ChaControl chaControl, CharacterSliders sliders)
        {
            var controller = chaControl.GetComponent<BoneController>();
            controller.enabled = false;
            chaControl.fileCustom.face.shapeValueFace = sliders.HeadSliders;
            chaControl.fileCustom.body.shapeValueBody = sliders.BodySliders;
            chaControl.AltFaceUpdate();
            chaControl.AltBodyUpdate();
            controller.enabled = true;

            Observable.NextFrame(FrameCountType.EndOfFrame).Take(1)
                .Subscribe(_ => controller.UpdateModifiers(sliders.AbmxValuesMap));
        }

        public static void UpdateAbmx(this ChaControl control, in Dictionary<string, ABMXValues> processedMaps,
            bool fullUpdate = false)
        {
            var controller = control.GetComponent<BoneController>();
            if (controller == null) return;
            controller.UpdateModifiers(processedMaps, fullUpdate);
        }

        #endregion

        #region Interpolation Methods

        public static void InterpolateFaceSliders(this ChaControl control, float min, float max, float median,
            float range, bool uniformFactor = false, float factor = 0)
        {
            var nodeA = Templates[0].HeadSliders;
            var nodeB = Templates[1].HeadSliders;
            var array = new float[nodeA.Length];

            for (var i = 0; i < array.Length; i++)
                array[i] = Utility.GetInterpolatedFactor(nodeA[i], nodeB[i],
                    uniformFactor ? factor : Utility.GetRandomNumber(min, max, median, range));

            control.fileCustom.face.shapeValueFace = array;
        }

        public static void InterpolateBodySliders(this ChaControl control, float min, float max, float median,
            float range, bool uniformFactor = false, float factor = 0)
        {
            var tA = Templates.ElementAtOrDefault(0);
            var tB = Templates.ElementAtOrDefault(1);
            var nodeA = tA.BodySliders;
            var nodeB = tB.BodySliders;

            for (var i = 0; i < control.fileCustom.body.shapeValueBody.Length; i++)
                control.fileCustom.body.shapeValueBody[i] = Utility.GetInterpolatedFactor(nodeA[i], nodeB[i],
                    uniformFactor ? factor : Utility.GetRandomNumber(min, max, median, range));
            control.fileCustom.body.bustSoftness =
                Utility.GetInterpolatedFactor(tA.BodyBreastSoft, tB.BodyBreastSoft,
                    uniformFactor ? factor : Utility.GetRandomNumber(min, max, median, range));
            control.fileCustom.body.bustWeight =
                Utility.GetInterpolatedFactor(tA.BodyBreastWeight, tB.BodyBreastWeight,
                    uniformFactor ? factor : Utility.GetRandomNumber(min, max, median, range));
        }

        // todo: one to one interpolation
        // todo: one to many lerp interpolation.
        public static void InterpolateAbmx(this ChaControl control,
            int defaultIndex, HashSet<string> filters,
            float min, float max, float median, float range,
            bool uniformFactor = false, float factor = 0.5f, bool inverted = false,
            bool preventShifting = false,
            bool fullUpdate = false
        )
        {
            // todo: support multi shape
            var le = Math.Min(Templates.Length, 2);
            var dictMaps = new Dictionary<string, ABMXValues[]>();
            var processedMaps = new Dictionary<string, ABMXValues>();

            // assign all references just in case ...
            // for when you want to mix more than 2 characters.
            for (var i = 0; i < le; i++)
            {
                foreach (var kv in Templates[i].AbmxValuesMap)
                {
                    if (dictMaps.TryGetValue(kv.Key, out var arr)) arr[i] = kv.Value;
                    else
                    {
                        dictMaps[kv.Key] = new ABMXValues[le];
                        dictMaps[kv.Key][i] = kv.Value;
                    }
                }
            }

            // because im lazy as fuck
            var rnd = new Func<float>(() => Utility.GetRandomNumber(min, max, median, range));
            foreach (var kv in dictMaps)
            {
                var filterTarget = inverted
                    ? !(filters != null && !filters.Contains(kv.Key))
                    : filters != null && !filters.Contains(kv.Key);
                var def = kv.Value[defaultIndex];

                // todo: this can be changed later to support multi point lerp
                // uniformFactor = use same lerp factor for all values.
                //          else = random factor for all
                var values = new ABMXValues
                {
                    Name = Utility.PickName(kv.Value),
                    Position = filterTarget
                        ? def?.Position ?? Vector3.zero
                        : Utility.LerpValue(kv.Value, Utility.ABMXValueType.Position, uniformFactor ? factor : rnd()),
                    Scale = filterTarget
                        ? def?.Scale ?? Vector3.one
                        : Utility.LerpValue(kv.Value, Utility.ABMXValueType.Scale, uniformFactor ? factor : rnd()),
                    VectorAngle = filterTarget
                        ? def?.VectorAngle ?? Vector3.zero
                        : Utility.LerpValue(kv.Value, Utility.ABMXValueType.Angle, uniformFactor ? factor : rnd()),
                    RelativePosition = preventShifting && ABMXMutation.BadDragons.Contains(kv.Key)
                        ? 1
                        : filterTarget
                            ? def?.RelativePosition ?? 1
                            : Utility.LerpValue(kv.Value, uniformFactor ? factor : rnd())
                };

                if (values.Position.magnitude <= 0.1 &&
                    values.VectorAngle.magnitude <= VectorOneTolerance &&
                    Mathf.Abs(values.Scale.magnitude - VectorOneTolerance) < 0.001 &&
                    Mathf.Abs(values.RelativePosition - 1) < 0.001) continue;
                if (Mathf.Abs(values.RelativePosition - 1) < 0.001) values.RelativePosition = 1; // stop.

                processedMaps[kv.Key] = values;
            }

            control.UpdateAbmx(processedMaps, fullUpdate);
        }

        private static readonly float VectorOneTolerance = Vector3.one.magnitude;

        #endregion
    }
}
