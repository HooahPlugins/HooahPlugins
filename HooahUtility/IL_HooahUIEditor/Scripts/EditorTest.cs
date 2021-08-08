using System.Collections;
using AdvancedStudioUI;
using HooahUtility.Model;
using HooahUtility.Model.Attribute;
using HooahUtility.Service;
using MessagePack;
using UnityEngine;

public class EditorTest : MonoBehaviour, IFormData
{
    private IEnumerator Start()
    {
        CanvasManager.InitializeCanvas();
        yield return new WaitUntil(() => StudioItemControl.Instance != null);
        var self = StudioItemControl.Instance;
        var form = self.CreateTab("studioObject", "Studio Object");
        self.Form = form;
        StudioItemControl.Instance.MakeForm(new[] {gameObject});
    }

    public enum Mode { Cringe, Based, Chad, }

    // aThe form initializer should initialize the form
    [Range(5, 50)] public int integerValue = 10;
    [Range(0, 1)] public float floatValue = 0.5f;
    public string stringValue = "";
    public float floatTextValue = 10f;
    public uint uintValue = 0;
    public ulong ulongValue = 0;
    public double doubleValue = 0;
    public Mode enumValue;
    public Light light;
    [Key(9)] public bool checkValue;
    private Color _color = Color.white;

    [Key(10)]
    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            if (light != null) light.color = _color;
        }
    }

    [Key(11)]
    public Texture Texture
    {
        get => light.cookie;
        set => light.cookie = value;
    }

    [RuntimeFunction("Run Me Please?")]
    public void RunMe()
    {
        Debug.Log("Nice");
    }

    [RuntimeFunction("Run Me Please?")]
    public void RunMe2()
    {
        Debug.Log("Nice");
    }

    [RuntimeFunction("Run Me Please?")]
    public void RunMe3()
    {
        Debug.Log("Nice");
    }
}