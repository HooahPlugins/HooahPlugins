using System.Collections;
using System.Linq;
using HooahComponents;
using MessagePack;
using UnityEngine;

public class LookAtIt : ChannelTrackerGimmickBase
{
    public GameObject turnObject;

    [Key(0)] public LookAtAxis axis;

    [Key(1)] public bool useLocal;

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

    // Update is called once per frame
    private void Update()
    {
        switch (TargetFindingMode)
        {
            case Mode.Default:
                if (channelTarget != null) turnObject.transform.LookAt(channelTarget.transform, direction);
                break;
            case Mode.ChannelMode:
                if (TryFindChannelTarget(transform, out var lookAtTarget))
                    turnObject.transform.LookAt(lookAtTarget.transform, direction);
                break;
        }
    }

    protected override IEnumerator DefaultTargetFinder()
    {
        if (channelTarget != null && channelTarget.GetComponentsInParent<LookAtIt>().FirstOrDefault(x => x == this) != this)
        {
            channelTarget = null;
            yield break;
        }

        channelTarget = GetComponentInChildren<LookAtMe>()?.gameObject;
    }
}