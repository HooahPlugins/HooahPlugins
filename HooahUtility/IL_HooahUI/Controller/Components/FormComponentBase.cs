using System;
using System.Collections.Generic;
using System.Reflection;
using HooahUtility.Model.Attribute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace HooahUtility.Controller.Components
{
    public abstract class FormComponentBase : MonoBehaviour, IFormComponent
    {
        protected static readonly Dictionary<Type, TMP_InputField.ContentType> InputFieldContentTypeInfo =
            new Dictionary<Type, TMP_InputField.ContentType>
            {
                {typeof(short), TMP_InputField.ContentType.IntegerNumber},
                {typeof(int), TMP_InputField.ContentType.IntegerNumber},
                {typeof(long), TMP_InputField.ContentType.IntegerNumber},
                {typeof(ushort), TMP_InputField.ContentType.IntegerNumber},
                {typeof(uint), TMP_InputField.ContentType.IntegerNumber},
                {typeof(ulong), TMP_InputField.ContentType.IntegerNumber},
                {typeof(float), TMP_InputField.ContentType.DecimalNumber},
                {typeof(double), TMP_InputField.ContentType.DecimalNumber},
            };

        protected static readonly Dictionary<Type, float> DesignatedMinimumValue =
            new Dictionary<Type, float>
            {
                {typeof(ushort), Int16.MinValue},
                {typeof(uint), Int32.MinValue},
                {typeof(ulong), Int64.MinValue},
                {typeof(short), Int16.MinValue},
                {typeof(int), Int32.MinValue},
                {typeof(long), Int64.MinValue},
                {typeof(float), Single.MinValue},
                {typeof(double), (float) Double.MinValue},
            };

        protected static readonly Dictionary<Type, float> DesignatedMaximumValue =
            new Dictionary<Type, float>
            {
                {typeof(short), Int16.MaxValue},
                {typeof(int), Int32.MaxValue},
                {typeof(long), Int64.MaxValue},
                {typeof(ushort), Int16.MaxValue},
                {typeof(uint), Int32.MaxValue},
                {typeof(ulong), Int64.MaxValue},
                {typeof(float), Single.MaxValue},
                {typeof(double), (float) Double.MaxValue},
            };

        protected MemberInfo MemberInfo { get; private set; }

        public bool RegisterFieldInfo(MemberInfo info)
        {
            if (info == null)
            {
                Destroy(this);
                return false;
            }

            MemberInfo = info;
            ParseAttribute();
            SetTitle();

            return true;
        }

        public TMP_Text title;
        public event Action preUpdateValue;
        public event Action postUpdateValue;

        public void SetTitle()
        {
            var nameAttribute = MemberInfo.GetCustomAttribute<FieldNameAttribute>();
            title.text = nameAttribute == null ? MemberInfo.Name.ToProperCase() : nameAttribute.name;
        }

        public virtual void ParseAttribute()
        {
        }

        protected object Reference { get; private set; }
        protected object[] Targets { get; private set; }

        protected bool IsStatic { get; private set; }

        public void SetTargetObject(MemberInfo info, in object reference, in object[] targets)
        {
            if (!RegisterFieldInfo(info)) return;
            Reference = reference;
            Targets = targets;
            AssignValues();
        }

        public void SetStaticObject(FieldInfo info)
        {
            if (!RegisterFieldInfo(info)) return;
            IsStatic = true;
            AssignValues();
        }

        public abstract void AssignValues();

        protected object GetValueInternal()
        {
            var target = IsStatic ? null : Reference;
            switch (MemberInfo)
            {
                case FieldInfo fieldInfo:
                    return fieldInfo.GetValue(target);
                case PropertyInfo propertyInfo:
                    return propertyInfo.GetValue(target);
                default:
                    return null;
            }
        }

        protected virtual T GetValue<T>() => (T) Convert.ChangeType(GetValueInternal(), typeof(T));

        protected virtual string GetTextValue() => GetValueInternal().ToString();

        protected void SetUIValue(TMP_InputField ui) => ui.text = GetTextValue();

        protected void SetUIValue(Slider slider) => slider.value = GetValue<float>();

        protected void SetUIValue(TMP_Dropdown dropdown) => dropdown.value = GetValue<int>();

        protected void SetUIValue(Toggle toggle) => toggle.isOn = GetValue<bool>();

        private bool _updating;

        private void SetMemberValue(Type type, object value, object instance = null)
        {
            var convertedValue = Convert.ChangeType(value, type);
            switch (MemberInfo)
            {
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(instance, convertedValue);
                    break;
                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(instance, convertedValue);
                    break;
            }
        }

        private void SetMemberValue<T>(object value, object instance = null) =>
            SetMemberValue(typeof(T), value, instance);

        private void SetRawMemberValue<T>(T value, object instance = null) 
        {
            switch (MemberInfo)
            {
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(instance, value);
                    break;
                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(instance, value);
                    break;
            }
        }

        protected Type MemberType
        {
            get
            {
                switch (MemberInfo)
                {
                    case FieldInfo fieldInfo:
                        return fieldInfo.FieldType;
                    case PropertyInfo propertyInfo:
                        return propertyInfo.PropertyType;
                    default:
                        return null;
                }
            }
        }

        protected void SetValue(Type t, object value, Action callback)
        {
            if (_updating) return;
            _updating = true;
            try
            {
                preUpdateValue?.Invoke();

                if (IsStatic) SetMemberValue(t, value);
                else
                    foreach (var target in Targets)
                        SetMemberValue(t, value, target);

                callback();

                postUpdateValue?.Invoke();
            }
            finally
            {
                _updating = false;
            }
        }

        protected void SetValue<T>(T value, Action callback)
        {
            if (_updating) return;
            _updating = true;
            try
            {
                preUpdateValue?.Invoke();

                if (IsStatic) SetMemberValue<T>(value);
                else
                    foreach (var target in Targets)
                        SetMemberValue<T>(value, target);

                callback();

                postUpdateValue?.Invoke();
            }
            finally
            {
                _updating = false;
            }
        }
        
        protected void SetRawValue<T>(T value, Action callback)
        {
            if (_updating) return;
            _updating = true;
            try
            {
                preUpdateValue?.Invoke();

                if (IsStatic) SetRawMemberValue(value);
                else
                    foreach (var target in Targets)
                        SetRawMemberValue(value, target);

                callback();

                postUpdateValue?.Invoke();
            }
            finally
            {
                _updating = false;
            }
        }
    }
}