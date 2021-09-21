using System.Collections.Generic;
using HooahUtility.Model;
using HooahUtility.Model.Attribute;
using MessagePack;
using UnityEngine;
#if AI || HS2

#endif

#if AI || HS2
public class HijackNeck : CharacterGimmickBase, IFormData
#else
public class HijackNeck : MonoBehaviour
#endif
{
#if AI || HS2
    public static Dictionary<NeckLookControllerVer2, Vector3> _updatePositions =
        new Dictionary<NeckLookControllerVer2, Vector3>();

    private NeckLookControllerVer2 _lastNeckLookCtrl;

    [Key(10), PropertyRange(0.01f, 10f)]
    public float Rate
    {
        get => _rate;
        set
        {
            _rate = value;
            if (_lastNeckLookCtrl != null)
                _lastNeckLookCtrl.neckLookScript.neckTypeStates[1].leapSpeed = _rate;
        }
    }

    private float _rate;

    private void OnDestroy()
    {
        if (!IsReferenceValid()) return;
        Release();
    }

    private void Update()
    {
        if (!IsReferenceValid()) return;
        var p = transform.position;
        _updatePositions[TargetCharacter.neckLookCtrl] = p;
    }

    private void Release()
    {
        _updatePositions.Remove(_lastNeckLookCtrl);
        _lastNeckLookCtrl.neckLookScript.neckTypeStates[1].leapSpeed = 2;
    }


    protected override void OnChangeCharacterReference()
    {
        if (!IsReferenceValid()) return;
        if (_lastNeckLookCtrl != null && TargetCharacter.eyeLookCtrl == null)
            Release();

        _lastNeckLookCtrl = TargetCharacter.neckLookCtrl;
    }
#endif
}