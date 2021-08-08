#if AI || HS2
using System;
using System.Linq;
using AIChara;
using HooahUtility.Model.Attribute;
using HooahUtility.Utility;
using MessagePack;
using Studio;
#endif
using UnityEngine;

public abstract class CharacterGimmickBase : MonoBehaviour
{
#if AI || HS2
    private int _characterIndex;
    protected ChaControl targetCharacter;
    protected OCIChar targetOci;

    [Key(1), NumberSpinner]
    public int CharacterIndex
    {
        get => _characterIndex;
        set
        {
            var characters = Singleton<Studio.Studio>.Instance.dicObjectCtrl
                .Select(x => x.Value)
                .OfType<OCIChar>()
                .ToArray();

            try
            {
                if (characters.Length <= 0)
                {
                    _characterIndex = -1;
                }
                else
                {
                    _characterIndex = value % characters.Length;
                    targetOci = characters[_characterIndex];
                    targetCharacter = targetOci.charInfo;
                    OnChangeCharacterReference();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                _characterIndex = -2;
            }
        }
    }

    protected abstract void OnChangeCharacterReference();
#endif
}