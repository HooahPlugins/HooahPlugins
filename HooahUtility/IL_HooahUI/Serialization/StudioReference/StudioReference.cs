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
        // # Things to concern:
        // ----
        // 1. the hash key in unique to the scene.
        // 2. when importing the scene, it will be remapped 
        //      so the data mapped with it needs to be changed.
        //      so we need to remember the changes.
        //      we're going to solve this with some hooks
        //      like dicKey?

        [Key(0)] public int DicKey = -1; // an id for the serialization

#if HS2 || AI
        [CanBeNull, IgnoreMember]
        public ObjectCtrlInfo Reference =>
            Studio.Studio.Instance.dicObjectCtrl.TryGetValue(DicKey, out var objectCtrlInfo) ? objectCtrlInfo : null;

        [CanBeNull, IgnoreMember]
        public Transform ReferenceTransform => ReferenceEquals(null, Reference) ? GetRootTransform(Reference) : null;

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