// Decompiled with JetBrains decompiler
// Type: HooahUtility.Serialization.Attributes.StudioReferenceAttribute
// Assembly: HooahUtilityEditorCompatible, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 52B179E7-4743-4027-9930-3D3BFEF61A6C
// Assembly location: D:\himates\AIHSModding\Assets\Plugins\Hooah\HooahUtilityEditorCompatible.dll

using System;

namespace HooahUtility.Serialization.Attributes
{
  public class StudioReferenceAttribute : Attribute
  {
    public StudioReferenceAttribute(
      StudioReferenceAttribute.ReferenceType referenceType = StudioReferenceAttribute.ReferenceType.All)
    {
    }

    public enum ReferenceType
    {
      Female = 0,
      Character = 1,
      Male = 1,
      Item = 2,
      Light = 3,
      Folder = 4,
      Camera = 5,
      Generic = 6,
      All = 7,
    }

    public enum SearchMode
    {
      ItemNode,
      FKNode,
      IKNode,
      ChildTransform,
    }
  }
}
