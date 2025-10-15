using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using NineSlik.FsmStateActions;
using UnityEngine;

namespace NineSlik
{
    [BepInPlugin(Guid, "NineSilk", Version)]
    public class NineSilkMod : BaseUnityPlugin
    {
        public const string Guid = "Reits.NineSilk";
        public const string Version = "1.0.1.0";
        public const string Parry = "Parry";
        public ConfigEntry<int> ParrySilk = null!;
        public ConfigEntry<bool> ForceUnlockParry = null!;
        public ConfigEntry<bool> RefreshMoveAbility = null!;
        internal static NineSilkMod Ins = null!;
        private Vector3 oldPos;
        public void Awake()
        {
            Ins = this;
            CounterAttackCheck.Ins = new CounterAttackCheck();
            new Harmony(Guid).PatchAll();
            ParrySilk = Config.Bind("General", nameof(ParrySilk), 1,
                new ConfigDescription("Amount of silk granted when parring success\n格挡成功获得灵丝的数量",
                new AcceptableValueRange<int>(0, 3)));
            ForceUnlockParry = Config.Bind("General", nameof(ForceUnlockParry), true);
            RefreshMoveAbility = Config.Bind("General", nameof(RefreshMoveAbility), true,
                "Whether refresh Air Dash and Double Jump after parring success\n" +
                "是否在格挡成功后刷新空中冲刺和二段跳");
        }
        public void Update()
        {
            CounterAttackCheck.Ins.Update();
        }
    }
}
