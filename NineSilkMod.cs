using BepInEx;
using HarmonyLib;

namespace NineSlik
{
    [BepInPlugin(Guid, "NineSilk", Version)]
    public class NineSilkMod : BaseUnityPlugin
    {
        public const string Guid = "Reits.NineSilk";
        public const string Version = "1.0.1.2";
        public const string Parry = "Parry";
        internal static NineSilkMod Ins = null!;
        public void Awake()
        {
            Ins = this;
            new Harmony(Guid).PatchAll();
            ModConfig.Ins = new ModConfig(Config);
        }
    }
}
