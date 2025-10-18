using BepInEx.Configuration;

namespace NineSlik
{
    public class ModConfig
    {
        internal static ModConfig Ins = null!;
        public ConfigEntry<int> ParrySilk;
        public ConfigEntry<int> ParryCost;
        public ConfigEntry<bool> ForceUnlockParry;
        public ConfigEntry<bool> RefreshMoveAbility;
        public ConfigEntry<bool> NineSolsMode;
        public ModConfig(ConfigFile file)
        {
            ParrySilk = file.Bind("General", nameof(ParrySilk), 1,
                new ConfigDescription("Amount of silk granted when parring success\n格挡成功获得灵丝的数量",
                new AcceptableValueRange<int>(0, 3)));
            ParryCost = file.Bind("General", nameof(ParryCost), 0,
                new ConfigDescription("Silk cost for Parry\n执行格挡消耗的灵丝",
                new AcceptableValueRange<int>(0, 4)));
            /*ParryDirection = file.Bind("General", nameof(ParryDirection), false,
                "Whether parry need to be in the correct direction\n格挡是否需要朝向正确的方向");*/
            ForceUnlockParry = file.Bind("General", nameof(ForceUnlockParry), true);
            RefreshMoveAbility = file.Bind("General", nameof(RefreshMoveAbility), true,
                "Whether refresh Air Dash and Double Jump after parring success\n" +
                "是否在格挡成功后刷新空中冲刺和二段跳");
            NineSolsMode = file.Bind("General", nameof (NineSolsMode), true,
                "Use parry mode from Nine Sols\n使用九日的格挡模式");
        }
    }
}
