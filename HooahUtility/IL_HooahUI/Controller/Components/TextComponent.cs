using TMPro;

namespace HooahUtility.Controller.Components
{
    public class TextComponent : FormComponentBase
    {
        public TMP_InputField input;

        private void SetValue(string value)
        {
            SetValue(MemberType, value, () => { SetUIValue(input); });
        }

        public override void AssignValues()
        {
            SetUIValue(input);
            if (InputFieldContentTypeInfo.TryGetValue(MemberType, out var contentType))
                input.contentType = contentType;

            input.onSubmit.AddListener(SetValue);
        }
    }
}