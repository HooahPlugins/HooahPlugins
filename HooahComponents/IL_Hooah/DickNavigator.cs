using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using MessagePack;
#if AI || HS2
using HarmonyLib;
using HooahUtility.Model;
#endif

#if AI || HS2
public class DickNavigator : MonoBehaviour, IFormData
#else
public class DickNavigator : MonoBehaviour
#endif
{
    // you don't want to find whole scene to find navigator instances.
    public static List<DickNavigator> Instances = new List<DickNavigator>();

    // These two cannot be null.
    public Transform dickMidPoint;
    public Transform dickEndPoint;
    [NonSerialized] public float IntegrationFactor;
    [NonSerialized] public float IntegrationFactorUncap;


    [Header("Pregmod Offset"), Key(10)] public bool pmiEnabled = false;

    [Header("Bulge Start Depth"), Key(0), Range(0f, 1f)]
    public float pmiOffset = 0.5f;

    [Header("Bulge Sensitivity"), Key(1), Range(0.1f, 4f)]
    public float pmiDepth = 0.2f;

    [Header("Bulge Multiplier"), Key(2), Range(0.01f, 100f)]
    public float pmiInflationMultiplier = 30f;

    private void Start()
    {
        Instances.Add(this);
    }

    private void OnDestroy()
    {
        Instances.Remove(this);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(dickMidPoint.transform.position, .1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(dickEndPoint.transform.position, .1f);
    }
#else

    #region Reflection Type Reference

    private static readonly Type PregPlusControllerType;
    private static readonly FieldInfo PregPlugControllerDataField;
    private static readonly FieldInfo PregPlusInflationField;
    private static readonly MethodInfo MeshInflateMethod;
    private static readonly ConstructorInfo MeshInflateTypeConstructor;

#if AI || HS2
    static DickNavigator()
    {
        // Static Type/Field/Method Reflection.
        PregPlusControllerType = AccessTools.TypeByName("PregnancyPlusCharaController");
        var pregPlusDataType = AccessTools.TypeByName("PregnancyPlusData");
        PregPlugControllerDataField = PregPlusControllerType.GetField("infConfig");
        PregPlusInflationField = pregPlusDataType.GetField("inflationSize");
        var meshInflateFlagType = AccessTools.TypeByName("MeshInflateFlags");
        MeshInflateMethod =
            PregPlusControllerType.GetMethod("MeshInflate", new[] { meshInflateFlagType, typeof(string) });
        MeshInflateTypeConstructor = meshInflateFlagType.GetConstructors().FirstOrDefault();
    }
#endif

    #endregion

    #region Pregmod Integraiton Context

    private const string IntegrationID = "hooah_DN_Integration";
    private float _lastMorphValue;

    private static bool IsPmIntegrationValid() =>
        // is all reflection references valid?
        !(
            PregPlusControllerType == null ||
            PregPlugControllerDataField == null ||
            PregPlusInflationField == null ||
            MeshInflateMethod == null ||
            MeshInflateTypeConstructor == null
        );

    private void LateUpdate()
    {
        if (pmiEnabled) return;
        if (!IsPmIntegrationValid() || _pregmodController == null) return;
        
        #region Insertion Intensity
        var value = Mathf.Min(1, Mathf.Max(0, IntegrationFactorUncap - pmiOffset) / pmiDepth) * pmiInflationMultiplier;
        if (Math.Abs(_lastMorphValue - value) < 0.01f) return;
        #endregion
        
        #region Pregmod Integration
        var infConfig = PregPlugControllerDataField.GetValue(_pregmodController);
        PregPlusInflationField.SetValue(infConfig, value);
        _lastMorphValue = value;
        var flags = MeshInflateTypeConstructor.Invoke(new[] { _pregmodController, null, null, null, null, null, null });
        MeshInflateMethod.Invoke(_pregmodController, new[] { flags, IntegrationID });
        #endregion
    }


    private object _pregmodController;

    public void OnTransformParentChanged()
    {
        if (IsPmIntegrationValid()) _pregmodController = GetComponentInParent(PregPlusControllerType);
    }

    #endregion


#endif
}
