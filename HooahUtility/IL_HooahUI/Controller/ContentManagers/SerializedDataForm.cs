using System;
using System.Linq;
using System.Reflection;
using HooahUtility.Editor;
using HooahUtility.Model.Attribute;
#if AI || HS2
using KKAPI.Utilities;

#endif

namespace HooahUtility.Controller.ContentManagers
{
    public static class SerializedDataFormUtility
    {
        // todo: integrate pool system
        // > if form is not registered in the pool
        //   create form in the pool
        //   and move the form to the panel.
        // > if form is registered in the pool
        //   move current form to the virtual pool
        //   move target form to the current panel
        public static void AddForms(this Type type, SerializedDataForm form, object[] targets, Action pre = null,
            Action post = null)
        {
            if (form == null) return;
            if (targets == null || targets.Length == 0) return;
            form.SetTargets(targets);
            foreach (var member in type.GetMembers()) form.RegisterForm(member, pre, post);
            var delta = form.uiRectTransformParent.sizeDelta;
            delta.y = form.Height;
            form.uiRectTransformParent.sizeDelta = delta;
            form.SyncHeight();
        }
    }

    public partial class SerializedDataForm : ContentManager
    {
        public event Action preUpdateValue;
        public event Action postUpdateValue;

        private const float margin = 2.5f;
        private float _height = -margin;

        public float Height
        {
            get => (_height * -1) + margin;
        }

        public void ResetHeight()
        {
            _height = 0;
        }

        private FormPrefabs _prefabReference;
        // public Proxy

        public void SetPrefabAssets(in FormPrefabs prefabReference)
        {
            _prefabReference = prefabReference;
        }

        private object[] _targets = { };
        private object _reference;

        public void SetTarget(object target)
        {
            _reference = target;
            _targets = new[] {target};
        }

        public void SetTargets(object target)
        {
            _reference = target;
            _targets = new[] {target};
        }

        public void SetTargets(object[] targets)
        {
            _reference = targets.FirstOrDefault();
            _targets = targets;
        }

        public void RegisterForm(MethodInfo info)
        {
            var function = info.GetCustomAttribute(typeof(RuntimeFunctionAttribute));
            if (function is RuntimeFunctionAttribute func)
                AddButton(
                    func.name.Length == 0 ? info.Name : func.name, () =>
                    {
                        foreach (var target in _targets) info.Invoke(target, new object[] { });
                    }
                );
        }
    }
}