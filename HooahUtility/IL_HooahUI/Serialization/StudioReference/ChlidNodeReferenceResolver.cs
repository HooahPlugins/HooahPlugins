using MessagePack;
using UnityEngine;

#if HS2 || AI
using Studio;

#endif

namespace HooahUtility.Serialization.StudioReference
{
    [MessagePackObject(false)]
    public abstract class ChlidNodeReferenceResolver
    {
#if HS2 || AI
        public abstract bool IsResolverCompatible(ObjectCtrlInfo objectCtrlInfo);
        public abstract Transform GetReferenceTransform(ObjectCtrlInfo objectCtrlInfo);
        public abstract void StoreReferenceData(ObjectCtrlInfo objectCtrlInfo, Transform transform);
#endif
    }
}