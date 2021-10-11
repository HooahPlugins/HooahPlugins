using MessagePack;
using UnityEngine;
#if HS2 || AI
using Studio;
#endif

namespace HooahUtility.Serialization.StudioReference
{
    public class IKReferenceResolver : ChlidNodeReferenceResolver
    {
        [Key(0)] public int index = -1;

#if HS2 || AI
        public override bool IsResolverCompatible(ObjectCtrlInfo objectCtrlInfo) => false;
        public override Transform GetReferenceTransform(ObjectCtrlInfo objectCtrlInfo) => null;
        public override void StoreReferenceData(ObjectCtrlInfo objectCtrlInfo, Transform transform)
        {
        }
#endif
    }
}
