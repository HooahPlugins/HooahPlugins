using System.Reflection;
using HooahUtility.AdvancedStudioUI.Constant;
using HooahUtility.Controller.Components;
using HooahUtility.Serialization.StudioReference;
using HooahUtility.Utility;
using Utility;
#if HS2 || AI
using Studio;
#endif

namespace HooahUtility.Controller.ContentManagers
{
    public partial class SerializedDataForm
    {
        // Todo: this is too ugly and lowering my self-esteem
        public static void AddStudioReference(SerializedDataForm form, MemberInfo memberInfo, object reference,
            object[] targets)
        {
#if HS2 || AI
            var listSpinnerComponent = form.AddFieldComponent<ListSpinnerComponent>(UIConstant.ListSpinnerField);

            listSpinnerComponent.CallbackSetValue = value =>
            {
                if (targets.Length > 1)
                {
                    foreach (var target in targets)
                    {
                        if (!memberInfo.TryCastMember<StudioObjectReference>(target, out var chaRef)) continue;
                        chaRef.DicKey = value;
                        listSpinnerComponent.SetText(chaRef.Name);
                    }
                }
                else
                {
                    if (!memberInfo.TryCastMember<StudioObjectReference>(reference, out var itemRef)) return;
                    itemRef.DicKey = value;
                    listSpinnerComponent.SetText(itemRef.Name);
                }
            };
            listSpinnerComponent.CallbackGetValue = () =>
            {
                if (memberInfo.TryCastMember<CharacterReference>(reference, out var itemRef))
                    return itemRef.DicKey;
                return -1;
            };
            listSpinnerComponent.CallbackGetNextValue = StudioReferenceUtility.GetNextTypeKey<OCIFolder>;
            listSpinnerComponent.CallbackGetPrevValue = StudioReferenceUtility.GetPrevTypeKey<OCIFolder>;
            listSpinnerComponent.Initialize();
            listSpinnerComponent.SetTitle(memberInfo);

            if (memberInfo.TryCastMember<StudioObjectReference>(reference, out var cr))
                listSpinnerComponent.SetText(cr.Name);
            else
                listSpinnerComponent.SetText("Not Assigned");
#endif
        }

        public static void AddCharacterReference(SerializedDataForm form, MemberInfo memberInfo, object reference,
            object[] targets)
        {
#if HS2 || AI
            var listSpinnerComponent = form.AddFieldComponent<ListSpinnerComponent>(UIConstant.ListSpinnerField);

            listSpinnerComponent.CallbackSetValue = value =>
            {
                if (targets.Length > 1)
                {
                    foreach (var target in targets)
                    {
                        if (!memberInfo.TryCastMember<CharacterReference>(target, out var chaRef)) continue;
                        chaRef.DicKey = value;
                        listSpinnerComponent.SetText(chaRef.CharName);
                    }
                }
                else
                {
                    if (!memberInfo.TryCastMember<CharacterReference>(reference, out var chaRef)) return;
                    chaRef.DicKey = value;
                    listSpinnerComponent.SetText(chaRef.CharName);
                }
            };
            listSpinnerComponent.CallbackGetValue = () =>
            {
                if (memberInfo.TryCastMember<CharacterReference>(reference, out var chaRef))
                    return chaRef.DicKey;
                return -1;
            };
            listSpinnerComponent.CallbackGetNextValue = StudioReferenceUtility.GetNextTypeKey<OCIChar>;
            listSpinnerComponent.CallbackGetPrevValue = StudioReferenceUtility.GetPrevTypeKey<OCIChar>;
            listSpinnerComponent.Initialize();
            listSpinnerComponent.SetTitle(memberInfo);

            if (memberInfo.TryCastMember<CharacterReference>(reference, out var cr))
                listSpinnerComponent.SetText(cr.CharName);
            else
                listSpinnerComponent.SetText("Not Assigned");
#endif
        }
    }
}