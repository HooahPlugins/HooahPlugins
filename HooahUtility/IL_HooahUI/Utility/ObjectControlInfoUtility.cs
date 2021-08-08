// Decompiled with JetBrains decompiler
// Type: HooahUtility.Utility.ObjectControlInfoUtility
// Assembly: HooahComponents, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 36F76C18-36CE-4019-97EF-FF9FBC728F0D
// Assembly location: D:\projects\HooahPlugins\HooahComponents\AI_Hooah\bin\Release\final\AI_Hooah.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

#if HS2 || AI
using Studio;
#endif
namespace HooahUtility.Utility
{
    public static class ObjectControlInfoUtility
    {
#if HS2 || AI
        private static readonly FieldInfo CharFkListBonesField;
        private static readonly FieldInfo CharFkTargetChangeAmountField;
        private static readonly FieldInfo ItemFkListBonesField;
        private static readonly FieldInfo ItemFkTargetChangeAmountField;
        private static readonly MethodInfo CharFkTargetTransformGetter;
        private static readonly MethodInfo ItemFkTargetTransformGetter;
        private static readonly Type CharFkTargetInfoClass;
        private static readonly Type ItemFkTargetInfoClass;
        private static readonly Type CharIKTargetInfoClass;
        private static readonly FieldInfo CharIKListBonesField;
        private static readonly FieldInfo CharIKTargetGuideObjectField;
        private static readonly MethodInfo CharIKTargetTransformGetter;

        static ObjectControlInfoUtility()
        {
            var type1 = typeof(ItemFKCtrl);
            var type2 = typeof(FKCtrl);
            var type3 = typeof(OCIChar);
            var type4 = typeof(IKCtrl);
            ItemFkTargetInfoClass = type1.GetNestedType("TargetInfo", BindingFlags.NonPublic);
            ItemFkTargetChangeAmountField = ItemFkTargetInfoClass.GetField("changeAmount");
            ItemFkTargetTransformGetter = ItemFkTargetInfoClass.GetProperty("transform")?.GetGetMethod();
            ItemFkListBonesField = type1.GetField("listBones", BindingFlags.Instance | BindingFlags.NonPublic);
            CharFkTargetInfoClass = type2.GetNestedType("TargetInfo", BindingFlags.NonPublic);
            CharFkTargetChangeAmountField = CharFkTargetInfoClass.GetField("changeAmount");
            CharFkTargetTransformGetter = CharFkTargetInfoClass.GetProperty("transform")?.GetGetMethod();
            CharFkListBonesField = type2.GetField("listBones", BindingFlags.Instance | BindingFlags.NonPublic);
            CharIKTargetInfoClass = type3.GetNestedType("IKInfo");
            CharIKListBonesField = type4.GetField("listIKInfo", BindingFlags.Instance | BindingFlags.NonPublic);
            CharIKTargetGuideObjectField = CharIKTargetInfoClass.GetField("guideObject");
            CharIKTargetTransformGetter = CharIKTargetInfoClass.GetProperty("targetObject")?.GetGetMethod();
        }

        public static IEnumerable<object> GetFkTargets(OCIItem ociItem)
        {
            if (!(ItemFkListBonesField.GetValue(ociItem.itemFKCtrl) is IEnumerable source))
                source = new object[0];
            return source.Cast<object>();
        }

        public static IEnumerable<ChangeAmount> GetChangeAmounts(OCIItem ociItem) => GetFkTargets(ociItem)
            .Select(ItemFkTargetChangeAmountField.GetValue).Cast<ChangeAmount>();

        public static IEnumerable<Transform> GetFkTransforms(
            OCIItem ociItem,
            in int[] indexRequest)
        {
            var bones = GetFkTargets(ociItem).ToArray();
            return indexRequest.Where(x => x < bones.Length).Select(i =>
            {
                var obj = bones[i];
                return obj == null ? null : ItemFkTargetTransformGetter.Invoke(obj, null);
            }).Cast<Transform>();
        }

        public static Transform GetFkTransform(OCIItem ociItem, int index)
        {
            var obj = GetFkTargets(ociItem).ElementAtOrDefault(index);
            return obj == null ? null : ItemFkTargetTransformGetter.Invoke(obj, null) as Transform;
        }

        public static int GetFkTransformIndex(OCIItem ociItem, Transform transform) => Array.FindIndex(
            GetFkTargets(ociItem).Select(x => ItemFkTargetTransformGetter.Invoke(x, null) as Transform).ToArray(),
            x => x == transform);

        public static IEnumerable<object> GetFkTargets(OCIChar ociChar)
        {
            if (!(CharFkListBonesField.GetValue(ociChar.fkCtrl) is IEnumerable source))
                source = new object[0];
            return source.Cast<object>();
        }

        public static IEnumerable<ChangeAmount> GetChangeAmounts(OCIChar ociChar) => GetFkTargets(ociChar)
            .Select(CharFkTargetChangeAmountField.GetValue).Cast<ChangeAmount>();

        public static IEnumerable<Transform> GetFkTransforms(
            OCIChar ociChar,
            in int[] indexRequest)
        {
            var bones = GetFkTargets(ociChar).ToArray();
            return indexRequest.Where(x => x < bones.Length).Select(i =>
            {
                var obj = bones[i];
                return obj == null ? null : CharFkTargetTransformGetter.Invoke(obj, null);
            }).Cast<Transform>();
        }

        public static Transform GetFkTransform(OCIChar ociItem, int index)
        {
            var obj = GetFkTargets(ociItem).ElementAtOrDefault(index);
            return obj == null ? null : CharFkTargetTransformGetter.Invoke(obj, null) as Transform;
        }

        public static int GetFkTransformIndex(OCIChar ociChar, Transform transform) => Array.FindIndex(
            GetFkTargets(ociChar).Select(x => CharFkTargetTransformGetter.Invoke(x, null) as Transform).ToArray(),
            x => x == transform);

        public static IEnumerable<OCIChar.IKInfo> GetIKTargets(OCIChar ociChar)
        {
            if (!(CharIKListBonesField.GetValue(ociChar.ikCtrl) is IEnumerable source))
                source = new object[0];
            return source.Cast<OCIChar.IKInfo>();
        }

        public static IEnumerable<GuideObject> GetIKGuideObjects(OCIChar ociChar) => GetIKTargets(ociChar)
            .Select(x=>x.guideObject);

        public static IEnumerable<Transform> GetIKTransforms(
            OCIChar ociChar,
            in int[] indexRequest)
        {
            var bones = GetIKTargets(ociChar).ToArray();
            return indexRequest.Where(x => x < bones.Length).Select(i =>
            {
                var obj = bones[i];
                return obj == null ? null : CharIKTargetTransformGetter.Invoke(obj, null);
            }).Cast<Transform>();
        }

        public static Transform GetIKTransform(OCIChar ociItem, int index)
        {
            var obj = GetIKTargets(ociItem).ElementAtOrDefault(index);
            return obj == null ? null : CharIKTargetTransformGetter.Invoke(obj, null) as Transform;
        }

        public static int GetIKTransformIndex(OCIChar ociChar, Transform transform) => Array.FindIndex(
            GetIKTargets(ociChar).Select(x => CharIKTargetTransformGetter.Invoke(x, null) as Transform).ToArray(),
            x => x == transform);
#endif
    }
}