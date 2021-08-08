using System;
using System.Linq;
using JetBrains.Annotations;
using MessagePack;
using UnityEngine;
#if HS2 || AI
using Studio;

#endif

namespace HooahUtility.Serialization.StudioReference
{
    [MessagePackObject]
    public class StudioReference : IMessagePackSerializationCallbackReceiver
    {
        [Key(1)] public int index = -1;

#if HS2 || AI
        [CanBeNull, IgnoreMember] public ObjectCtrlInfo Reference { get; private set; }

        [CanBeNull, IgnoreMember] public Transform ReferenceTransform { get; private set; }

        [CanBeNull, Key(0)] public ChlidNodeReferenceResolver Resolver { set; private get; }

        public bool IsResolved { set; get; }

        public void AssignReference(ObjectCtrlInfo objectCtrlInfo)
        {
            index = Array.FindIndex(Singleton<Studio.Studio>.Instance.dicObjectCtrl.Values.ToArray(),
                x => x == objectCtrlInfo);
            Reference = index >= 0 ? objectCtrlInfo : null;
            UpdateTransform();
        }

        public void AssignReference(int index)
        {
            this.index = index;
            ObjectCtrlInfo objectCtrlInfo;
            Reference = this.index <= 0 ||
                        !Singleton<Studio.Studio>.Instance.dicObjectCtrl.TryGetValue(index, out objectCtrlInfo)
                ? null
                : objectCtrlInfo;
        }

        public void OnBeforeSerialize()
        {
            if (Resolver == null)
                return;
            Resolver.StoreReferenceData(Reference, ReferenceTransform);
        }

        public void OnAfterDeserialize()
        {
            ObjectCtrlInfo objectCtrlInfo;
            if (index < 0 ||
                !Singleton<Studio.Studio>.Instance.dicObjectCtrl.TryGetValue(index, out objectCtrlInfo)) return;
            Reference = objectCtrlInfo;
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (Resolver != null)
            {
                ReferenceTransform = Resolver.GetReferenceTransform(Reference);
                if (ReferenceTransform == null) return;
            }

            ReferenceTransform = GetRootTransform(Reference);
        }

        private Transform GetRootTransform(ObjectCtrlInfo objectCtrlInfo)
        {
            switch (objectCtrlInfo)
            {
                case OCIChar ociChar:
                    return ociChar.charInfo.gameObject.transform;
                case OCILight ociLight:
                    return ociLight.objectLight.transform;
                case OCIItem ociItem:
                    return ociItem.objectItem.transform;
                case OCIFolder ociFolder:
                    return ociFolder.objectItem.transform;
                case OCICamera ociCamera:
                    return ociCamera.objectItem.transform;
                case OCIRoute ociRoute:
                    return ociRoute.objectItem.transform;
                default:
                    return null;
            }
        }
#else
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }
#endif
    }
}