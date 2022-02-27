using HooahUtility.Model;
using UnityEngine;
#if AI || HS2
using AIChara;
using HooahUtility.Serialization.StudioReference;
using MessagePack;
using Studio;
using UniRx;
#endif

/*
 * This class's members are only available in plugin environments.
 */
public abstract class CharacterGimmickBase : HooahBehavior
{
#if AI || HS2
    protected ChaControl TargetCharacter => (CharacterIndex?.ChaControl);
    protected OCIChar TargetOci => (CharacterIndex?.Reference as OCIChar);

    [Key(1)] public CharacterReference CharacterIndex;

    private void Start()
    {
        CharacterIndex = new CharacterReference();
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
