using System;
using HooahUtility.Model;
using HooahUtility.Serialization;
using HooahUtility.Serialization.Attributes;
using MessagePack;
using UnityEngine;

namespace HooahComponents.StudioExtension
{
    [Serializable, MessagePackObject()]
    public struct StudioOptionDependency
    {
        [Key(0)] public int[] RequiresAny;
        [Key(1)] public int[] RequiresAll;
        [Key(2)] public int[] BlocksAny;
        [Key(3)] public int[] BlocksAll;

        public StudioOptionDependency(int[] requiresAny, int[] requiresAll, int[] blocksAny, int[] blocksAll)
        {
            RequiresAny = requiresAny;
            RequiresAll = requiresAll;
            BlocksAny = blocksAny;
            BlocksAll = blocksAll;
        }
    }

    [Serializable, MessagePackObject()]
    public class StudioOption
    {
        [Key(0)] public StudioOptionDependency Dependency; // the dependency
        [Key(1)] public string Name; // name of the option
        [Key(2)] public GameObject[] GameObjects; // objects to toggle
        [Key(3)] public GameObject AttachmentPoint; // where the child gameobjects will go
    }

    [Serializable, MessagePackObject()]
    public class StudioOptionGroup
    {
        [Key(0)] public string Name; // name of option group
        [Key(1)] public int Limit; // total limit of group
        [Key(2)] public StudioOption[] Options; // options for the group
        [Key(3)] public StudioOptionDependency Dependency; // 
    }

    [Serializable, MessagePackObject()]
    public class ExtendedOptionData
    {
        [Key(0)] public StudioOptionGroup[] groups = { };
    }

    [Serializable, MessagePackObject()]
    public struct OptionStateData
    {
        // generate array of booleans based on the extended option data.
    }

    public class ExtendedStudioItem : HooahSerializer, IFormData
    {
        public MaterialPresetGroup[] MaterialPresetGroups; // for selecting material options
        [HooahSerialize] public ExtendedOptionData ExtendedOptionData; // recover the serialization 
        [Key(0)] public OptionStateData OptionStateData;

        public void GenerateStateData()
        {
            
        }

        public new void OnAfterDeserialize()
        {
            DeserializeData();
            // Apply option state data
        }
    }
}