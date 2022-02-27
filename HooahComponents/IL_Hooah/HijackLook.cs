using HooahUtility.Model;
using UnityEngine;
#if AI || HS2
using System.Collections.Generic;

#endif

#if AI || HS2
public class HijackLook : CharacterGimmickBase
#else
public class HijackLook : HooahBehavior
#endif
{
#if AI || HS2
    public static Dictionary<EyeLookController, Vector3>
        _updatePositions = new Dictionary<EyeLookController, Vector3>();

    private EyeLookController _lastEyeLookCtrl;

    private void Release()
    {
        if (_lastEyeLookCtrl != null && _updatePositions.ContainsKey(_lastEyeLookCtrl))
            _updatePositions.Remove(_lastEyeLookCtrl);
    }

    private void OnDestroy()
    {
        var eyeLookCtrl = TargetCharacter.eyeLookCtrl;
        if (IsReferenceValid() && _updatePositions.ContainsKey(eyeLookCtrl))
            _updatePositions.Remove(eyeLookCtrl);
    }

    private void Update()
    {
        if (!IsReferenceValid()) return;
        var p = transform.position;
        _updatePositions[TargetCharacter.eyeLookCtrl] = p;
    }


    protected override void OnChangeCharacterReference()
    {
        if (!IsReferenceValid()) return;
        Release();
        _lastEyeLookCtrl = TargetCharacter.eyeLookCtrl;
    }
#endif
}
