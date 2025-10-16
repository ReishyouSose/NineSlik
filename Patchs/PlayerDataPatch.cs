using HarmonyLib;

namespace NineSlik.Patchs
{
    [HarmonyPatch(typeof(PlayerData))]
    public static class PlayerDataPatch
    {
        [HarmonyPatch(nameof(PlayerData.TakeHealth))]
        [HarmonyPostfix]
        private static void TakeHealth()
        {
            CounterAttackCheck.Ins.OnHurt();
        }
    }
}
