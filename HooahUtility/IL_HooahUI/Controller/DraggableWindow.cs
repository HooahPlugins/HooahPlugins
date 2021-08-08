using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HooahUtility.Controller
{
    [Serializable]
    public class DraggableWindow
    {
        [SerializeField] public TMP_Text uiTextTitle;
        [SerializeField] public Button uiButtonClose;
        public string Title
        {
            get { return uiTextTitle.text; }
            set { uiTextTitle.text = value; }
        }
    }
}
