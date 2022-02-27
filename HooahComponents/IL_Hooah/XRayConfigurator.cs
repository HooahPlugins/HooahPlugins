using HooahUtility.Model;
using HooahUtility.Model.Attribute;
using MessagePack;
using UnityEngine;

public class XRayConfigurator : HooahBehavior
{
    // the length of the xray path
    public float depth = 1f;

    // the scale of vagina guide
    private float _tightness = 1f;
#if AI || HS2
    [Key(0), PropertyRange(0.1f, 15f)]
    public float Tightness
    {
        get => _tightness;
        set
        {
            _tightness = value;
            var scale = vagGuideObject.localScale;
            scale.x = value;
            vagGuideObject.localScale = scale;
        }
    }
#endif

    // visualize steepness of xray object
    private float _visualDepth = 1f;
#if AI || HS2
    [Key(1), PropertyRange(0.0f, 1.0f)]
    public float VisualDepth
    {
        get => _visualDepth;
        set
        {
            _visualDepth = value;
            startObject.localPosition = Vector3.Lerp(startPosA, startPosB, _visualDepth);
            endObject.localPosition = Vector3.Lerp(endPosA, endPosB, _visualDepth);
        }
    }
#endif

    // serialized field
    [SerializeField] private Vector3 startPosA;
    [SerializeField] private Vector3 startPosB;
    [SerializeField] private Vector3 endPosA;
    [SerializeField] private Vector3 endPosB;
    [SerializeField] private Transform vagGuideObject;
    [SerializeField] private Transform endObject;
    [SerializeField] private Transform startObject;
}
