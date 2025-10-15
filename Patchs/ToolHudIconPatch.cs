using HarmonyLib;
using HutongGames.PlayMaker.Actions;

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
                __result = false;
                return false;
            }
            return true;
        }
    }
}
