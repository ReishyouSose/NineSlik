using HarmonyLib;

namespace NineSlik.Patchs
{
    [HarmonyPatch(typeof(ToolHudIcon))]
    public static class ToolHudIconPatch
    {
        [HarmonyPatch(nameof(ToolHudIcon.GetIsEmpty))]
        [HarmonyPrefix]
        private static bool GetIsEmpty(ToolHudIcon __instance, ref bool __result)
        {
            if (__instance.CurrentTool.name == NineSilkMod.Parry)
            {
                if (!ModConfig.Ins.NineSolsMode.Value)
                {
                    __result = false;
                    return false;
                }
                __result = CounterAttackCheck.Cost > PlayerData.instance.silk;
                return false;
            }
            return true;
        }
    }
}
