using HooahUtility.Serialization;
using TMPro;
using UnityEngine.UI;


public class WindowBase : HooahSerializer
{
    public TMP_Text uiTextTitle;
    public Button uiButtonClose;

#if UNITY_EDITOR
    public void AssignElements()
    {
    }
#endif
}
