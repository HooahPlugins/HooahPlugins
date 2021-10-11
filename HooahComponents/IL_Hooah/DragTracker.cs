using System;
using HooahComponents;
using MessagePack;
using UniRx;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Serialization;
#if AI || HS2
using HooahComponents;
using HooahUtility.Model;

public class DragTracker : ChannelTrackerBase, IFormData
#else
public class DragTracker : ChannelTrackerBase
#endif
{
    public enum Behavior { Lerp, VelocityApporach, Instant }

    // TODO: create character reference
    public Transform parent;
    public Transform tracker;

    [Range(0.001f, 100f), Key(0)] public float approachSpeed = 30f;
    [Range(0.001f, 100f), Key(5)] public float velocitySpeed = .5f;
    [Key(6), Range(0.1f, 10)] public float velocityPower = 2f;
    [Key(7), Range(0.1f, 1000)] public float maximumVelocity = 1000f;
    [Key(1)] public bool addVelocity = false;
    [Key(2)] public float velocityMultiplier = 1;
    [Key(3)] public Behavior mode;

    private Vector3 _previousPosition;
    private Vector3 _momentum;

    private Transform Target
    {
        get
        {
            switch (TargetFindingMode)
            {
                case Mode.ChannelMode:
                    TryFindChannelTarget(tracker, out var target);
                    return target;
                default:
                    return tracker;
            }
        }
    }

    private void Awake()
    {
        Observable.EveryGameObjectUpdate()
            .TakeUntilDestroy(this)
            .Subscribe(_ => { DragUpdate(); });
    }

    private void DragUpdate()
    {
        var target = Target;
        if (!isActiveAndEnabled || parent == null || target == null) return;

        var targetPosition = target.position;
        var parentTransform = parent.transform;
        if (addVelocity)
        {
            var velocity = (_previousPosition - targetPosition) * velocityMultiplier;
            targetPosition += velocity;
        }

        switch (mode)
        {
            case Behavior.Lerp:
                parentTransform.position = Vector3.Lerp(
                    parentTransform.position,
                    targetPosition,
                    approachSpeed * Time.deltaTime
                );
                break;
            case Behavior.VelocityApporach:
                var position = parentTransform.position;
                var direction = targetPosition - position;
                var distance = direction.sqrMagnitude * velocityPower;
                var velocity = direction.normalized * Mathf.Min(maximumVelocity, distance);
                _momentum = Vector3.Lerp(_momentum, velocity, velocitySpeed * Time.deltaTime);

                position += _momentum;
                parentTransform.position = position;
                break;
            case Behavior.Instant:
                parentTransform.position = targetPosition;
                break;
        }


        _previousPosition = target.position;
    }

    public override void OnTrackingModeChanged(Mode targetFindingMode)
    {
        ResetChannelTargets();
    }
}