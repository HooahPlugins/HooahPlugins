using System;
using System.Collections;
using HooahComponents;
using HooahUtility.Model;
using UniRx;
using UnityEngine;
#if AI || HS2
using System.Collections.Generic;
using AIChara;

#endif

#if AI || HS2
public class HijackLook : CharacterGimmickBase, IFormData
#else
public class HijackLook : MonoBehaviour
#endif
{
#if AI || HS2
    public static Dictionary<EyeLookController, Vector3>
        _updatePositions = new Dictionary<EyeLookController, Vector3>();

    private EyeLookController _lastEyeLookCtrl;

    private void Awake()
    {
    }

    private void OnDestroy()
    {
        if (targetCharacter != null)
            _updatePositions.Remove(targetCharacter.eyeLookCtrl);
    }

    private void Update()
    {
        if (targetCharacter == null) return;
        var p = transform.position;
        _updatePositions[targetCharacter.eyeLookCtrl] = p;
    }

    
    protected override void OnChangeCharacterReference()
    {
        if (_lastEyeLookCtrl != null && targetCharacter.eyeLookCtrl == null)
            _updatePositions.Remove(_lastEyeLookCtrl);
        
        _lastEyeLookCtrl = targetCharacter.eyeLookCtrl;
    }
#endif
}