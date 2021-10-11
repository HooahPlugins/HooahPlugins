using System.Collections.Generic;
using System.Linq;
using HooahUtility.AdvancedStudioUI;
using HooahUtility.AdvancedStudioUI.Constant;
using HooahUtility.Service;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HooahUtility.Controller.Components
{
    public class MultiOptionButtonComponent : MonoBehaviour, IFormComponent
    {
        private Option[] _checks;
        public Transform checkParent;
        public Button button;
        public TMP_Text buttonText;
        public bool[] OptionValues => _checks.Select(x => x.Value()).ToArray();

        struct Option
        {
            private TMP_Text _text;
            private Toggle _check;

            public Option(TMP_Text text, Toggle check)
            {
                _text = text;
                _check = check;
                _check.isOn = true;
            }

            public Option SetText(string text)
            {
                _text.text = text;
                return this;
            }

            public bool Value() => _check.isOn;
        }

        public void SetOptions(string buttonName, string[] options, UnityAction<bool[]> btnCallback)
        {
            var div = 1f / options.Length;
            _checks = options.Select((title, i) =>
            {
                if (!AssetManager.TryMakeUIPrefab(UIConstant.Checkbox, out var check, checkParent)) return default;
                UIUtility.ScaleAsGrid(check, i, div);
                return new Option(
                    check.GetComponentInChildren<TMP_Text>(),
                    check.GetComponentInChildren<Toggle>()
                ).SetText(title);
            }).ToArray();

            buttonText.text = buttonName;
            button.onClick.AddListener(() => btnCallback.Invoke(OptionValues));
        }
    }
}