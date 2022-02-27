using MessagePack;
using UnityEngine;
using UnityEngine.Events;

#if AI || HS2
using System.Collections;
using HooahComponents;
#endif

#if AI || HS2
public class SmashRailedTracker : ChannelTrackerGimmickBase
#else
public class SmashRailedTracker : MonoBehaviour
#endif
{
#if AI || HS2
    protected override IEnumerator DefaultTargetFinder()
    {
        yield break;
    }
#endif

    public Transform root;
    public Transform child;
    public Transform tracker;

    // the smoothness of smash
    [Key(0), Range(0, 100)] public float dragLerp = 2f;

    // the maximum distance for smash
    [Key(1), Range(0, 50)] public float maxDistance = 2f;

    // start correcting angle for adjusted position
    [Key(2), Range(0.001f, 1)] public float angleCorrectionStart = 1f;

    [Key(10)] public bool useLocal = true;
    [Key(11)] public LookAtAxis axis;

    public enum LookAtAxis
    {
        Up, Down, Right, Left, Forward,
        Backward, Custom
    }

    // Use this for initialization

    private Vector3 direction
    {
        get
        {
            var transform = tracker.transform;
            switch (axis)
            {
                case LookAtAxis.Up:
                    return useLocal ? transform.up : Vector3.up;
                case LookAtAxis.Down:
                    return useLocal ? -transform.up : Vector3.down;
                case LookAtAxis.Right:
                    return useLocal ? transform.right : Vector3.right;
                case LookAtAxis.Left:
                    return useLocal ? -transform.right : Vector3.left;
                case LookAtAxis.Forward:
                    return useLocal ? transform.forward : Vector3.forward;
                case LookAtAxis.Backward:
                    return useLocal ? -transform.forward : Vector3.back;
                default:
                    return transform.up;
            }
        }
    }
    // look at axis?
    // look at axis?

    // curve strength

    public UnityEvent onHitExit;
    public UnityEvent onHitStart;

    private void LateUpdate()
    {
#if AI || HS2
        if (!TryFindChannelTarget(tracker, out var target)) return;
        
        var position = target.position;
        var position1 = root.position;
        var ptrDir = position - position1;
        var mag = ptrDir.magnitude;
        var uncFactor = mag / maxDistance;
        var factor = Mathf.Min(1, uncFactor) * Mathf.Max(0, Vector3.Dot(ptrDir.normalized, root.forward));
        var targetDir = Vector3.Lerp(root.transform.forward, ptrDir, Mathf.Min(1, factor / angleCorrectionStart));
        var transform1 = child.transform;
        if (targetDir != Vector3.zero) transform1.rotation = Quaternion.LookRotation(targetDir);
        transform1.position =
            Vector3.Lerp(
                transform1.position,
                root.transform.position + ptrDir.normalized * factor * maxDistance,
                Time.deltaTime * dragLerp
            );
#endif
    }
}
