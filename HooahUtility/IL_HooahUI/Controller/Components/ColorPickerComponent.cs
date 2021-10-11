using AdvancedStudioUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = AdvancedStudioUI.Utility.ColorUtility;
using UnityEngineColorUtility = UnityEngine.ColorUtility;

namespace HooahUtility.Controller.Components
{
    public class ColorPickerComponent : FormComponentBase
    {
        public Button colorButton;
        public TMP_InputField inputField;

        protected override string GetTextValue()
        {
            if (GetValueInternal() is Color color)
                return color.a < 1f
                    ? UnityEngineColorUtility.ToHtmlStringRGBA(color)
                    : UnityEngineColorUtility.ToHtmlStringRGB(color);

            return "FFFFFF";
        }

        public override void AssignValues()
        {
            SetUIValue(inputField);
            colorButton.image.color = GetValue<Color>();
            inputField.onSubmit.AddListener(SetValue);
            colorButton.onClick.AddListener(DisplayColorPicker);
        }

        public void DisplayColorPicker()
        {
            ColorPickerControl.Show(GetValue<Color>(), SetValue);
        }

        public void SyncColor()
        {
            ColorPickerControl.instance.SetColor(GetValue<Color>(), true);
        }

        public void SetValue(string value)
        {
            if (ColorUtility.ParseColorString(value, out var color))
            {
                SetValue(MemberType, color, () =>
                {
                    colorButton.image.color = color;
                    SyncColor();
                });
            }
        }

        public void SetValue(Color value)
        {
            SetValue(MemberType, value, () =>
            {
                colorButton.image.color = value;
                SetUIValue(inputField);
            });
        }
    }
}