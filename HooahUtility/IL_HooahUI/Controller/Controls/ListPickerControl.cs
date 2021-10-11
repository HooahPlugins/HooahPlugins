using UnityEngine;
using System;
using System.Linq;
using HooahUtility.Controller.Components;
using UnityEngine.UI;

namespace HooahUtility.Controller.Controls
{
    public class ListPickerControl : WindowBase
    {
        public struct ListItem
        {
            public Sprite Sprite;
            public string Title;
            public string Desc;
            public Action Callback;
        }

        public static ListPickerControl Instance { get; set; }

        public static bool TryGetInstance(out ListPickerControl instance)
        {
            instance = Instance;
            return instance != null;
        }

        private void Start()
        {
            Instance = this;
            AdjustSize();
            Enumerable.Range(0, 10).ToList().ForEach(_ => { Add(); });
        }

        public uint page = 0;
        public uint column = 2;
        public uint row = 12;
        public float margin = 2;
        public RectTransform ContentTransform;
        public GridLayoutGroup GridLayoutGroup;
        public GameObject ListObject;

        public void AdjustSize()
        {
            var sizeDelta = ContentTransform.rect;
            GridLayoutGroup.cellSize = new Vector2(
                (sizeDelta.width - margin * (column - 1)) / column,
                (sizeDelta.height - margin * (row - 1)) / row
            );
        }

        public void Add()
        {
            var newList = Instantiate(ListObject, ContentTransform);
            if (newList == null) throw new Exception("Something went wrong while Initializing the item to the list.");
            var component = newList.GetComponent<ListItemComponent>();
            if (component == null) throw new Exception("The object does not have list item component");
            component.SubText = "Cope";
            component.TitleText = "Dialate";
        }

        public void Clear()
        {
            foreach (var i in Enumerable.Range(0, ContentTransform.childCount))
                Destroy(ContentTransform.GetChild(i));
        }
    }
}