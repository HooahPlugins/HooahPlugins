using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HooahUtility.Controller.Components
{
    public class ListItemComponent : MonoBehaviour
    {
        public Image image;
        public TMP_Text title;
        public TMP_Text desc;
        public Button button;
        public Action onClick;
        private bool _enabled = true;

        public bool Enabled
        {
            get => button.interactable;
            set => button.interactable = value;
        }

        private void Awake()
        {
            button.onClick.AddListener(delegate { onClick?.Invoke(); });
        }

        public Sprite Sprite
        {
            get => image.sprite;
            set => image.sprite = value;
        }

        public string TitleText
        {
            get => title.text;
            set => title.SetText(value);
        }

        public string SubText
        {
            get => desc.text;
            set => desc.SetText(value);
        }
    }
}