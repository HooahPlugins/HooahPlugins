using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace HooahUtility.Controller.Components
{
    /// <summary>
    /// A Dropdown Component for Enums
    /// </summary>
    public class DropdownComponent : FormComponentBase
    {
        public TMP_Dropdown inputDropdown;

        public override void AssignValues()
        {
            inputDropdown.ClearOptions();
            UpdateOptions();
            SetUIValue(inputDropdown);
            inputDropdown.onValueChanged.AddListener(value =>
                SetValue(MemberType, Enum.ToObject(MemberType, value), () => { }));
        }

        public void UpdateOptions()
        {
            inputDropdown.options = MemberType.GetEnumNames().Select(x => new TMP_Dropdown.OptionData(x)).ToList();
        }
    }
}