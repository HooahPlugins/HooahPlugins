using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static void CopyComponentData<T>(T from, T to) where T : IFormData
        {
            var fields = SerializationUtility.GetAllSerializableFields(from);
            foreach (var keyValuePair in fields)
            {
                switch (keyValuePair.Value)
                {
                    case PropertyInfo propertyInfo:
                        propertyInfo.SetValue(to, propertyInfo.GetValue(from));
                        break;
                    case FieldInfo fieldInfo:
                        fieldInfo.SetValue(to, fieldInfo.GetValue(from));
                        break;
                }
            }
        }

        public static void CopyComponentsData(ObjectCtrlInfo srcInfo, ObjectCtrlInfo targetInfo)
        {
            GameObject srcGameObject = null;
            GameObject targetGameObject = null;
            // Just Items and Monika.
            if (srcInfo is OCIItem srcItemInfo && targetInfo is OCIItem targetItemInfo)
            {
                srcGameObject = srcItemInfo.itemComponent.gameObject;
                targetGameObject = targetItemInfo.itemComponent.gameObject;
            }
            // Volumetric lights
            else if (srcInfo is OCILight srcLightInfo && targetInfo is OCILight targetLightInfo)
            {
                srcGameObject = srcLightInfo.objectLight;
                targetGameObject = targetLightInfo.objectLight;
            }

            if (srcGameObject == null || targetGameObject == null) return;

            var srcComponents = srcGameObject.GetComponents<IFormData>().ToArray();
            var targetComponents = targetGameObject.GetComponents<IFormData>().ToArray();

            if (srcComponents.Length != targetComponents.Length) return;
            for (var i = 0; i < srcComponents.Length; i++)
            {
                var srcComponent = srcComponents[i];
                var targetComponent = targetComponents[i];
                if (srcComponent.GetType() == targetComponent.GetType())
                {
                    // yes, it is same. it is time to merge.
                    CopyComponentData(srcComponent, targetComponent);
                }
            }
        }

        public struct KeyChange
        {
            public int From;
            public int To;
            public ObjectCtrlInfo FromOci;
            public ObjectCtrlInfo ToOci;
        }

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

        public static IEnumerable<KeyChange> GetItemChanges()
        {
            return GetKeyChanges().Where(
                x => (x.FromOci is OCIItem && x.ToOci is OCIItem) || (x.FromOci is OCILight && x.ToOci is OCILight)
            );
        }

        // todo: update this shit when character is being added or removed.
        // or just shit like that i guess
        public static KeyValuePair<int, T>[] GetOfType<T>() where T : ObjectCtrlInfo
        {
            return Studio.Studio.Instance.dicObjectCtrl
                .Where(x => x.Value is T)
                .ToArray() as KeyValuePair<int, T>[];
        }

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