#if AI || HS2
using AIChara;
using KKAPI;
using KKAPI.Chara;

namespace HooahComponents.Hooks
{
    public class HooahCharacterController : CharaCustomFunctionController
    {
        protected override void Start()
        {
            // Initialize things please.
            foreach (var dickController in DickController.Instances) dickController.CollideWithCharacter(ChaControl);
        }

        protected override void OnCardBeingSaved(GameMode currentGameMode)
        {
            // does nothing
        }
    }
}
#endif