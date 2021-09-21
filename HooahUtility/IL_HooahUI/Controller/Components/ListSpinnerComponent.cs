using System;
using System.Reflection;
using HooahUtility.Model.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace HooahUtility.Controller.Components
{
    // Since this is totally new and requires dynamic shits, i will bite the bullet
    public class ListSpinnerComponent : MonoBehaviour, IFormComponent
    {
        public Button plusButton;
        public Button minusButton;
        public TMP_Text title;
        public TMP_Text display;

        // get the value
        // return int
        public Func<int> CallbackGetValue;
        public Action<int> CallbackSetValue;
        public Func<int, int> CallbackGetNextValue;
        public Func<int, int> CallbackGetPrevValue;

        public void Initialize()
        {
            plusButton.onClick.AddListener(SetNext);
            minusButton.onClick.AddListener(SetPrev);
        }

        public void SetTitle(MemberInfo memberInfo)
        {
            var nameAttribute = memberInfo.GetCustomAttribute<FieldNameAttribute>();
            title.text = nameAttribute == null ? memberInfo.Name.ToProperCase() : nameAttribute.name;
        }

        public void SetText(string text) => display.text = text;

        public int Value => CallbackGetValue?.Invoke() ?? -1;
        public int NextValue => CallbackGetNextValue?.Invoke(Value) ?? -1;
        public int PrevValue => CallbackGetPrevValue?.Invoke(Value) ?? -1;

        public void SetNext() => CallbackSetValue(NextValue);

        public void SetPrev() => CallbackSetValue(PrevValue);
    }
}