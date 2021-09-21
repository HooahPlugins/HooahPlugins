using MessagePack;
#if AI || HS2
using AIChara;
using Studio;
#endif

namespace HooahUtility.Serialization.StudioReference
{
    [MessagePackObject]
    public class CharacterReference : StudioReference
    {
#if AI || HS2

        #region Useful Properties but not for serialization.

        [IgnoreMember]
        public string CharName => (Reference as OCIChar)?.oiCharInfo?.charFile?.parameter?.fullname
                                  ?? (ReferenceTransform ? ReferenceTransform.name 
                                      : "Not Assigned");

        [IgnoreMember] public ChaControl ChaControl => (Reference as OCIChar)?.charInfo;

        #endregion

#else
        #region Useful Properties but not for serialization.
        [IgnoreMember] public string CharName => "Editor Placeholder";
        #endregion
#endif
    }
}