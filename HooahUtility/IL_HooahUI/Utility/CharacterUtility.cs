using System;
using UnityEngine;
#if AI || HS2
using RootMotion.FinalIK;
using UniRx;
#endif

namespace Utility
{
    public static class CharacterUtility
    {
#if AI || HS2
        public static IObservable<Unit> ObservableIKPreSolve<T>(T attachInstance, IKSolverFullBody fullBody)
            where T : MonoBehaviour
        {
            return Observable.FromEvent<IKSolver.UpdateDelegate>(h => () => h(),
                    h => fullBody.OnPreSolve += h,
                    h => fullBody.OnPreSolve -= h)
                .TakeUntilDestroy(attachInstance);
        }
#endif
    }
}