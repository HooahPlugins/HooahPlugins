using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if AI || HS2
using CharaUtils;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using AIChara;
using BepInEx.Logging;
using HarmonyLib;
using HooahComponents.Utility;
using JetBrains.Annotations;
using Studio;

#endif

public static class SkinnedAccessoryHook
{
#if AI || HS2
    private static CoroutineFields _fields;
    public static ManualLogSource Logger { get; set; }

    // ReSharper disable once InconsistentNaming
    public static void YeetOutHierarchy(OCIChar _ociChar, Transform _transformRoot,
        Dictionary<int, Info.BoneInfo> _dicBoneInfo)
    {
        foreach (var accessory in _ociChar.charInfo.cmpAccessory)
        {
            if (accessory == null) continue;
            var skinnedAccessory = accessory.GetComponent<SkinnedAccessory>();
            if (skinnedAccessory == null) continue;
            skinnedAccessory.skeleton.transform.parent = null;

            Object.Destroy(_ociChar.charInfo.objRoot.GetComponent(typeof(Expression)));
            _ociChar.charInfo.InitializeExpression(_ociChar.sex);
        }
    }

    private static Type _moreAccessoriesHookType;
    private static MethodInfo _moreAccessoriesGetCmpAccessoryMethodInfo;

    public static void RegisterHook()
    {
        var harmony = new Harmony("IL_HooahSkinnedAccessory");

        // To prevent accessory to slow down recursive hierarchy traversal.
        harmony.Patch(AccessTools.Method(typeof(AddObjectAssist), "InitBone"),
            new HarmonyMethod(typeof(SkinnedAccessoryHook), nameof(YeetOutHierarchy)));

        // Find Specific Coroutine Type with parameter.
        _fields = typeof(ChaControl)
            .GetNestedTypes(AccessTools.all)
            .Where(x => x.Name.StartsWith("<ChangeAccessoryAsync>"))
            .Select(x => new CoroutineFields(x)).FirstOrDefault(x => x.valid);

        if (_fields == null || !_fields.valid)
        {
#if DEBUG
            Logger.LogMessage(
                "Failed to find Accessory Initialization Coroutine! Aborting Skinned Accessory Hooking Procedure.");
#endif
            return;
        }

        harmony.Patch(AccessTools.Method(_fields.type, "MoveNext"), null,
            new HarmonyMethod(typeof(SkinnedAccessoryHook), nameof(RegisterQueue)));
#if DEBUG
        Logger.LogMessage("Successfully Hooked the Skinned Accessory Initializer.");
#endif

        // Hook More Accessories
        _moreAccessoriesHookType = AccessTools.TypeByName("MoreAccessoriesAI.Patches.ChaControl_Patches");
        if (_moreAccessoriesHookType != null)
        {
            _moreAccessoriesGetCmpAccessoryMethodInfo = AccessTools.Method(_moreAccessoriesHookType, "GetCmpAccessory");
            harmony.Patch(AccessTools.Method(_moreAccessoriesHookType, "ChangeAccessoryAsync_Prefix"), null,
                new HarmonyMethod(typeof(SkinnedAccessoryHook), nameof(ChangeAccessoryAsync_Prefix)));
#if DEBUG
            Logger.LogInfo("Hooked More Accessories");
#endif
        }
        else
        {
#if DEBUG
            Logger.LogInfo("More Accessories Not Found");
#endif
        }
    }


    private static void ProcessForSkinnedAccessory(ChaControl chaControl, CmpAccessory accessory)
    {
        // This exception is too common. Suppressing the error message.
        if (accessory == null) return;

        var gameObject = accessory.gameObject;
        if (gameObject == null)
        {
#if DEBUG
            throw new Exception($"Unable to find GameObject from the CmpAccessory Component");
#else
            return;
#endif
        }

        var skinnedAccessory = gameObject.GetComponent<SkinnedAccessory>();
        if (skinnedAccessory == null)
        {
#if DEBUG
            throw new Exception($"Unable to find Skinned Accesory.");
#else
            return;
#endif
        }

        skinnedAccessory.Merge(chaControl);
    }

    public static void ChangeAccessoryAsync_Prefix(ChaControl __0, int slotNo)
    {
        try
        {
            var accessory =
                (CmpAccessory) _moreAccessoriesGetCmpAccessoryMethodInfo.Invoke(null, new object[] {__0, slotNo + 20});
            ProcessForSkinnedAccessory(__0, accessory);
        }
        catch (Exception e)
        {
            // I hope you dont see this one ever again.
            Logger.LogError("Failed to attach More Accessory SkinnedAccessory to the character controller!");
            Logger.LogError(e.Message);
            Logger.LogError(e.StackTrace);
        }
    }

    // ReSharper disable once InconsistentNaming
    public static void RegisterQueue(object __instance)
    {
        if (__instance.GetType() != _fields.type)
        {
            Logger.LogError("SkinnedAccessory hook is not called by correct coroutine class!");
            return;
        }

        try
        {
            var chaControl = (ChaControl) _fields.chaControl.GetValue(__instance);
            if (chaControl == null) throw new Exception("Unable to find character controller.");

            var slotId = (int) _fields.slotNo.GetValue(__instance);
            if (slotId < 0) throw new Exception("Unable to obtain accessory slot id from the coroutine.");

            var accessory = chaControl.cmpAccessory[slotId];
            ProcessForSkinnedAccessory(chaControl, accessory);
        }
        catch (Exception e)
        {
            // I hope you dont see this one ever again.
            Logger.LogError("Failed to attach SkinnedAccessory to the character controller!");
            Logger.LogError(e.Message);
            Logger.LogError(e.StackTrace);
        }
    }

    private class CoroutineFields
    {
        public readonly FieldInfo chaControl;
        public readonly FieldInfo slotNo;
        public readonly Type type;
        public readonly bool valid;

        public CoroutineFields(Type type)
        {
            this.type = type;
            var fields = type.GetFields(AccessTools.all);
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.Name == "slotNo" && fieldInfo.FieldType.Name == "Int32")
                {
                    slotNo = fieldInfo;
                    if (chaControl == null) continue;
                    valid = true;
                    break;
                }

                if (fieldInfo.FieldType.Name == "ChaControl" && fieldInfo.Name.Contains("this"))
                {
                    chaControl = fieldInfo;
                    if (slotNo == null) continue;
                    valid = true;
                    break;
                }
            }
        }
    }
#endif
}

[DisallowMultipleComponent]
public class SkinnedAccessory : MonoBehaviour
{
    private static readonly Bounds Bound = new Bounds(new Vector3(0f, 10f, 0f), new Vector3(20f, 20f, 20f));
    public List<SkinnedMeshRenderer> meshRenderers;
    public GameObject skeleton;
    private int _done;

#if AI || HS2
    public void Merge(ChaControl chaControl)
    {
        StartCoroutine(TryMerge(chaControl));
    }

    private IEnumerator TryMerge(ChaControl chaControl)
    {
        if (ReferenceEquals(chaControl, null) ||
            !SkinnedBones.TryGetSkinnedBones(chaControl, out var dict)) yield break;
        meshRenderers.ForEach(smr =>
        {
            smr.enabled = false;
            smr.rootBone = chaControl.objBodyBone.transform;
            StartCoroutine(MergeCoroutine(smr, dict));
        });
    }

    private IEnumerator MergeCoroutine(SkinnedMeshRenderer smr, [NotNull] IReadOnlyDictionary<string, Transform> dict)
    {
        try
        {
            smr.bones = smr.bones
                .Select(boneTransform =>
                    !ReferenceEquals(boneTransform, null) && dict.TryGetValue(boneTransform.name, out var bone)
                        ? bone
                        : boneTransform
                )
                .ToArray();
            smr.enabled = true;
            smr.localBounds = Bound;
        }
        finally
        {
            _done++;
            if (_done == meshRenderers.Count) Destroy(skeleton); // 😂👌
        }

        yield break;
    }
#endif
}
