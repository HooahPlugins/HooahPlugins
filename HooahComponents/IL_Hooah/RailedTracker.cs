﻿using System.Collections;
using MessagePack;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;

#if AI || HS2
using HooahComponents;
using HooahUtility.Model;

#endif

#if AI || HS2
public class RailedTracker : ChannelTrackerGimmickBase
#else
public class RailedTracker : MonoBehaviour
#endif
{
    public Transform start;
    public Transform mid;
    public Transform end;
    public Transform target;
    public Transform tracker;
    [Key(0), Range(0, 1)] public float dragLerp = 0.3f;

    public UnityEvent onHitEnd;
    public UnityEvent onHitStart;
    public ushort state;
    private float _factor;
    private Vector3 _targetNormal;
    private Vector3 _targetPosition;

    private void LateUpdate()
    {
#if AI || HS2
        if (!TryFindChannelTarget(tracker, out var realTracker)) return;

        var v = new NativeArray<Vector3>(8, Allocator.TempJob)
        {
            [0] = start.localPosition,
            [1] = mid.localPosition,
            [2] = end.localPosition,
            [3] = target.localPosition,
            [4] = TargetFindingMode == Mode.ChannelMode
                ? transform.InverseTransformPoint(realTracker.position)
                : tracker.localPosition,
            [5] = default,
            [6] = default,
            [7] = Vector3.zero
        };
        new TrackCalculation {v = v, lerp = dragLerp}.Schedule().Complete();

        try
        {
            target.localPosition = v[5];
            target.localRotation = Quaternion.LookRotation(v[6]);
            if (v[7].x <= 0.1 && state != 1)
            {
                onHitStart?.Invoke();
                state = 1;
            }
            else if (v[7].x >= 0.9 && state != 2)
            {
                onHitEnd?.Invoke();
                state = 2;
            }
        }
        finally
        {
            v.Dispose();
        }
#endif
    }

    public struct TrackCalculation : IJob
    {
        public NativeArray<Vector3> v;
        public float lerp;

        public void Execute()
        {
            var t = Mathf.Max(0, Mathf.Min(1, (Mathf.Abs(v[0].x) - v[4].x) / (Mathf.Abs(v[2].x) + Mathf.Abs(v[2].x))));
            var u = 1f - t;
            var tt = t * t;
            var uu = u * u;
            var p = uu * v[0]; //first term
            p += 2 * u * t * v[1]; //second term
            p += tt * v[2]; //third term
            v[5] = Vector3.Lerp(v[3], p, lerp); // targetPosition
            v[6] = 2 * (1 - t) * (v[1] - v[0]) + 2 * t * (v[2] - v[1]); // derivative
            var vec = new Vector3(t, u, 0);
            v[7] = vec;
        }
    }
#if AI || HS2
    protected override IEnumerator DefaultTargetFinder()
    {
        yield break;
    }
#endif
}