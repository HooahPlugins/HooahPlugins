
using UnityEngine;
#if AI || HS2
using AIChara;
using HooahUtility.Serialization.StudioReference;
using MessagePack;
using Studio;
using UniRx;
#endif

public abstract class CharacterGimmickBase : MonoBehaviour
{
#if AI || HS2
    protected ChaControl TargetCharacter => (CharacterIndex?.ChaControl);
    protected OCIChar TargetOci => (CharacterIndex?.Reference as OCIChar);

    [Key(1)] public CharacterReference CharacterIndex = new CharacterReference();

    private void Start()
    {
        this.ObserveEveryValueChanged(x => x.TargetOci)
            .TakeUntilDestroy(this)
            .Subscribe(_ => OnChangeCharacterReference());
    }

    public virtual bool IsReferenceValid()
    {
        return TargetCharacter != null;
    }

    protected abstract void OnChangeCharacterReference();
#endif
}