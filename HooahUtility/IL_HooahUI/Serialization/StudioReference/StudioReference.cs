using JetBrains.Annotations;
using MessagePack;
using UnityEngine;
#if HS2 || AI
using HooahUtility.Utility;
using Studio;
#endif

namespace HooahUtility.Serialization.StudioReference
{
    [MessagePackObject]
    public class StudioReference : IStudioReference
    {
        [Key(0)] public int DicKey = -1; // an id for the serialization

#if HS2 || AI
        [IgnoreMember]
        public ObjectCtrlInfo Reference =>
            Studio.Studio.Instance.dicObjectCtrl.TryGetValue(DicKey, out var objectCtrlInfo) ? objectCtrlInfo : null;

        [IgnoreMember]
        public Transform ReferenceTransform => ReferenceEquals(null, Reference) ? GetRootTransform(Reference) : null;

        [CanBeNull]
        private Transform GetRootTransform(ObjectCtrlInfo objectCtrlInfo) =>
            StudioReferenceUtility.GetOciGameObject(objectCtrlInfo)?.transform;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            // create some import key index adjustment.
            // if the last import key references has some shit then good
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
