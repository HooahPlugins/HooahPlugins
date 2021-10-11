using JetBrains.Annotations;
using MessagePack;
using UnityEngine;
#if HS2 || AI
using Studio;
#endif

namespace HooahUtility.Serialization.StudioReference
{
    public interface IStudioReference : IMessagePackSerializationCallbackReceiver
    {
#if HS2 || AI
        [CanBeNull, IgnoreMember] Transform ReferenceTransform { get; }

        [CanBeNull, IgnoreMember] ObjectCtrlInfo Reference { get; }
#endif
    }
}