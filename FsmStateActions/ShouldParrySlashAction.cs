using HutongGames.PlayMaker;

namespace NineSlik.FsmStateActions
{
    public class ShouldParrySlashAction : FsmStateAction
    {
        public override void OnEnter()
        {
            if (!CounterAttackCheck.Ins.OnParrySuccess())
                Fsm.SetState("Parry Catch");
            Finish();
        }
    }
}
