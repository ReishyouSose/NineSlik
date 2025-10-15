using HarmonyLib;
using NineSlik.FsmStateActions;
using System;
using System.Linq;

namespace NineSlik.Patchs
{
    [HarmonyPatch(typeof(HeroController))]
    public static class HeroControllerPatch
    {
        [HarmonyPatch(nameof(HeroController.CanThrowTool),
            new Type[] { typeof(ToolItem), typeof(AttackToolBinding), typeof(bool) })]
        [HarmonyPrefix]
        private static bool CanThrowTool(ToolItem tool, ref bool __result)
        {
            if (tool.name == NineSilkMod.Parry)
            {
                __result = true;
                return false;
            }
            return true;
        }
        [HarmonyPatch(nameof(HeroController.Awake))]
        [HarmonyPostfix]
        private static void Awake(HeroController __instance)
        {
            if (NineSilkMod.Ins.ForceUnlockParry.Value)
                PlayerData.instance.hasParry = true;
            //PlayMakerFSM fsm = __instance.gameObject.LocateMyFSM("Silk Specials");
            PlayMakerFSM fsm = __instance.silkSpecialFSM;
            string state = "Parry Start";
            var fsmState = fsm.FsmStates.First(x => x.Name == state);
            var list = fsmState.Actions.ToList();
            var getSilkCost = list[1];
            var takeSilk = list[2];
            list.RemoveAt(2);
            list.RemoveAt(1);
            list.Insert(0, new ParryStartAction());
            fsmState.Actions = list.ToArray();

            fsmState = fsm.FsmStates.First(x => x.Name == "Parry Clash");
            list = fsmState.Actions.ToList();
            list.Add(new ShouldParrySlashAction());
            fsmState.Actions = list.ToArray();

            fsmState = fsm.FsmStates.First(x => x.Name == "Parry Recover");
            list = fsmState.Actions.ToList();
            list.Add(new ParryRecoverAction());
            fsmState.Actions = list.ToArray();
        }
    }
}
