using System.Collections.Generic;
using System.Linq;
using HooahUtility.Model;
using HooahUtility.Model.Attribute;
using MessagePack;
using UnityEngine;

namespace HooahComponents
{
    [DefaultExecutionOrder(1002)]
    public class ChannelTarget : MonoBehaviour, IFormData
    {
        public static HashSet<ChannelTarget> Instances;
        public static Dictionary<uint, HashSet<ChannelTarget>> ChannelInstances;

        private uint _channel = 0;

        [Key(0), NumberSpinner]
        public uint Channel
        {
            set
            {
                SetChannel(this, _channel, false);
                _channel = value;
                SetChannel(this, _channel);
            }
            get => _channel;
        }

        private static void SetChannel(ChannelTarget target, uint channel, bool add = true)
        {
            if (ReferenceEquals(null, ChannelInstances))
                ChannelInstances = new Dictionary<uint, HashSet<ChannelTarget>>();

            if (!ChannelInstances.TryGetValue(channel, out var channelTargets))
            {
                channelTargets = new HashSet<ChannelTarget>();
                ChannelInstances[channel] = channelTargets;
            }

            if (add) channelTargets.Add(target);
            else channelTargets.Remove(target);
        }

        private static void RegisterInstance(ChannelTarget target, bool add = true)
        {
            if (ReferenceEquals(null, Instances)) Instances = new HashSet<ChannelTarget>();

            if (add) Instances.Add(target);
            else Instances.Remove(target);
        }

        public static ChannelTarget GetSingleTargetFromChannel(uint channel)
        {
            return ChannelInstances != null && ChannelInstances.TryGetValue(channel, out var channelTargets)
                ? channelTargets?.FirstOrDefault(x => x != null)
                : null;
        }


        public void Start()
        {
            RegisterInstance(this);
            SetChannel(this, _channel);
        }

        private void OnDestroy()
        {
            RegisterInstance(this, false);
        }
    }
}