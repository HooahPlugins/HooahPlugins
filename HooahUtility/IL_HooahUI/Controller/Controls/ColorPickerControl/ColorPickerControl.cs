using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HooahUtility.AdvancedStudioUI.Constant;
using HooahUtility.AdvancedStudioUI.Model.Class;
using HooahUtility.Service;
using HooahUtility.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if AI || HS2
using KKAPI.Utilities;

#else
#endif

namespace AdvancedStudioUI
{
    public partial class ColorPickerControl : WindowBase
    {
        /*
         * Serializable fields for In-game Integrations
         */
        [Header("Object References")] public GameObject HSVObjects;
        public GameObject RGBObjects;
        [Header("Sliders")] public GameObject sliderRed;
        public GameObject sliderAll;
        public GameObject sliderGreen;
        public GameObject sliderBlue;
        public GameObject sliderAlpha;
        public GameObject sliderHue;
        public GameObject sliderSaturation;
        public GameObject sliderValue;
        [Header("Hex Inputs")] public TMP_InputField HexInput;
        public Image HexDisplay;

        public static ColorPickerControl instance;

        public void Awake()
        {
            instance = this;
            AssignPickerReferences();
        }

        public static ColorPickerControl GetInstance()
        {
            if (instance != null) return instance;
            AssetManager.TryMakeUIPrefab(
                UIConstant.ColorPicker,
                out var picker,
                CanvasManager.Canvas.transform);
            instance = picker.GetComponent<ColorPickerControl>();
            return instance;
        }

        public static void Show(Color color, Action<Color> onColorChange)
        {
            var i = GetInstance();
            i.gameObject.SetActive(true);
            i.SetColor(color);
            i.ColorReference.onColorChange = onColorChange;
        }


        /*
         * Private Fields
         */
        [Header("Current Mode - to be private")]
        public MainPickingMode mode;

        private Canvas _canvas;

        private Image _currentFocusElement;
        private PickerType _currentPickerType;
        private MainPickingMode _lastUpdatedMode;

        private bool _useRGB;

        public ColorReference ColorReference;

        /*
         * Main Logics
         */
        public Dictionary<PickerType, PickerData> Pickers = new Dictionary<PickerType, PickerData>
        {
            { PickerType.Main, new PickerData { PickerType = PickerType.Main, Direction = PickerDirection.Universal } },
            { PickerType.R, new PickerData { PickerType = PickerType.R, Direction = PickerDirection.Horizontal } },
            { PickerType.G, new PickerData { PickerType = PickerType.G, Direction = PickerDirection.Horizontal } },
            { PickerType.B, new PickerData { PickerType = PickerType.B, Direction = PickerDirection.Horizontal } },
            {
                PickerType.A,
                new PickerData { PickerType = PickerType.A, Direction = PickerDirection.Horizontal, IsAlpha = true }
            },
            { PickerType.H, new PickerData { PickerType = PickerType.H, Direction = PickerDirection.Horizontal } },
            { PickerType.S, new PickerData { PickerType = PickerType.S, Direction = PickerDirection.Horizontal } },
            { PickerType.V, new PickerData { PickerType = PickerType.V, Direction = PickerDirection.Horizontal } },
            {
                PickerType.Preview,
                new PickerData
                {
                    PickerType = PickerType.Preview, Direction = PickerDirection.None, IsAlpha = true, IsPreview = true
                }
            }
        };

        public bool UseRGB
        {
            get => _useRGB;
            set
            {
                _useRGB = value;
                HSVObjects.gameObject.SetActive(!_useRGB);
                RGBObjects.gameObject.SetActive(_useRGB);
            }
        }

        private void Start()
        {
#if AI || HS2
            uiTextTitle.text = "Color Picker";
#else
            uiTextTitle.text = "Color Picker";
#endif
        }

        public void SetColor(Color color, bool keepInstance = false)
        {
            if (keepInstance)
            {
                ColorReference.TargetColor = color;
                ChangeColorReference();
            }
            else
            {
                ColorReference = new ColorReference(color);
                ChangeColorReference();
            }
        }

        private void OnEnable()
        {
#if AI || HS2
#else
#endif
        }

        private void OnDisable()
        {
#if AI || HS2
#else
#endif
        }

        private void SetPointerFocus(BaseEventData eventData, Image picker, PickerType pickerType)
        {
            _currentFocusElement = picker;
            _currentPickerType = pickerType;
            PointerUpdate(eventData);
        }


        private void PointerUpdate(BaseEventData baseEventData)
        {
            var v = ColorPickerUtility.GetNormalizedPointerPosition(_canvas, _currentFocusElement.rectTransform,
                baseEventData);

            PickColor(ColorReference, _currentPickerType, v);
            ChangeColorReference();
        }

        private void ScrollUpdate(BaseEventData baseEventData)
        {
            var delta = baseEventData.currentInputModule.input.mouseScrollDelta * 0.11f;

            switch (mode)
            {
                case MainPickingMode.HS:
                case MainPickingMode.SH:
                    PickColor1D(ColorReference, PickerType.V,
                        Mathf.Clamp(ColorReference.V + delta.y, 0f, 1f)
                    );
                    break;
                case MainPickingMode.HV:
                case MainPickingMode.VH:
                    PickColor1D(ColorReference, PickerType.S,
                        Mathf.Clamp(ColorReference.S + delta.y, 0f, 1f)
                    );
                    break;
                case MainPickingMode.SV:
                case MainPickingMode.VS:
                    PickColor1D(ColorReference, PickerType.H,
                        Mathf.Repeat(ColorReference.H + delta.y * 0.25f, 1f)
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ChangeColorReference();
        }

        private void ChangeColorReference()
        {
            UpdateMinMax();
            UpdatePreview();
            UpdateMarkers();
            UpdateTextures();
        }

        private void UpdatePreview()
        {
            HexDisplay.color = ColorReference.TargetColor;
            HexInput.text = ColorReference.A < 1f
                ? ColorUtility.ToHtmlStringRGBA(ColorReference.TargetColor)
                : ColorUtility.ToHtmlStringRGB(ColorReference.TargetColor);
        }

        private void UpdateTextures(bool forceUpdate = false)
        {
            foreach (var pickerData in Pickers.Values.Where(pickerData => !forceUpdate || pickerData.IsPreview))
                UpdateTexture(pickerData, false);
        }

        private int GetGradientMode(PickerType type)
        {
            if (!Pickers.TryGetValue(type, out var pickerData)) return default;
            if (type == PickerType.Main) return 2;
            if (type == PickerType.H) return 3;
            return 0;
        }

        private IEnumerable<Tuple<PickerType, GameObject>> GetReferences()
        {
            yield return new Tuple<PickerType, GameObject>(PickerType.Main, sliderAll);
            yield return new Tuple<PickerType, GameObject>(PickerType.R, sliderRed);
            yield return new Tuple<PickerType, GameObject>(PickerType.G, sliderGreen);
            yield return new Tuple<PickerType, GameObject>(PickerType.B, sliderBlue);
            yield return new Tuple<PickerType, GameObject>(PickerType.H, sliderHue);
            yield return new Tuple<PickerType, GameObject>(PickerType.S, sliderSaturation);
            yield return new Tuple<PickerType, GameObject>(PickerType.V, sliderValue);
            yield return new Tuple<PickerType, GameObject>(PickerType.A, sliderAlpha);
            var o = HexDisplay.gameObject;
            yield return new Tuple<PickerType, GameObject>(PickerType.PreviewAlpha, o);
            yield return new Tuple<PickerType, GameObject>(PickerType.Preview, o);
        }

        private void UpdateMarkers()
        {
            foreach (var pickerData in Pickers.Values)
            {
                if (pickerData.Direction == PickerDirection.None) continue;
                var image = pickerData.SliderImage;
                if (!image || !image.isActiveAndEnabled) continue;
                var type = pickerData.PickerType;
                var v = GetValue(type);
                UpdateMarker(type, v);
            }
        }

        private void UpdateMarker(PickerType type, Vector2 v)
        {
            if (!Pickers.TryGetValue(type, out var pickerData)) return;
            var marker = pickerData.MarkerTransform;
            marker.gameObject.SetActive(true);
            var parent = pickerData.Transform;
            var parentSize = parent.rect.size;
            var localPos = marker.localPosition;

            var pivot = parent.pivot;
            switch (pickerData.Direction)
            {
                case PickerDirection.Vertical:
                    localPos.y = (v.y - pivot.y) * parentSize.y;
                    break;
                case PickerDirection.Horizontal:
                    localPos.x = (v.x - pivot.x) * parentSize.x;
                    break;
                case PickerDirection.Universal:
                    localPos.x = (v.x - pivot.x) * parentSize.x;
                    localPos.y = (v.y - pivot.y) * parentSize.y;
                    break;
                default:
                    break;
            }

            marker.localPosition = localPos;
        }

        private void AssignPickerReferences()
        {
            _canvas = GetComponentInParent<Canvas>();
            foreach (var reference in GetReferences())
            {
                var pickerType = reference.Item1;
                var sliderObject = reference.Item2;
                if (!Pickers.TryGetValue(pickerType, out var p)) continue;

                p.Transform = sliderObject.GetComponent<RectTransform>();
                p.MarkerTransform = sliderObject.GetComponentsInChildren<RectTransform>()
                    .FirstOrDefault(x => x.transform.name.Contains("DragArea"));

                if (p.MarkerTransform is null) continue;
                p.SliderImage = sliderObject.GetComponent<Image>();
                var trigger = sliderObject.AddComponent<EventTrigger>();

                trigger.triggers.Add(FocusEventEntry(p.SliderImage, pickerType));
                trigger.triggers.Add(DragUpdateEntry());
                trigger.triggers.Add(ScrollUpdateEntry());

                p.EventTrigger = trigger;
                Pickers[pickerType] = p;
            }

            HexInput.onEndEdit.AddListener(ParseColor);
            HexInput.onSubmit.AddListener(ParseColor);
        }

        private void ParseColor(string value)
        {
            if (Utility.ColorUtility.ParseColorString(value, out var color)) SetColor(color, true);
        }

        private bool _isUpdating = false;

        public void ChangeMode(int pickingMode)
        {
            if (_isUpdating) return;

            _isUpdating = true;
            switch (pickingMode)
            {
                case 0:
                case 1:
                    mode = (MainPickingMode)pickingMode;
                    break;
                case 2:
                    mode = MainPickingMode.SV;
                    break;
            }

            UpdateMarkers();
            UpdateTextures();
            _isUpdating = false;
        }

        public void ChangeGroup(bool isHSV)
        {
            if (_isUpdating) return;

            _isUpdating = true;
            if (isHSV)
            {
                RGBObjects.SetActive(true);
                HSVObjects.SetActive(false);
            }
            else
            {
                RGBObjects.SetActive(false);
                HSVObjects.SetActive(true);
            }

            UpdateMarkers();
            UpdateTextures();
            _isUpdating = false;
        }

        public static void Hide()
        {
            var i = GetInstance();
            i.gameObject.SetActive(false);
        }
    }
}