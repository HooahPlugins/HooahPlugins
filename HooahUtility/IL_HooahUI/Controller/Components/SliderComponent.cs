using System;
using System.Reflection;
using HooahUtility.Model.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HooahUtility.Controller.Components
{
    public class SliderComponent : FormComponentBase
    {
        public TMP_InputField inputField;
        public Slider inputSlider;

        public override void ParseAttribute()
        {
            foreach (var attr in MemberInfo.GetCustomAttributes())
            {
                switch (attr)
                {
                    case RangeAttribute rangeAttribute:
                        inputSlider.minValue = rangeAttribute.min;
                        inputSlider.maxValue = rangeAttribute.max;
                        break;
                    case PropertyRangeAttribute propertyRangeAttribute:
                        inputSlider.minValue = propertyRangeAttribute.min;
                        inputSlider.maxValue = propertyRangeAttribute.max;
                        break;
                }
            }

            if (DesignatedMinimumValue.TryGetValue(MemberType, out var min))
                inputSlider.minValue = Math.Max(inputSlider.minValue, min);

            if (DesignatedMaximumValue.TryGetValue(MemberType, out var max))
                inputSlider.maxValue = Math.Min(inputSlider.maxValue, max);
        }

        private void SetValue(float value)
        {
            SetValue(MemberType, value, () =>
            {
                SetUIValue(inputField);
                SetUIValue(inputSlider);
            });
        }

        private void SetValue(string value)
        {
            var floatValue = Convert.ToSingle(value);
            SetValue(floatValue);
        }

        public override void AssignValues()
        {
            SetUIValue(inputField);
            SetUIValue(inputSlider);
            
            if (InputFieldContentTypeInfo.TryGetValue(MemberType, out var contentType))
                inputField.contentType = contentType;

            inputSlider.onValueChanged.AddListener(SetValue);
            inputField.onSubmit.AddListener(SetValue);
        }
    }
}