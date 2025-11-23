using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace NineSlik.FsmStateActions
{
    public class ParryStanceAction : Wait
    {
        public override void OnEnter()
        {
            time = CounterAttackCheck.Ins.AllowCounter ? 1f : 0.4f;
            base.OnEnter();
            Debug.Log("are you ok?");
        }
    }
}
