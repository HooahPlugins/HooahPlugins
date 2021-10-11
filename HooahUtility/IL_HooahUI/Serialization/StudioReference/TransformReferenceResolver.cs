using UnityEngine;

#if HS2 || AI
using Studio;
#endif

namespace HooahUtility.Serialization.StudioReference
{
    public class TransformReferenceResolver : ChlidNodeReferenceResolver
    {
#if HS2 || AI
        public override bool IsResolverCompatible(ObjectCtrlInfo objectCtrlInfo) => false;

        public override Transform GetReferenceTransform(ObjectCtrlInfo objectCtrlInfo) => (Transform) null;

        public override void StoreReferenceData(ObjectCtrlInfo objectCtrlInfo, Transform transform)
        {
        }
#endif
    }
}