using System;
using System.Reflection;
using HooahUtility.Model.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HooahUtility.Controller.Components
{
    public class SpinnerComponent : FormComponentBase
    {
        public TMP_InputField inputField;
        public Button plusButton;
        public Button minusButton;
        public int amount = 1;

        public override void ParseAttribute()
        {
            // foreach (var attr in MemberInfo.GetCustomAttributes())
            // {
            //     switch (attr)
            //     {
            //         case RangeAttribute rangeAttribute:
            //             inputSlider.minValue = rangeAttribute.min;
            //             inputSlider.maxValue = rangeAttribute.max;
            //             break;
            //         case PropertyRangeAttribute propertyRangeAttribute:
            //             inputSlider.minValue = propertyRangeAttribute.min;
            //             inputSlider.maxValue = propertyRangeAttribute.max;
            //             break;
            //     }
            // }
            //
            // if (DesignatedMinimumValue.TryGetValue(MemberType, out var min))
            //     inputSlider.minValue = Math.Max(inputSlider.minValue, min);
            //
            // if (DesignatedMaximumValue.TryGetValue(MemberType, out var max))
            //     inputSlider.maxValue = Math.Min(inputSlider.maxValue, max);
        }

        private void SetValue(object value)
        {
            if ((MemberType == typeof(uint) || MemberType == typeof(ulong)) && (int) value < 0)
                value = 0;

            SetValue(MemberType, value, () => { SetUIValue(inputField); });
        }

        private void SetValue(string value)
        {
            if (Int32.TryParse(value, out var parsedValue))
                SetValue(parsedValue);
            else
                SetValue(0);
        }

        public int Amount
        {
            get
            {
                var ret = amount;
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftControl))
                    ret *= 10;
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift))
                    ret *= 100;
                return ret;
            }
        }

        public override void AssignValues()
        {
            SetUIValue(inputField);
            if (InputFieldContentTypeInfo.TryGetValue(MemberType, out var contentType))
                inputField.contentType = contentType;
            
            plusButton.onClick.AddListener(() =>
            {
                SetValue(GetValue<int>() + Amount);
            });
            minusButton.onClick.AddListener(() =>
            {
                SetValue(GetValue<int>() - Amount);
            });
            inputField.onSubmit.AddListener(SetValue);
        }
    }
}