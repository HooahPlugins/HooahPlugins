using System;
using System.Collections.Generic;
using System.Linq;
using HooahComponents;
using HooahUtility.Model.Attribute;
using MessagePack;
using UniRx;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Serialization;
using Random = System.Random;
#if AI || HS2
using AIChara;
using Studio;
using HooahUtility.Model;
using KKAPI;

#elif UNITY_EDITOR
using MyBox;
#endif

// ReSharper disable MergeConditionalExpression


namespace HooahComponents
{
    [Serializable]
    public class VertexShapeGraph
    {
        public string key;
        public AnimationCurve curve;
        public int index;
    }
}

#if AI || HS2
/// <list type="bullet|number|table">
///     <listheader><index>Serialization Index</index><description>Data Description</description></listheader>
///     <item><index>0</index><description>Dock Distance (0.1f-5.0f:2.1f)</description></item>
///     <item><index>1</index><description>Pull Length (0.1f-5.0f:1.0f)</description></item>
///     <item><index>2</index><description>Use Nearest Navigator (false)</description></item>
///     <item><index>3</index><description>Use Nearest Proxy (false)</description></item>
///     <item><index>4</index><description>Scale (0.001f-3.0f:1.0f)</description></item>
///     <item><index>5</index><description>Use Censor Effect (false)</description></item>
///     <item><index>50</index><description>Collider Diameter (0.01f-2.0f:1.0f)</description></item>
///     <item><index>51</index><description>Collider Height (0.01f-2.0f:2.0f)</description></item>
/// </list>
/// <summary>
///     Core controller MonoBehavior for the item.
/// 
/// </summary>
public class DickController : MonoBehaviour, ISerializationCallbackReceiver, IFormData
#else
public class DickController : MonoBehaviour, ISerializationCallbackReceiver
#endif
{
    #region Static Variables

    private static readonly Quaternion RotationA = Quaternion.Euler(90, 0, 0) * Quaternion.Euler(0, 90, 0);

    public static HashSet<DickController> Instances = new HashSet<DickController>();

    #endregion

    #region Public Variables

    #region Object References

    [Header("Audio References")] public AudioClip[] pewSounds;
    public AudioSource audioPlayer;
    [Header("GameObject References")] public GameObject curveEnd;
    public GameObject curveMiddle;
    public GameObject curveStart;
    public GameObject[] dickChains;

    // TODO: make dickmesh more modular..
    public SkinnedMeshRenderer dickMesh;
    public GameObject censoringObject;
    private SkinnedMeshRenderer _censoringMesh;
    public Transform pullProxy;
    public Transform pullProxyRoot;

    #endregion

    #region Behavior Adjustment

    [Header("Navigation Control"), Key(0), Range(0.1f, 5)]
    public float dockDistance = 2.1f;

    [FormerlySerializedAs("steepness"), Key(1), Range(0.1f, 5f)]
    public float pullLength = 1f;

    [FormerlySerializedAs("followClosestNavigator"), Key(2)]
    public bool useNearestNavigator;

    [Key(3)] public bool useNearestProxy;

    // Fuck you unity
    [FormerlySerializedAs("_dataKey")] [Header("Shape Animation")]
    public string[] dataKey;

    [FormerlySerializedAs("_dataCurve")] public AnimationCurve[] dataCurve;
    [FormerlySerializedAs("_dataIndex")] public int[] dataIndex;

    [FormerlySerializedAs("_benisScale"), Key(4), Range(0.001f, 3f)]
    public float benisScale;


    [Key(5)]
    public bool UseCensorEffect
    {
        set
        {
            _useCensor = value;
            censoringObject.SetActive(_useCensor);
        }
        get => _useCensor;
    }

    #endregion

    #region Material Adjustment

    #region Material Adjustment Parameter

    // Todo: integrate with other material adjustment plugins 
    [FieldName("Rubber Opacity"), PropertyRange(0f, 1f), Key(100)]
    public float MaterialRubberOpacity
    {
        get => _materialRubberOpacity;
        set
        {
            _materialRubberOpacity = value;
            UpdateMaterialFloat(MaterialRubberOpacityKey, value);
        }
    }

    [FieldName("Border Strength"), PropertyRange(0.01f, 100f), Key(101)]
    public float MaterialBorderStrength
    {
        get => _materialBorderStrength;
        set
        {
            _materialBorderStrength = value;
            UpdateMaterialFloat(MaterialBorderStrengthKey, value);
        }
    }

    [FieldName("Border Progress"), PropertyRange(0f, 1f), Key(102)]
    public float MaterialProgress
    {
        get => _materialProgress;
        set
        {
            _materialProgress = value;
            UpdateMaterialFloat(MaterialProgressKey, value);
        }
    }

    [FieldName("Residue Scale Y"), PropertyRange(0.01f, 32f), Key(103)]
    public float MaterialResidueScaleY
    {
        get => _materialResidueScaleY;
        set
        {
            _materialResidueScaleY = value;
            UpdateMaterialFloat(MaterialResidueScaleYKey, value);
        }
    }

    [FieldName("Residue Scale X"), PropertyRange(0.01f, 32f), Key(104)]
    public float MaterialResidueScaleX
    {
        get => _materialResidueScaleX;
        set
        {
            _materialResidueScaleX = value;
            UpdateMaterialFloat(MaterialResidueScaleXKey, value);
        }
    }

    [FieldName("Residue Scale Noise"), PropertyRange(0.01f, 256f), Key(104)]
    public float MaterialResidueScaleZ
    {
        get => _materialResidueScaleZ;
        set
        {
            _materialResidueScaleZ = value;
            UpdateMaterialFloat(MaterialResidueScaleZKey, value);
        }
    }

    [FieldName("Residue Opacity"), PropertyRange(0, 1f), Key(105)]
    public float MaterialResidueOpacity
    {
        get => _materialResidueOpacity;
        set
        {
            _materialResidueOpacity = value;
            UpdateMaterialFloat(MaterialResidueOpacityKey, value);
        }
    }

    #endregion

    #region Shader Keywords

    private const string MaterialRubberOpacityKey = "_RubberOpacity";
    private const string MaterialBorderStrengthKey = "_BorderStrength";
    private const string MaterialProgressKey = "_Progress";
    private const string MaterialResidueOpacityKey = "_ResidueIntensity";
    private const string MaterialResidueScaleXKey = "_ResidueScaleX";
    private const string MaterialResidueScaleYKey = "_ResidueScaleY";
    private const string MaterialResidueScaleZKey = "_ResidueScaleZ";

    #endregion

    #region Private Variables

    private float _materialRubberOpacity = 0f;
    private float _materialBorderStrength = 1f;
    private float _materialProgress;
    private float _materialResidueOpacity = 0f;
    private float _materialResidueScaleX = 15f;
    private float _materialResidueScaleY = 15f;
    private float _materialResidueScaleZ = 125f;

    #endregion

    private void UpdateMaterialFloat(string key, float value)
    {
        if (dickMesh) dickMesh.material.SetFloat(key, value);
    }

    #endregion

    #endregion

    #region Private Variables

#pragma warning disable 169
    private bool _useCensor;
    private readonly Random _random = new Random();
    private Animator _animator;
    private bool _canMorph;
    private DickNavigator _dickNavigator;
    private Transform[] _dickTransforms;
    private ApproachTarget[] _dickTransformTarget;
    private NativeArray<ApproachTarget> _dickTransformTargetNative;
    private Vector3 _endPos;
    private NativeArray<float> _factor;
    private Vector3 _midPos;
    private Vector3 _startPos;
    private JobHandle _moveHandle;
    private JobHandle _pullHandle;
    private Transform _pullTransform;
    private float _renderPullFactor;
    private VertexShapeGraph[] _shapeGraphs;
    private TransformAccessArray _transformAccessArray;
    private bool _disposed;
    private ApplyTransformJob _applyTransformJob;
    private DickPositionCalcuationJob _jobPosCalc;
    private DickPullCalculationJob _jobCalcPull;
    private JobHandle _applyTransformJobHandle;
    private float _leftLength;
    private float _rightLength;
    private bool _lazy;
#pragma warning restore 169

    #endregion

    #region In-game Parameters

#if AI || HS2
    private DynamicBoneColliderBase[] _colliderBases;

    private DynamicBoneColliderBase[] Colliders
    {
        get
        {
            _colliderBases = dickChains
                .Select(x => x.GetComponent<DynamicBoneColliderBase>())
                .Where(x => x != null)
                .ToArray();
            return _colliderBases;
        }
    }

    private float _colliderDiameter = 0.26f;

    [Key(50), PropertyRange(0.01f, 2f)]
    public float ColliderDiameter
    {
        get => _colliderDiameter;
        set
        {
            _colliderDiameter = value;
            if (_colliderHeight <= _colliderDiameter * 2) _colliderHeight = _colliderDiameter * 2.05f;
            foreach (var c in Colliders)
                if (c is DynamicBoneCollider collider)
                    collider.m_Radius = _colliderDiameter;
        }
    }

    private float _colliderHeight = 0.65f;

    [Key(51), PropertyRange(0.01f, 2f)]
    public float ColliderHeight
    {
        get => _colliderHeight;
        set
        {
            _colliderHeight = value;
            if (_colliderHeight <= _colliderDiameter * 2) _colliderHeight = _colliderDiameter * 2.05f;
            foreach (var c in Colliders)
                if (c is DynamicBoneCollider collider)
                    collider.m_Height = _colliderHeight;
        }
    }

    [RuntimeFunction("Collide with Selected characters")]
    public void ColliderWithSelectedCharacters()
    {
        foreach (var o in Studio.Studio.Instance
            .treeNodeCtrl.selectObjectCtrl
            .OfType<OCIChar>()
            .Select(x => x.charInfo.gameObject)) RegisterCollidersWithGameObject(o);
    }

    public void RegisterCollidersWithGameObject(GameObject o)
    {
        foreach (var bones in o.GetComponentsInChildren<DynamicBone>())
            bones.m_Colliders.AddRange(Colliders);
    }

    public void CollideWithCharacter(ChaControl chaControl)
    {
        RegisterCollidersWithGameObject(chaControl.gameObject);
    }
#endif

    #endregion

    #region In-Game Logics

    private void Start()
    {
        Instances.Add(this);

#if AI || HS2
        // Forgive me, father. I've sinned.
        // But for now, I think no one will spawn more than 100 dicks in a scene.
        if (KoikatuAPI.GetCurrentGameMode() == GameMode.Studio)
        {
            foreach (var ociChar in Studio.Studio.Instance.dicObjectCtrl.Values.OfType<OCIChar>())
                CollideWithCharacter(ociChar.charInfo);
        }
#endif

        // Reference animator for sound pitch control
        _animator = GetComponent<Animator>();

        // Initialize Unity Jobs
        _applyTransformJob = new ApplyTransformJob();
        _jobPosCalc = new DickPositionCalcuationJob();
        _jobCalcPull = new DickPullCalculationJob();

        if (censoringObject != null) _censoringMesh = censoringObject.GetComponent<SkinnedMeshRenderer>();

        MaterialRubberOpacity = 0f;
        MaterialProgress = 0.5f;
        MaterialBorderStrength = 0.4f;
        MaterialResidueOpacity = 0f;
        MaterialResidueScaleX = 15f;
        MaterialResidueScaleY = 15f;
        MaterialResidueScaleZ = 125f;

        // Register shape keys to make pull proxy works
        if (_shapeGraphs == null) return;
        foreach (var i in Enumerable.Range(0, dickMesh.sharedMesh.blendShapeCount))
        {
            var blendShapeName = dickMesh.sharedMesh.GetBlendShapeName(i);
            if (blendShapeName == null || blendShapeName.Length <= 0) continue;
            var data = _shapeGraphs.FirstOrDefault(x => x.key == blendShapeName);
            if (data == null) continue;
            data.index = i;
        }
    }

    private void CalculateUpdate()
    {
        if (dickChains == null || dickChains.Length <= 0) return;
        _moveHandle = NavigationCalculation();

        var pullProxyTransform = _pullTransform ? _pullTransform : pullProxy;
        if (!ReferenceEquals(pullProxyTransform, null)) _pullHandle = PullFactorCalculation(pullProxyTransform);
    }

    private bool CanUpdate() => !(_lazy || _disposed || _dickNavigator == null || !isActiveAndEnabled);

    private void ProcessJobs()
    {
        if (!CanUpdate()) return;
        UpdateNavigatorPosition();
        CalculateUpdate();
        CompleteJobs();

        if (_shapeGraphs == null) return;
        foreach (var vertexShapeGraph in _shapeGraphs)
        {
            var value = vertexShapeGraph.curve.Evaluate(_renderPullFactor) * 100;
            try
            {
                dickMesh.SetBlendShapeWeight(vertexShapeGraph.index, value);
                // assuming censoring object is using same mesh as dick mesh.
                if (_censoringMesh != null)
                    _censoringMesh.SetBlendShapeWeight(vertexShapeGraph.index, value);
            }
            catch (Exception e)
            {
                // do nothing
            }
        }
    }

    private void CompleteJobs()
    {
        _moveHandle.Complete();
        _pullHandle.Complete();
        _applyTransformJobHandle.Complete();
        if (_pullHandle.IsCompleted) _renderPullFactor = _factor[0];
        _applyTransformJob.Targets = _dickTransformTargetNative;
        _applyTransformJobHandle = _applyTransformJob.Schedule(_transformAccessArray, _moveHandle);
    }

    private void OnEnable()
    {
        _startPos = curveStart.transform.position;
        _endPos = curveEnd.transform.position;
        _midPos = curveMiddle.transform.position;

        var chainTransforms = dickChains.Select(o => o.transform).ToArray();
        _transformAccessArray = new TransformAccessArray(chainTransforms, chainTransforms.Length);
        _dickTransformTargetNative = new NativeArray<ApproachTarget>(dickChains.Length, Allocator.Persistent);
        _factor = new NativeArray<float>(1, Allocator.Persistent) { [0] = 0f };

        Observable.IntervalFrame(10)
            .TakeUntilDisable(this)
            .Subscribe(_ =>
            {
                AssignNearestDickNavigator();
                AssignNearestPullProxy();
            });

        Observable.EveryEndOfFrame()
            .TakeUntilDisable(this)
            .Subscribe(_ => { ProcessJobs(); });

        _disposed = false;
    }

    private void OnBecameInvisible()
    {
        _lazy = true;
    }

    private void OnBecameVisible()
    {
        _lazy = false;
    }


    private void DisposeNativeJobAndArray()
    {
        if (_disposed) return;
        _moveHandle.Complete();
        _pullHandle.Complete();
        _dickTransformTargetNative.Dispose();
        _transformAccessArray.Dispose();
        _factor.Dispose();
        _disposed = true;
    }

    private void OnDisable() => DisposeNativeJobAndArray();

    private void OnDestroy()
    {
        Instances.Remove(this);
        DisposeNativeJobAndArray();
    }

    #endregion

    #region Unity Editor Logics

#if UNITY_EDITOR
    private void EditorDrawDickChain()
    {
        if (dickChains == null) return;
        foreach (var dickChain in dickChains)
        {
            var t = dickChain.transform;
            var p = t.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(p, .1f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(p, p + t.forward * 1);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(p, p + t.up * 1);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(p, p + t.right * 1);
        }
    }

    private void EditorDrawProxy()
    {
        if (pullProxyRoot == null) return;
        var t = pullProxyRoot;
        var p = t.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(p, .1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(p, p + t.forward * 1);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(p, p + t.up * 1);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(p, p + t.right * 1);
    }

    private void OnDrawGizmos()
    {
        EditorDrawDickChain();
        EditorDrawProxy();
    }
#endif

    #endregion

    #region Serialization

    public void OnBeforeSerialize()
    {
        if (_shapeGraphs == null || _shapeGraphs.Length == 0) return;
        for (var i = _shapeGraphs.Length - 1; i >= 0; i--)
        {
            var structData = _shapeGraphs[i];
            if (structData == null) continue;
            dataIndex[i] = structData.index;
        }
    }

    public void OnAfterDeserialize()
    {
        if (dataCurve == null || dataKey == null || dataIndex == null ||
            dataCurve.Length == 0 || dataKey.Length == 0 || dataIndex.Length == 0 ||
            dataCurve.Length != dataKey.Length || dataCurve.Length != dataIndex.Length ||
            dataKey.Length != dataIndex.Length
        ) return;

        _shapeGraphs = new VertexShapeGraph[dataCurve.Length];
        for (var i = _shapeGraphs.Length - 1; i >= 0; i--)
        {
            if (dataIndex[i] < 0 || dataKey[i] == null || dataCurve[i] == null) continue;
            _shapeGraphs[i] = new VertexShapeGraph { key = dataKey[i], curve = dataCurve[i], index = dataIndex[i] };
        }
    }

    #endregion

    #region Parallel Jobs

    private JobHandle PullFactorCalculation(Transform pullProxyTransform)
    {
        _jobCalcPull.Root = pullProxy.position;
        _jobCalcPull.Target = pullProxyTransform.position;
        _jobCalcPull.Factor = _factor;
        _jobCalcPull.PullLength = pullLength;
        _jobCalcPull.BenisScale = benisScale;
        _jobCalcPull.Up = pullProxyRoot.up;
        return _jobCalcPull.Schedule();
    }

    private JobHandle NavigationCalculation()
    {
        var segmentScale = benisScale / _dickTransformTargetNative.Length;

        _jobPosCalc.DickTransformTarget = _dickTransformTargetNative;
        _jobPosCalc.Start = _startPos;
        _jobPosCalc.End = _endPos;
        _jobPosCalc.Middle = _midPos;
        _jobPosCalc.Right = transform.right;
        _jobPosCalc.BenisScale = benisScale;
        _jobPosCalc.ChainsLength = _dickTransformTargetNative.Length;

        _leftLength = Vector3.Distance(_jobPosCalc.Start, _jobPosCalc.Middle);
        _rightLength = Vector3.Distance(_jobPosCalc.Middle, _jobPosCalc.End);
        _jobPosCalc.LeftLength = _leftLength;
        _jobPosCalc.RightLength = _rightLength;

        // var total = Mathf.Max(0, rightLength - (benisScale / dickChains.Length)*transform.localScale.z);
        var threshold = (segmentScale * (_dickTransformTargetNative.Length - 1));
        var insertFactor = Math.Min(1, Math.Max(0, threshold - Mathf.Max(0, _leftLength)));
        var fullFactor = Math.Min(1, Math.Max(0, insertFactor / Mathf.Max(0.00001f, _rightLength)));
        _dickNavigator.factor = fullFactor;

        return _jobPosCalc.Schedule(_dickTransformTargetNative.Length, 1);
    }


    private void AssignNearestDickNavigator()
    {
        if (!useNearestNavigator || DickNavigator.Instances.Count <= 0) return;
        _dickNavigator = DickNavigator.Instances
            .OrderBy(x => (x.dickMidPoint.position - _startPos).sqrMagnitude)
            .FirstOrDefault();
    }

    private void AssignNearestPullProxy()
    {
        if (!useNearestProxy) return;
        if (DickPuller.Instances.Count > 0)
            _pullTransform = DickPuller.Instances
                .OrderBy(x => (x.transform.position - pullProxyRoot.transform.position).sqrMagnitude)
                .First().gameObject.transform;
        else
            _pullTransform = pullProxy;
    }

    private void ReleaseDickNavigator()
    {
        if (_dickNavigator == null) return;
        _dickNavigator.factor = 0f;
        _dickNavigator = null;
    }

    private void UpdateNavigatorPosition()
    {
        try
        {
            // the parenting transform should be good.
            _startPos = curveStart.transform.position;
            if (ReferenceEquals(null, _dickNavigator)) return;
            _midPos = _dickNavigator.dickMidPoint.position;
            _endPos = _dickNavigator.dickEndPoint.position;
        }
        catch (MissingReferenceException)
        {
            ReleaseDickNavigator();
        }
        catch (NullReferenceException)
        {
            ReleaseDickNavigator();
        }
    }

    public void PlayPew()
    {
#if DEBUG
        // still finding some nice sounds to work with
        if (!pewSounds.Any() || audioPlayer == null) return;
        audioPlayer.Stop();
        var randomIndex = _random.Next(0, pewSounds.Length - 1);
        audioPlayer.pitch = (_animator != null ? _animator.speed : 1f) + Convert.ToSingle(_random.NextDouble() * 0.4);
        audioPlayer.PlayOneShot(pewSounds[randomIndex]);
        EventConsumer.EmitEvent(EventConsumer.EventType.Nomi);
#endif
    }

    public struct ApproachTarget
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }

    private struct ApplyTransformJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<ApproachTarget> Targets;

        public void Execute(int i, TransformAccess transform)
        {
            transform.position = Targets[i].Position;
            if (i == Targets.Length - 1) return;
            transform.rotation = Targets[i].Rotation;
        }
    }

    private struct DickPositionCalcuationJob : IJobParallelFor
    {
        public NativeArray<ApproachTarget> DickTransformTarget;
        [ReadOnly] public Vector3 Start;
        [ReadOnly] public Vector3 End;
        [ReadOnly] public Vector3 Middle;
        [ReadOnly] public Vector3 Right;
        [ReadOnly] public float ChainsLength;
        [ReadOnly] public float BenisScale;
        [ReadOnly] public float LeftLength;
        [ReadOnly] public float RightLength;


        private static Vector3 Linear(Vector3 p0, Vector3 p1, Vector3 p2, float left, float right, float length,
            float factor)
        {
            var abs = factor * length;
            return abs < left ? Vector3.Lerp(p0, p1, abs / left) : Vector3.Lerp(p1, p2, (abs - left) / right);
        }

        public void Execute(int index)
        {
            if (ChainsLength == 0) return; // no divide by zero
            var approachTarget = DickTransformTarget[index];
            var leftDistance = LeftLength;
            var rightDistance = RightLength;
            var posA = Linear(Start, Middle, End, leftDistance, rightDistance, BenisScale, index / ChainsLength);
            var posB = Linear(Start, Middle, End, leftDistance, rightDistance, BenisScale, (index + 1f) / ChainsLength);
            var dir = (posB - posA).normalized;

            approachTarget.Position = posA;
            if (dir != Vector3.zero)
            {
                var q = Quaternion.LookRotation(dir, this.Right);
                q *= RotationA;
                approachTarget.Rotation = q;
            }

            DickTransformTarget[index] = approachTarget;
        }
    }

    private struct DickPullCalculationJob : IJob
    {
        [ReadOnly] public Vector3 Root;
        [ReadOnly] public Vector3 Target;
        public NativeArray<float> Factor;
        [ReadOnly] public float PullLength;
        [ReadOnly] public float BenisScale;
        [ReadOnly] public Vector3 Up;

        public void Execute()
        {
            Factor[0] = (Vector3.Distance(Root, Target) * Vector3.Dot((Root - Target).normalized, Up)
                / PullLength * BenisScale + 1) / 2;
        }
    }

    #endregion
}