using System;
using System.Collections.Generic;
using System.Reflection;
using HooahUtility.AdvancedStudioUI;
using HooahUtility.AdvancedStudioUI.Constant;
using HooahUtility.Controller.Components;
using HooahUtility.Model.Attribute;
using HooahUtility.Serialization.StudioReference;
using HooahUtility.Service;
using MessagePack;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;
#if AI || HS2
using HooahUtility.Utility;
using KKAPI.Utilities;
using Studio;
using Utility;
#endif

namespace HooahUtility.Controller.ContentManagers
{
    public partial class SerializedDataForm
    {
        public static void AddCheckField(SerializedDataForm form, MemberInfo memberInfo, object reference,
            object[] targets, Action pre = null, Action post = null)
        {
            form.AddField<CheckComponent>(UIConstant.CheckboxField, memberInfo, reference, targets, pre, post);
        }

        public static void AddEnumDropdown(SerializedDataForm form, MemberInfo memberInfo, object reference,
            object[] targets, Action pre = null, Action post = null)
        {
            form.AddField<DropdownComponent>(UIConstant.DropdownField, memberInfo, reference, targets, pre, post);
        }

        public static void AddTextField(SerializedDataForm form, MemberInfo memberInfo, object reference,
            object[] targets, Action pre = null, Action post = null)
        {
            form.AddField<TextComponent>(UIConstant.InputField, memberInfo, reference, targets, pre, post);
        }

        public static void AddNumericField(SerializedDataForm form, MemberInfo memberInfo, object reference,
            object[] targets, Action pre = null, Action post = null)
        {
            // create various slider fields based on the attributes
            // include options in the attribute
            foreach (var customAttribute in memberInfo.GetCustomAttributes())
            {
                switch (customAttribute)
                {
                    case NumberSpinnerAttribute ___:
                        form.AddField<SpinnerComponent>(UIConstant.NumberSpinnerField, memberInfo, reference, targets,
                            pre, post);
                        return;
                    case RangeAttribute _:
                    case PropertyRangeAttribute __:
                        form.AddField<SliderComponent>(UIConstant.SliderField, memberInfo, reference, targets, pre,
                            post);
                        return;
                }
            }

            form.AddField<TextComponent>(UIConstant.InputField, memberInfo, reference, targets, pre, post);
        }

        public static void AddVectorField(SerializedDataForm form, MemberInfo memberInfo, object reference,
            object[] targets, Action pre = null, Action post = null)
        {
        }

        public static void AddColorField(SerializedDataForm form, MemberInfo memberInfo, object reference,
            object[] targets, Action pre = null, Action post = null)
        {
            form.AddField<ColorPickerComponent>(UIConstant.ColorField, memberInfo, reference, targets, pre, post);
        }

        public static void AddPresetField(SerializedDataForm form, MemberInfo memberInfo, object reference,
            object[] targets, Action pre = null, Action post = null)
        {
        }

        public static void AddExternalReferenceField(SerializedDataForm form, MemberInfo memberInfo, object reference,
            object[] targets, Action pre = null, Action post = null)
        {
            // load predefined list if it requires fixed asset.
            form.AddField<AssetSelectComponent>(UIConstant.ExternalAssetWindow, memberInfo, reference, targets, pre,
                post);
        }

        public List<GameObject> InitializedFields = new List<GameObject>();

        public void RemoveAllFields()
        {
            _height = -margin;
            foreach (var field in InitializedFields)
            {
#if AI || HS2
                field.FancyDestroy();
#else
                Object.Destroy(field);
#endif
            }
        }

        private GameObject InstantiateUIElement(string key, Transform parent = null)
        {
            if (!_prefabReference.TryGetForm(key, out var gameObject)) return null;
            return Object.Instantiate(gameObject, parent);
        }

        public bool InstantiateBase(string key, out GameObject clone)
        {
            clone = null;
            if (!_prefabReference.TryGetForm(key, out var gameObject)) return false;
            clone = Object.Instantiate(gameObject, uiRectTransformParent);
            var rectTransform = clone.GetComponent<RectTransform>();
            _height -= margin; // margin...
            rectTransform.anchoredPosition = new Vector2(0, _height);
            _height -= rectTransform.rect.height;
            return true;
        }

        public T AddFieldComponent<T>(string assetKey)
        {
            if (!InstantiateBase(assetKey, out var clone)) return default(T);
            var component = clone.GetComponent<T>();
            InitializedFields.Add(clone);
            return component;
        }

        public T AddField<T>(string assetKey, MemberInfo info, object reference, object[] targets,
            Action pre = null, Action post = null)
            where T : FormComponentBase
        {
            if (!InstantiateBase(assetKey, out var clone)) return null;
            var component = clone.GetComponent<T>();
            if (component == null) return null;
            component.SetTargetObject(info, reference, targets);

            if (pre != null) component.preUpdateValue += pre;
            if (post != null) component.postUpdateValue += post;

            InitializedFields.Add(clone);
            return component;
        }

        public T AddField<T>(string assetKey, Type type, string key, Action pre = null, Action post = null)
            where T : FormComponentBase
        {
            if (!InstantiateBase(assetKey, out var clone)) return null;
            T component = clone.GetComponent<T>();
            if (component == null) return null;
            component.SetStaticObject(type.GetField(key));

            if (pre != null) component.preUpdateValue += pre;
            if (post != null) component.postUpdateValue += post;

            InitializedFields.Add(clone);
            return component;
        }

        public Button AddButton(string text, UnityAction callback)
        {
            if (!InstantiateBase(UIConstant.ButtonField, out var clone)) return null;
            var uiButton = clone.GetComponentInChildren<Button>();
            var uiText = clone.GetComponentInChildren<TMP_Text>();
            uiButton.onClick.AddListener(callback);
            uiText.text = text;
            InitializedFields.Add(clone);
            return uiButton;
        }

        public struct ButtonArgument
        {
            public string Text;
            public UnityAction Callback;

            public ButtonArgument(string text, UnityAction callback)
            {
                Text = text;
                Callback = callback;
            }
        }

        public void AddOptionButton(string buttonName, string[] options, UnityAction<bool[]> callback)
        {
            if (options == null || options.Length <= 0) return;
            if (!InstantiateBase(UIConstant.ButtonCombined, out var btn)) return;
            var btnComponent = btn.GetComponentInChildren<MultiOptionButtonComponent>();
            btnComponent.SetOptions(buttonName, options, callback);
        }

        private const float ButtonGroupMargin = 5f;


        public void AddButtonGroup(string groupName, params ButtonArgument[] buttons)
        {
            if (buttons == null) return;
            if (buttons.Length <= 0) return;

            if (!InstantiateBase(UIConstant.LabelComponent, out var label)) return;
            var text = label.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = groupName;

            if (!InstantiateBase(UIConstant.ButtonBaseComponent, out var clone)) return;
            var transform = clone.transform;
            var div = 1f / buttons.Length;

            for (var i = 0; i < buttons.Length; i++)
            {
                if (!AssetManager.TryMakeUIPrefab(UIConstant.Button, out var element, transform)) continue;

                var args = buttons[i];
                var uiButton = element.GetComponent<Button>();
                var uiRectTransform = element.GetComponent<RectTransform>();
                var uiText = element.GetComponentInChildren<TMP_Text>();
                UIUtility.ScaleAsGrid(uiRectTransform, i, div, ButtonGroupMargin);
                uiButton.onClick.AddListener(args.Callback);
                uiText.text = args.Text;
            }

            InitializedFields.Add(clone);
        }

        private bool IsConfigurableMember(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo fInfo when
                    fInfo.GetCustomAttribute<KeyAttribute>() != null:
                    return true;
                case PropertyInfo pInfo when
                    pInfo.GetCustomAttribute<KeyAttribute>() != null &&
                    pInfo.CanRead && pInfo.CanWrite: // only rw properties.
                    return true;
                case MethodInfo _:
                    return true;
                default:
                    return false;
            }
        }

        public void RegisterForm(MemberInfo memberInfo, Action preApply, Action postApply)
        {
            if (!IsConfigurableMember(memberInfo)) return;

            switch (memberInfo)
            {
                case FieldInfo fInfo:
                    RegisterFormElement(fInfo, fInfo.FieldType, preApply, postApply);
                    break;
                case PropertyInfo pInfo:
                    RegisterFormElement(pInfo, pInfo.PropertyType, preApply, postApply);
                    break;
                case MethodInfo mInfo:
                    RegisterForm(mInfo);
                    break;
            }
        }

        public void RegisterFormElement<T>(T memberInfo, Type memberType, Action preAssign, Action postAssign)
            where T : MemberInfo
        {
            // TODO: load the field dependency
            switch (memberType)
            {
                case var type when type == typeof(bool):
                    AddCheckField(this, memberInfo, _reference, _targets, preAssign, postAssign);
                    break;
                case var type when type.IsEnum:
                    AddEnumDropdown(this, memberInfo, _reference, _targets, preAssign, postAssign);
                    break;
                case var type when type == typeof(string):
                    AddTextField(this, memberInfo, _reference, _targets, preAssign, postAssign);
                    break;
                case var type when type == typeof(int) || type == typeof(uint) || type == typeof(short) ||
                                   type == typeof(ushort) || type == typeof(long) || type == typeof(ulong) ||
                                   type == typeof(float) || type == typeof(double):
                    AddNumericField(this, memberInfo, _reference, _targets, preAssign, postAssign);
                    break;
                case var type when
                    type == typeof(Vector2Int) || type == typeof(Vector3Int) ||
                    type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4) ||
                    type == typeof(Quaternion):
                    AddVectorField(this, memberInfo, _reference, _targets, preAssign, postAssign);
                    // Equatable?
                    break;
                case var type when type == typeof(Matrix4x4):
                    // Equatable?
                    break;
                case var type when type == typeof(AnimationCurve):
                    // https://assetstore.unity.com/packages/tools/gui/runtime-curve-editor-11835?locale=ko-KR
                    // Load from preset
                    // Save to preset
                    break;
                case var type when type == typeof(Color) || type == typeof(Color32):
                    AddColorField(this, memberInfo, _reference, _targets, preAssign, postAssign);
                    break;
                case var type when type == typeof(Texture) || type == typeof(Material):
                    AddExternalReferenceField(this, memberInfo, _reference, _targets, preAssign, postAssign);
                    break;
                case var type when type == typeof(CharacterReference):
                    AddCharacterReference(this, memberInfo, _reference, _targets);
                    break;
                case var type when type == typeof(StudioObjectReference):
                    AddStudioReference(this, memberInfo, _reference, _targets);
                    break;
                // Studio Reference Proxy
                // External Resource Parser
                //      Type : load and parse - parse 
                //      Type : reference and load - asset bundles
                //          Load from pre-existing material prests
                //          Load from given path
                // Sounds Assets
                // OBJ/FBX to mesh
                default:
                    break;
            }
        }
    }
}