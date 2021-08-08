using HooahUtility.Utility;
using MessagePack;
using UnityEngine;
#if HS2 || AI
using Studio;
#endif

namespace HooahUtility.Serialization.StudioReference
{
    [MessagePackObject()]
    public class FKReferenceResolver : ChlidNodeReferenceResolver
    {
        [Key(0)] public int index = -1;
#if HS2 || AI
        public override bool IsResolverCompatible(ObjectCtrlInfo objectCtrlInfo)
        {
            switch (objectCtrlInfo)
            {
                case OCIChar _:
                case OCIItem _:
                    return true;
                default:
                    return false;
            }
        }

        public override Transform GetReferenceTransform(ObjectCtrlInfo objectCtrlInfo)
        {
            switch (objectCtrlInfo)
            {
                case OCIChar ociItem1:
                    return ObjectControlInfoUtility.GetFkTransform(ociItem1, index);
                case OCIItem ociItem2:
                    return ObjectControlInfoUtility.GetFkTransform(ociItem2, index);
                default:
                    return null;
            }
        }

        public override void StoreReferenceData(ObjectCtrlInfo objectCtrlInfo, Transform transform)
        {
            switch (objectCtrlInfo)
            {
                case OCIChar ociChar:
                    index = ObjectControlInfoUtility.GetFkTransformIndex(ociChar, transform);
                    break;
                case OCIItem ociItem:
                    index = ObjectControlInfoUtility.GetFkTransformIndex(ociItem, transform);
                    break;
            }
        }
#endif
    }
}