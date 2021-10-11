#if AI || HS2
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

            // well, until i make some sort of character specific data container
            // eye adjustment will remain debug feature...
            // soon. mate. soon.
#if DEBUG
            foreach (var eyeTypeState in ChaControl.eyeLookCtrl.eyeLookScript.eyeTypeStates)
            {
                eyeTypeState.bendingMultiplier = 1f;
                eyeTypeState.upBendingAngle = -15f;
                eyeTypeState.downBendingAngle = 15f;
                eyeTypeState.minBendingAngle = -40;
                eyeTypeState.maxBendingAngle = 40;
                eyeTypeState.leapSpeed = 20f;
                eyeTypeState.nearDis = 8f;
                eyeTypeState.forntTagDis = 10f;
            }
#endif
        }

        protected override void OnCardBeingSaved(GameMode currentGameMode)
        {
            // does nothing
        }
    }
}
#endif