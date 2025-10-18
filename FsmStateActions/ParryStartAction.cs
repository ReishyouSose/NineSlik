using HutongGames.PlayMaker;

namespace NineSlik.FsmStateActions
{
    public class ParryStartAction : FsmStateAction
    {
        public override void OnEnter()
        {
            CounterAttackCheck.Ins.OnParryStart();
            Finish();
        }
    }
}
