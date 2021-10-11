using HooahUtility.Model.Attribute;
using MessagePack;
using UnityEngine;

namespace HooahComponents
{
    public abstract class ChannelTrackerBase : MonoBehaviour
    {
        public enum Mode { Default, ChannelMode }

        private Mode _mode;

        [Key(1000)]
        public Mode TargetFindingMode
        {
            get => _mode;
            set
            {
                OnTrackingModeChanged(_mode);
                _mode = value;
            }
        }

        [Key(1001), NumberSpinner] public uint targetChannel = 0;


        private ChannelTarget _currentChannelTarget;
        private Transform _currentTransform;

        protected bool TryFindChannelTarget(in Transform defaultTransform, out Transform target)
        {
            target = defaultTransform;

            if (!ReferenceEquals(null, _currentChannelTarget) && _currentChannelTarget.Channel != targetChannel)
            {
                _currentChannelTarget = null;
                _currentTransform = null;
            }

            if (ReferenceEquals(null, _currentChannelTarget))
                _currentChannelTarget = ChannelTarget.GetSingleTargetFromChannel(targetChannel);

            if (_currentChannelTarget == null) return ReferenceEquals(null, target);
            _currentTransform = _currentChannelTarget.transform;
            target = _currentTransform;
            return true;
        }

        protected void ResetChannelTargets()
        {
            _currentChannelTarget = null;
            _currentTransform = null;
        }

        public abstract void OnTrackingModeChanged(Mode targetFindingMode);
    }
}