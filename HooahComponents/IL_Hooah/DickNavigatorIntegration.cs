using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DickNavigatorIntegration : MonoBehaviour
{
    public SkinnedMeshRenderer[] renderer;
    public AnimationCurve[] curves;
    public int[] indices;
    public DickNavigator navigator;
    public float sizeWeight = 1f;

    void Update()
    {
        if (navigator == null || renderer == null || curves == null || indices == null) return;
        var f = navigator.factor;
        for (var i = 0; i < curves.Length; i++)
        {
            var animationCurve = curves[i];
            var power = animationCurve.Evaluate(f) * 100;
            if (power > 0) power *= sizeWeight;

            foreach (var skinnedMeshRenderer in renderer) 
                if (skinnedMeshRenderer.gameObject.activeSelf)
                    skinnedMeshRenderer.SetBlendShapeWeight(indices[i], power);
        }
    }
}