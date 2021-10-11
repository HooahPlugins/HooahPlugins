using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace HooahUtility.Controller.Components
{
    /// <summary>
    /// Dynamic Dropdown for something that changes constantly.
    /// has a feature to adjust the index
    /// </summary>
    public class DynamicDropdownComponent : DropdownComponent
    {
        private static HashSet<DynamicDropdownComponent> DynamicDropdownInstances =
            new HashSet<DynamicDropdownComponent>();

        public static void UpdateAll()
        {
            foreach (var dropdownComponent in
                DynamicDropdownInstances.Where(x => x.isActiveAndEnabled))
                dropdownComponent.UpdateOptions(); // update options.
        }

        public void Awake() => DynamicDropdownInstances.Add(this);

        public void OnDestroy() => DynamicDropdownInstances.Remove(this);

        // Apply the options
        public Action<DynamicDropdownComponent> OnUpdateOptions;

        // Apply the change
        public Action<DynamicDropdownComponent, object> OnSelectValue;

        private List<string> options = new List<string>();

        public void AddOption(string option) => options.Add(option);

        public void AddOption(IEnumerable<string> newOptions) => options.AddRange(newOptions);

        public override void AssignValues()
        {
            inputDropdown.ClearOptions();

            options.Clear();
            OnUpdateOptions?.Invoke(this);
            SetUIValue(inputDropdown);
            inputDropdown.onValueChanged.AddListener(value => OnSelectValue?.Invoke(this, value));
            options.Clear();
        }

        public void AdjustOptions()
        {
            // get value or get value 
            // update the options
            // adjust the index
            
            options.Clear();
            OnUpdateOptions?.Invoke(this);
            SetUIValue(inputDropdown);
            inputDropdown.onValueChanged.AddListener(value => OnSelectValue?.Invoke(this, value));
            options.Clear();
        }
    }
}