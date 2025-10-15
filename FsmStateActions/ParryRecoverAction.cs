using HutongGames.PlayMaker;

namespace NineSlik.FsmStateActions
{
    public class ParryRecoverAction : FsmStateAction
    {
        public override void OnEnter()
        {
            CounterAttackCheck.Ins.OnParryFailure();
            Finish();
        }
    }
}
