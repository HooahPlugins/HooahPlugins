using HooahUtility.Model;
using UnityEngine;
#if AI || HS2
using System.Collections.Generic;

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

    private void OnDestroy()
    {
        if (!IsReferenceValid())
            _updatePositions.Remove(TargetCharacter.eyeLookCtrl);
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
        if (_lastEyeLookCtrl != null && TargetCharacter.eyeLookCtrl == null)
            _updatePositions.Remove(_lastEyeLookCtrl);

        _lastEyeLookCtrl = TargetCharacter.eyeLookCtrl;
    }
#endif
}