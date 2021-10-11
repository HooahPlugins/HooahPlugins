using System.Collections.Generic;
using HooahUtility.Model;
using UniRx;
using UnityEngine;
using IK = HooahComponents.Hooks.IK;
#if AI || HS2
using System;
using System.Linq;
using HooahUtility.Utility;
using MessagePack;
using RootMotion.FinalIK;
using Utility;

#endif

#if AI || HS2
public class IKAnchor : CharacterGimmickBase, IFormData
#else
public class IKAnchor : MonoBehaviour, IFormData
#endif
{
#if AI || HS2
    public enum BindingMethod
    {
        None, Torso, Pelvis, Hip, Waist,
        HandL, HandR, HandLR, FootLR, FootL,
        FootR
    }

    private IKSolverFullBodyBiped _ik;
    private Dictionary<string, Transform> _ikGuideObjects;
    private BindingMethod _bindingMethod;


    [Key(0)]
    public BindingMethod IKBindingMethod
    {
        get => _bindingMethod;
        set { _bindingMethod = value; }
    }

    [Key(2)] public bool useRotate;
    private IDisposable _observ;


    protected override void OnChangeCharacterReference()
    {
        if (!IsReferenceValid()) return;
        _ikGuideObjects = ObjectControlInfoUtility.GetIKTargets(TargetOci)
            .ToDictionary(x => x.baseObject.name, x => x.targetObject);
        RenewWatch();
    }

    private void RenewWatch()
    {
        _ik = TargetOci.finalIK.solver;
        // This is better.
        _observ?.Dispose();
        _observ = CharacterUtility.ObservableIKPreSolve(this, _ik).Subscribe(_ => UpdatePosition());
    }

    private void ManipulateShoulderEffector(IKSolverFullBodyBiped solver, bool isLeft, Vector3 p, Vector3 r)
    {
        var e = isLeft ? solver.leftShoulderEffector : solver.rightShoulderEffector;
        e.position = isLeft ? p + -r : p + r;
    }

    private void ManipulateThighEffector(IKSolverFullBodyBiped solver, bool isLeft, Vector3 p, Vector3 r)
    {
        var e = isLeft ? solver.leftThighEffector : solver.rightThighEffector;
        e.position = isLeft ? p + -r : p + r;
    }

    private void ManipulateHandsEffector(IKSolverFullBodyBiped solver, bool isLeft, Vector3 p, Vector3 r, Quaternion rt)
    {
        var e = isLeft ? solver.leftHandEffector : solver.rightHandEffector;
        e.position = isLeft ? p + -r : p + r;
        if (useRotate) e.GetNode(solver).solverRotation = rt;
    }

    private void ManipulateFootEffector(IKSolverFullBodyBiped solver, bool isLeft, Vector3 p, Vector3 r, Quaternion rt)
    {
        var e = isLeft ? solver.leftFootEffector : solver.rightFootEffector;
        e.position = isLeft ? p + -r : p + r;
        if (useRotate) e.GetNode(solver).solverRotation = rt;
    }

    private void UpdatePosition()
    {
        if (!IsReferenceValid() || _ikGuideObjects == null || _ikGuideObjects.Count == 0) return;

        var t = transform;
        var r = t.right * t.localScale.x;
        var p = t.position;
        var rt = t.rotation;
        var solver = TargetCharacter.fullBodyIK.solver;
        switch (_bindingMethod)
        {
            case BindingMethod.None:
                break;
            case BindingMethod.Torso:
                ManipulateShoulderEffector(solver, true, p, r);
                ManipulateShoulderEffector(solver, false, p, r);
                break;
            case BindingMethod.Pelvis:
                ManipulateThighEffector(solver, true, p, r);
                ManipulateThighEffector(solver, false, p, r);
                solver.bodyEffector.position = p + transform.up;
                break;
            case BindingMethod.Hip:
                ManipulateThighEffector(solver, true, p, r);
                ManipulateThighEffector(solver, false, p, r);
                break;
            case BindingMethod.Waist:
                solver.bodyEffector.position = p;
                break;
            case BindingMethod.HandL:
                ManipulateHandsEffector(solver, true, p, Vector3.zero, rt);
                break;
            case BindingMethod.HandR:
                ManipulateHandsEffector(solver, false, p, Vector3.zero, rt);
                break;
            case BindingMethod.HandLR:
                ManipulateHandsEffector(solver, true, p, r, rt);
                ManipulateHandsEffector(solver, false, p, r, rt);
                break;
            case BindingMethod.FootLR:
                ManipulateFootEffector(solver, true, p, r, rt);
                ManipulateFootEffector(solver, false, p, r, rt);
                break;
            case BindingMethod.FootL:
                ManipulateFootEffector(solver, true, p, Vector3.zero, rt);
                break;
            case BindingMethod.FootR:
                ManipulateFootEffector(solver, false, p, Vector3.zero, rt);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
#endif
}