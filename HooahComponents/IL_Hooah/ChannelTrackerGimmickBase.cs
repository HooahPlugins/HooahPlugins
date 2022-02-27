using System;
using System.Collections;
using System.Linq;
using HooahUtility.Model;
using UnityEngine;

namespace HooahComponents
{
    public abstract class ChannelTrackerGimmickBase : ChannelTrackerBase
    {
        protected GameObject channelTarget;

        protected abstract IEnumerator DefaultTargetFinder();

        public IEnumerator ChannelTargetFinder()
        {
            if (channelTarget != null) yield return new WaitForSeconds(1f);

            channelTarget = ChannelTarget.GetSingleTargetFromChannel(targetChannel)?.gameObject;
        }

        protected IEnumerator FindTarget()
        {
            while (true)
            {
                yield return new WaitForSeconds(.5f);

                switch (TargetFindingMode)
                {
                    case Mode.Default:
                        yield return DefaultTargetFinder();
                        break;
                    case Mode.ChannelMode:
                        yield return ChannelTargetFinder();
                        break;
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public override void OnTrackingModeChanged(Mode targetFindingMode)
        {
        }

        private void Awake()
        {
            StartCoroutine(nameof(FindTarget));
        }

        private void OnDestroy()
        {
            StopCoroutine(nameof(FindTarget));
        }
    }
}
