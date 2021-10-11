using MessagePack;
#if AI || HS2
#endif

namespace HooahUtility.Serialization.StudioReference
{
    [MessagePackObject]
    public class StudioObjectReference : StudioReference
    {
#if AI || HS2
        [IgnoreMember] public string Name => Reference?.treeNodeObject?.textName ?? "Not Assigned";
#endif
    }
}