using System;
using System.Linq;
using HooahUtility.Controller.ContentManagers;
using HooahUtility.Editor;
using HooahUtility.Model;
#if AI || HS2
using HooahComponents.Utility;
using KKAPI.Utilities;
#else
using HooahUtility.Editor;
#endif
using UnityEngine;

namespace AdvancedStudioUI
{
    public class StudioItemControl : WindowBase
    {
        public GameObject tabButtonObject;
        public FormPrefabs formAssets;
        public RectTransform uiRectContentParent;
        public RectTransform uiRectTabParent;

        private TabbedContentControl _content;
        [NonSerialized] public SerializedDataForm Form;
        private bool _initialized;
        private GameObject[] _lastFailedObjects;

        public static event Action<StudioItemControl, TabbedContentControl> OnInitialize;
        public static event Action<StudioItemControl, TabbedContentControl> OnOpenMenu;

        public static StudioItemControl Instance { get; set; }

        public static bool TryGetInstance(out StudioItemControl instance)
        {
            instance = Instance;
            return instance != null;
        }

        public SerializedDataForm CreateTab(string key, string title)
        {
            var contentInfo = _content.AddTab<SerializedDataForm>(key, title);
            var form = contentInfo.Manager<SerializedDataForm>();
            form.SetPrefabAssets(formAssets);
            return form;
        }

        public bool internalUpdate;

        private void OnEnable()
        {
#if AI || HS2
            if (!UIIntegration.Loaded) return;
            try
            {
                internalUpdate = true;
                UIIntegration.ToolbarButton.Value = true;
                OnOpenMenu?.Invoke(this, _content);
            }
            finally
            {
                internalUpdate = false;
            }
#endif
        }

        private void OnDisable()
        {
#if AI || HS2
            try
            {
                if (!UIIntegration.Loaded) return;
                internalUpdate = true;
                UIIntegration.ToolbarButton.Value = false;
                ColorPickerControl.Hide();
            }
            finally
            {
                internalUpdate = false;
            }
#endif
        }

        private void Start()
        {
            Instance = this;
            _content = new TabbedContentControl(uiRectContentParent, uiRectTabParent, tabButtonObject);
#if AI || HS2
            uiTextTitle.text = "Studio Item Extended";
#else
            uiTextTitle.text = "Studio Item Extended (TEST)";
#endif
            OnInitialize?.Invoke(this, _content);
            _initialized = true;
            if (_lastFailedObjects != null) MakeForm(_lastFailedObjects);
            _lastFailedObjects = null;
        }

        public void MakeForm(GameObject[] gameObjects)
        {
            ColorPickerControl.Hide();

            if (Form == null || gameObjects == null || gameObjects.Length <= 0)
            {
                if (!_initialized) _lastFailedObjects = gameObjects;
                return;
            }

            var formDataComponents = gameObjects.SelectMany(x => x.GetComponentsInChildren<IFormData>()).ToArray();
            var targetFormDataComponent = formDataComponents.FirstOrDefault();
            if (targetFormDataComponent == null) return;
            {
                var firstComponent = targetFormDataComponent.GetType();
                var targets = formDataComponents
                    .Where(x => x.GetType() == firstComponent)
                    .ToArray();

                if (targets.Length > 0)
                    // ReSharper disable once CoVariantArrayConversion
                    firstComponent.AddForms(Form, targets);
            }
        }

        public void ClearForm()
        {
            if (Form == null) return;
            Form.RemoveAllFields();
        }
    }
}