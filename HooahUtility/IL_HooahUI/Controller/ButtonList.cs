using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HooahUtility.Controller
{
    [Serializable]
    public class ButtonList
    {
        protected List<Button> Buttons = new List<Button>();
        private GameObject _buttonPreset;
        private float _height = -5;
        private RectTransform _uiButtonListRoot;

        public ButtonList(RectTransform uiRectContentParent, GameObject tabButtonObject)
        {
            _uiButtonListRoot = uiRectContentParent;
            _buttonPreset = tabButtonObject;
        }

        public Button AddButton(string title, UnityAction callback)
        {
            var obj = Object.Instantiate(_buttonPreset, _uiButtonListRoot);

            var rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, _height);
            _height -= rectTransform.rect.height;

            var textComponent = obj.GetComponentInChildren<TMP_Text>();
            textComponent.text = title;

            var button = obj.GetComponent<Button>();
            button.onClick.AddListener(callback);

            Buttons.Add(button);
            return button;
        }
    }
}
