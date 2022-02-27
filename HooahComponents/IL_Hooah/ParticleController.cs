using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using MyBox;
#elif AI || HS2
using HooahUtility.Model;
using HooahUtility.Model.Attribute;
using MessagePack;
using System.Collections;

#endif

public class ParticleController : HooahBehavior
{
    public enum ControlType { Loop, ForceResetLoop, PauseAndSeek, Manual, AwaitForExternalSignal }

    public uint seed;
    [Range(0, 10)] public float forceLoopTime = 0.1f;
    [Range(0, 5)] public float playbackTime = 1f;

    [FormerlySerializedAs("simulationSpeed")] [Range(0, 5)]
    public float playbackSpeed = 1f;

    public bool particleSpeedOverride = false;
    [Range(0, 5)] public float particleSpeedMultiplier = 1f;
    public bool gravityOverride = false;
    [Range(0, 5)] public float gravityMultiplier = 1f;

    public ParticleSystem[] loopingParticlesInSeekMode = { };
    public ParticleSystem[] particles = { };
    public ParticleSystemRenderer[] particlesRenderers = { };

    struct ParticleDataCache
    {
        public ParticleSystem.Particle[] instances;
        public ParticleSystemRenderer renderer;
        public List<Vector4> channel1;
        public List<Vector4> channel2;
        public float originalSpeedMultiplier;
        public float originalGravityMultiplier;
    }

    private Dictionary<ParticleSystem, ParticleDataCache> customDataCaches =
        new Dictionary<ParticleSystem, ParticleDataCache>() { };

    public ControlType controlType;
    private IDisposable _forceLoopObserver;
#if UNITY_EDITOR
#endif

    private void Awake()
    {
        for (var i = 0; i < particles.Length; i++)
        {
            var pSystem = particles[i];
            var main = pSystem.main;
            customDataCaches[pSystem] = new ParticleDataCache()
            {
                instances = new ParticleSystem.Particle[main.maxParticles],
                renderer = TryGetRenderer(i, out var particleSystemRenderer) ? particleSystemRenderer : null,
                channel1 = new List<Vector4>(),
                channel2 = new List<Vector4>(),
                originalGravityMultiplier = main.gravityModifierMultiplier,
                originalSpeedMultiplier = main.startSpeedMultiplier
            };
        }
#if UNITY_EDITOR
        this.ObserveEveryValueChanged(x => x.controlType, FrameCountType.FixedUpdate)
            .TakeUntilDestroy(this)
            .Subscribe(x => ObserveForceLoop());
        this.ObserveEveryValueChanged(x => x.forceLoopTime, FrameCountType.FixedUpdate)
            .TakeUntilDestroy(this)
            .Subscribe(x => ObserveForceLoop());
        this.ObserveEveryValueChanged(x => x.playbackTime, FrameCountType.FixedUpdate)
            .TakeUntilDestroy(this)
            .Subscribe(x => SetPlaybackParticleTime(playbackTime));
        this.ObserveEveryValueChanged(x => x.particleSpeedMultiplier, FrameCountType.FixedUpdate)
            .TakeUntilDestroy(this)
            .Subscribe(x => SetSimulationSpeed());
        this.ObserveEveryValueChanged(x => x.gravityMultiplier, FrameCountType.FixedUpdate)
            .TakeUntilDestroy(this)
            .Subscribe(x => AdjustGravity());
#else
#endif
        UpdateState();
    }

    private void SetControlType(ControlType cType)
    {
        foreach (var particleSystem in loopingParticlesInSeekMode)
        {
            var particleSystemMain = particleSystem.main;
            if (controlType == ControlType.PauseAndSeek) particleSystemMain.loop = true;
            else particleSystemMain.loop = false;
        }

        switch (cType)
        {
            case ControlType.ForceResetLoop:
                ObserveForceLoop();
                break;
            case ControlType.Loop:
                ObserveForceLoop();
                break;
            case ControlType.PauseAndSeek:
                Stop();
                Pause();
                UpdateDurationAndSpeed();
                SetPlaybackParticleTime(playbackTime);
                break;
            case ControlType.Manual:
            case ControlType.AwaitForExternalSignal:
                Stop(true);
                break;
        }
    }


    private void UpdatePlayPause()
    {
        if (controlType == ControlType.PauseAndSeek) Play();
        else Pause();
    }

    private void UpdateDurationAndSpeed()
    {
        foreach (var particle in particles)
        {
            particle.Clear();
            particle.Stop();
            var particleMain = particle.main;
            particleMain.duration = forceLoopTime * playbackSpeed;
            particleMain.simulationSpeed = playbackSpeed;
            particle.Play();
        }

        if (controlType == ControlType.PauseAndSeek) SetParticleTime(playbackTime);
    }

    private void ObserveForceLoop()
    {
        _forceLoopObserver?.Dispose();
        if (controlType == ControlType.PauseAndSeek) return;
        UpdateDurationAndSpeed();
        _forceLoopObserver = Observable.Interval(TimeSpan.FromSeconds(forceLoopTime))
            .TakeWhile(x => controlType == ControlType.ForceResetLoop || controlType == ControlType.Loop)
            .TakeUntilDisable(this)
            .Subscribe(x => Pew());
    }

    private void OnEnable() => UpdateState();

    private void OnDisable() => UpdateState();

    private void UpdateState()
    {
        UpdatePlayPause();
        SetParticleTime(0);
#if AI || HS2
        SetControlType(controlType);
#endif
    }

    private void SetPlaybackParticleTime(float time)
    {
        if (controlType != ControlType.PauseAndSeek) return;
        SetParticleTime(time);
    }

    private void SetParticleTime(float time)
    {
        for (var i = 0; i < particles.Length; i++)
        {
            var system = particles[i];
            system.Simulate(time, false, true);
            if (!GetParticleCache(system, out var cache)) continue;
            CalculateParticleData(ParticleSystemCustomData.Custom1, cache.channel1, in system, in cache);
            CalculateParticleData(ParticleSystemCustomData.Custom2, cache.channel2, in system, in cache);
        }
    }

    private void CalculateParticleData(ParticleSystemCustomData dataChannel, List<Vector4> channelData,
        in ParticleSystem pSystem, in ParticleDataCache cache)
    {
        if (channelData == null) return;
        pSystem.GetCustomParticleData(channelData, dataChannel);
        pSystem.GetParticles(cache.instances);

        for (var i = 0; i < channelData.Count; i++)
        {
            if (i >= cache.instances.Length) break;

            var length = pSystem.customData.GetVectorComponentCount(dataChannel);
            if (length < 1) continue;
            var vector4 = new Vector4();
            foreach (var cmpIndex in Enumerable.Range(0, length))
            {
                var p = cache.instances[i];
                var data = pSystem.customData.GetVector(dataChannel, cmpIndex);
                vector4[cmpIndex] =
                    data.Evaluate(
                        ((p.startLifetime - p.remainingLifetime) / p.startLifetime) * playbackSpeed
                    );
            }

            channelData[i] = vector4;
        }

        pSystem.SetCustomParticleData(channelData, dataChannel);
    }

    private bool GetParticleCache(ParticleSystem pSystem, out ParticleDataCache psInstances)
    {
        psInstances = default;

        if (pSystem == null) return false;
        if (!customDataCaches.ContainsKey(pSystem))
        {
            return false;
        }

        if (!customDataCaches.TryGetValue(pSystem, out var cache)) return false;
        if (cache.instances.Length != pSystem.main.maxParticles)
        {
            cache.instances = new ParticleSystem.Particle[pSystem.main.maxParticles];
            psInstances = cache;
            return true;
        }

        psInstances = cache;
        return true;
    }

    private void SetSimulationSpeed()
    {
        UpdateDurationAndSpeed();
    }

    private void Pause()
    {
        foreach (var particle in particles)
            particle.Pause();
    }

    private void Play()
    {
        foreach (var particle in particles)
            particle.Play();
    }

    private void Stop(bool clear = false)
    {
        foreach (var particle in particles)
        {
            particle.Stop();
            if (clear) particle.Clear();
        }
    }

    private void Emit()
    {
        foreach (var particle in particles)
            particle.Play();
    }

    public bool TryGetRenderer(int index, out ParticleSystemRenderer renderer)
    {
        renderer = default;
        if (particles == null || particlesRenderers == null || index >= particles.Length ||
            index >= particlesRenderers.Length) return false;

        renderer = particlesRenderers[index];
        return renderer != null;
    }

    public void AdjustGravity()
    {
        foreach (var particle in particles)
        {
            if (!customDataCaches.TryGetValue(particle, out var cache)) continue;
            var particleMain = particle.main;
            particleMain.gravityModifierMultiplier = gravityOverride
                ? cache.originalGravityMultiplier * gravityMultiplier
                : cache.originalGravityMultiplier;
        }
        if (controlType == ControlType.PauseAndSeek) SetParticleTime(playbackTime);
    }

    public void AdjustParticleSpeed()
    {
        foreach (var particle in particles)
        {
            if (!customDataCaches.TryGetValue(particle, out var cache)) continue;
            var particleMain = particle.main;
            particleMain.startSpeedMultiplier = particleSpeedOverride
                ? cache.originalSpeedMultiplier * particleSpeedMultiplier
                : cache.originalSpeedMultiplier;
        }
        if (controlType == ControlType.PauseAndSeek) SetParticleTime(playbackTime);
    }

#if AI || HS2
    [Key(0), PropertyRange(0, 10f)]
    public float PlaybackTime
    {
        get => playbackTime;
        set
        {
            playbackTime = value;
            SetPlaybackParticleTime(value);
        }
    }

    [Key(1), PropertyRange(0, 3f)]
    public float PlaybackSpeed
    {
        get => playbackSpeed;
        set
        {
            playbackSpeed = value;
            SetSimulationSpeed();
        }
    }

    [Key(2), PropertyRange(0, 3f)]
    public float ForceLoopInterval
    {
        get => forceLoopTime;
        set
        {
            forceLoopTime = value;
            ObserveForceLoop();
        }
    }

    [Key(13), PropertyRange(0, 10f)]
    public float GravityMultiplier
    {
        get => gravityMultiplier;
        set
        {
            gravityMultiplier = value;
            AdjustGravity();
        }
    }


    [Key(14), PropertyRange(0, 10f)]
    public float ParticleSpeedMultiplier
    {
        get => particleSpeedMultiplier;
        set
        {
            particleSpeedMultiplier = value;
            AdjustParticleSpeed();
        }
    }

    [Key(5)]
    public ControlType ParticleControlType
    {
        get => controlType;
        set
        {
            controlType = value;
            SetControlType(value);
        }
    }


    [Key(6)]
    public bool GravityOverride
    {
        get => gravityOverride;
        set
        {
            gravityOverride = value;
            AdjustGravity();
        }
    }


    [Key(7)]
    public bool ParticleSpeedOverride
    {
        get => particleSpeedOverride;
        set
        {
            particleSpeedOverride = value;
            AdjustParticleSpeed();
        }
    }

#endif

    private void Pew()
    {
        switch (controlType)
        {
            case ControlType.ForceResetLoop:
                SetParticleTime(0);
                Play();
                break;
            case ControlType.Loop:
            case ControlType.Manual:
            case ControlType.PauseAndSeek:
                Emit();
                break;
        }
    }

#if UNITY_EDITOR
    [ButtonMethod]
#elif AI || HS2
    [RuntimeFunction("Pew")]
#endif
    public void TriggerParticleEvent()
    {
        if (controlType == ControlType.PauseAndSeek || controlType == ControlType.Manual) Pew();
    }

#if UNITY_EDITOR
    private float oldValue = 0f;

    [ButtonMethod]
    public void RegisterParticles()
    {
        particles = GetComponentsInChildren<ParticleSystem>().ToArray();
        particlesRenderers = particles.Select(x => x.GetComponent<ParticleSystemRenderer>()).ToArray();
    }
#endif
}
