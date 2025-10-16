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
                __result = CounterAttackCheck.Cost > PlayerData.instance.silk;
                return false;
            }
            return true;
        }
    }
}
