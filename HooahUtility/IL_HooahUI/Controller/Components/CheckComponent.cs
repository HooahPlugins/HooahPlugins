using UnityEngine.UI;

namespace HooahUtility.Controller.Components
{
    public class CheckComponent : FormComponentBase
    {
        public Toggle inputToggle;

        private void SetValue(bool value)
        {
            SetValue(value, () => { SetUIValue(inputToggle); });
        }

        public override void AssignValues()
        {
            SetUIValue(inputToggle);
            inputToggle.onValueChanged.AddListener(SetValue);
        }
    }
}