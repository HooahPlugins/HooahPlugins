using System;

namespace HooahUtility.Serialization.Attributes
{
    public class StudioReferenceAttribute : Attribute
    {
        public StudioReferenceAttribute(ReferenceType referenceType = ReferenceType.All)
        {
        }

        public enum ReferenceType
        {
            Female = 0, Character = 1, Male = 1, Item = 2, Light = 3,
            Folder = 4, Camera = 5, Generic = 6, All = 7,
        }

        public enum SearchMode { ItemNode, FKNode, IKNode, ChildTransform, }
    }
}
