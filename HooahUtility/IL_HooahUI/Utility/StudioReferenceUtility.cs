using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
#if AI || HS2
using HooahUtility.Model;
using Studio;
using UnityEngine;
using Utility;
#endif

namespace HooahUtility.Utility
{
    public static class StudioReferenceUtility
    {
#if AI || HS2
        public static GameObject GetOciGameObject(ObjectCtrlInfo objectCtrlInfo)
        {
            switch (objectCtrlInfo)
            {
                case OCICamera ociCamera:
                    return ociCamera.objectItem;
                case OCIChar ociChar:
                    return ociChar.charInfo.gameObject;
                case OCIFolder ociFolder:
                    return ociFolder.objectItem;
                case OCIItem ociItem:
                    return ociItem.objectItem;
                case OCILight ociLight:
                    return ociLight.objectLight;
                case OCIRoute ociRoute:
                    return ociRoute.objectItem;
                case OCIRoutePoint ociRoutePoint:
                    return ociRoutePoint.objectItem;
                default:
                    return null;
            }
        }

        public static bool TryGetOciGameObject(ObjectCtrlInfo objectCtrlInfo, out GameObject gameObject)
        {
            gameObject = GetOciGameObject(objectCtrlInfo);
            return gameObject != null;
        }

        public static GameObject GetOciEndNodeGameObject(ObjectCtrlInfo objectCtrlInfo)
        {
            switch (objectCtrlInfo)
            {
                case OCIItem ociItem:
                    return ociItem.objectItem;
                case OCILight ociLight:
                    return ociLight.objectLight;
                default:
                    return null;
            }
        }

        public static bool TryGetOciEndNodeGameObject(ObjectCtrlInfo objectCtrlInfo, out GameObject gameObject)
        {
            gameObject = GetOciEndNodeGameObject(objectCtrlInfo);
            return gameObject != null;
        }

        /// <summary>
        /// Copy all serializable hooah component's data to same target component.
        /// </summary>
        /// <param name="srcInstance"></param>
        /// <param name="targetInstance"></param>
        /// <typeparam name="T"></typeparam>
        public static void CopyComponentData<T>(T srcInstance, T targetInstance) where T : IFormData
        {
            foreach (var memberInfo in SerializationUtility.GetAllSerializableFields(srcInstance).Values)
            {
                if (!SerializationUtility.TryGetMemberValue(memberInfo, srcInstance, out var value)) continue;
                var type = value.GetType();

                // to not even try to copy unity objects
                if (!type.IsPrimitive && !type.IsAssignableFrom(typeof(Object)))
                    SerializationUtility.TrySetMemberValue(
                        memberInfo,
                        targetInstance,
                        SerializationUtility.Deserialize(type, SerializationUtility.Serialize(type, value))
                    );
                else
                    SerializationUtility.TrySetMemberValue(memberInfo, targetInstance, value);
            }
        }

        /// <summary>
        /// Copy all serializable hooah component's data in the object to target object.
        /// </summary>
        /// <param name="srcInfo"></param>
        /// <param name="targetInfo"></param>
        public static void CopyComponentsData(ObjectCtrlInfo srcInfo, ObjectCtrlInfo targetInfo)
        {
            // Just in case...
            if (srcInfo == null || targetInfo == null || srcInfo == targetInfo) return;
            if (
                !TryGetOciEndNodeGameObject(srcInfo, out var srcGameObject) ||
                !TryGetOciEndNodeGameObject(targetInfo, out var targetGameObject) ||
                srcGameObject == targetGameObject
            ) return;
            var srcComponents = srcGameObject.GetComponents<IFormData>().ToArray();
            var targetComponents = targetGameObject.GetComponents<IFormData>().ToArray();

            if (
                // ignore if there is no hooah utility data.
                srcComponents.Length == 0 ||
                targetComponents.Length == 0 ||
                // ignore if target component is slightly different.
                srcComponents.Length != targetComponents.Length
            ) return;

            for (var i = 0; i < srcComponents.Length; i++)
            {
                var srcComponent = srcComponents[i];
                var targetComponent = targetComponents[i];
                if (srcComponent.GetType() == targetComponent.GetType())
                    CopyComponentData(srcComponent, targetComponent);
            }
        }

        /// <summary>
        /// What things are change.
        /// </summary>
        public struct KeyChange
        {
            public int From;
            public int To;
            public ObjectCtrlInfo FromOci;
            public ObjectCtrlInfo ToOci;
        }

        /// <summary>
        /// To find what item's id has changed from last transaction.
        /// Example: Import Scene, Duplicate Studio Items
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<KeyChange> GetKeyChanges()
        {
            var std = Studio.Studio.Instance;
            var dict = std.dicObjectCtrl;
            foreach (var entries in std.sceneInfo.dicChangeKey
                .Where(x => dict.ContainsKey(x.Value) && dict.ContainsKey(x.Key)))
            {
                yield return new KeyChange()
                {
                    From = entries.Value,
                    To = entries.Key,
                    FromOci = dict[entries.Value],
                    ToOci = dict[entries.Key]
                };
            }
        }

        /// <summary>
        /// To find what item's id has changed from last transaction.
        /// This function will only find Item and lights.
        /// Example: Import Scene, Duplicate Studio Items
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<KeyChange> GetItemChanges()
        {
            return GetKeyChanges().Where(
                x => (x.FromOci is OCIItem && x.ToOci is OCIItem) || (x.FromOci is OCILight && x.ToOci is OCILight)
            );
        }

        /// <summary>
        /// To find certain type of things from the studio scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static KeyValuePair<int, T>[] GetOfType<T>() where T : ObjectCtrlInfo
        {
            return Studio.Studio.Instance.dicObjectCtrl
                .Where(x => x.Value is T)
                .ToArray() as KeyValuePair<int, T>[];
        }

        /// <summary>
        /// Get next entry of same type of item from the scene.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int GetNextTypeKey<T>(int key) where T : ObjectCtrlInfo
        {
            var chars = Studio.Studio.Instance.dicObjectCtrl.Where(x => (x.Value is T)).ToArray();
            if (chars.Length == 0) return -1;

            return chars
                .SkipWhile(x => x.Key != key)
                .Skip(1)
                .DefaultIfEmpty(chars[0])
                .FirstOrDefault().Key;
        }

        /// <summary>
        /// Get previous entry of same type of item from the scene.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int GetPrevTypeKey<T>(int key) where T : ObjectCtrlInfo
        {
            var chars = Studio.Studio.Instance.dicObjectCtrl.Where(x => (x.Value is T)).ToArray();


            return chars
                .TakeWhile(x => x.Key != key)
                .DefaultIfEmpty(chars[chars.Length - 1])
                .FirstOrDefault().Key;
        }
#else
#endif
    }
}
