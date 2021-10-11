using System;
using HooahUtility.Model;
using HooahUtility.Serialization;
using HooahUtility.Serialization.Attributes;
using MessagePack;
using UnityEngine;

namespace HooahComponents.StudioExtension
{
    // also this will require more attachment points
    //      character / item
    //         + add attachment point
    //          
    // The option should be tree structure?
    // etc
    // if i make a gun studio item
    //      rails
    //          attachments
    //      guard
    //          asdflkjj
    //      barrel
    //          suppressor
    // Notable multi option concerns
    //  * attachment transform change based on the option
    //  * option dependency
    //      * certain option or option group requires certain option or option group to be activated
    //      * certain option or option group requires certain option or option group to be deactiated
    // 
    // material preset
    //      list of custom materials
    //      load the asset by the demand.
    //      the material preset definition will be stored as csv or json... even yaml.
    //      but well using yaml is easier.


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