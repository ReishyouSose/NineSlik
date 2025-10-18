using GlobalSettings;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using NineSlik.FsmStateActions;
using System;
using System.Linq;
using UnityEngine;

namespace NineSlik
{
    public class CounterAttackCheck : MonoBehaviour
    {
        public static CounterAttackCheck Ins = null!;
        public enum ChargeState
        {
            Idle,
            Charging,
            Ready
        }

        public ChargeState State;
        public float ChargeTimer;
        public float ReadyTimer;
        public float ComboTimer;

        public GameObject Effect = null!;
        public tk2dSpriteAnimator EffectAnim = null!;
        public FsmState Cross = null!;
        public Wait Wait = null!;
        public DecelerateV2 Decelerate = null!;

        public bool AllowCounter => ReadyTimer > 0 || ComboTimer > 0;
        public static int Cost => Math.Max(0, ModConfig.Ins.ParryCost.Value - (Gameplay.FleaCharmTool.IsEquippedHud ? 1 : 0));
        public void Awake()
        {
            var hc = HeroController.instance;
            Effect = Instantiate(hc.artChargedEffect, hc.transform);
            Effect.GetComponent<tk2dSprite>().color = new Color(0.5f, 1f, 0.5f, 0.8f);
            EffectAnim = Effect.GetComponent<tk2dSpriteAnimator>();
            PlayMakerFSM fsm = hc.silkSpecialFSM;
            Cross = fsm.FsmStates.First(x => x.Name == "Parry Cross Slash");
            if (ModConfig.Ins.ForceUnlockParry.Value)
                PlayerData.instance.hasParry = true;

            var fsmState = fsm.FsmStates.First(x => x.Name == "Parry Start");
            var list = fsmState.Actions.ToList();
            /*var getSilkCost = list[1]; as GetPlayerDataVariable
            var takeSilk = list[2]; as TakeSilk*/
            list[1].Enabled = false;
            list[2].Enabled = false;
            list.Insert(0, new ParryStartAction());
            fsmState.Actions = list.ToArray();

            fsmState = fsm.FsmStates.First(x => x.Name == "Parry Stance");
            foreach (var action in fsmState.Actions)
            {
                if (action is Wait @wait)
                    Wait = @wait;
                if (action is DecelerateV2 dc)
                    Decelerate = dc;
            }

            fsmState = fsm.FsmStates.First(x => x.Name == "Parry Clash");
            list = fsmState.Actions.ToList();
            list.Add(new ShouldParrySlashAction());
            fsmState.Actions = list.ToArray();

            fsmState = fsm.FsmStates.First(x => x.Name == "Parry Recover");
            list = fsmState.Actions.ToList();
            list.Add(new ParryRecoverAction());
            fsmState.Actions = list.ToArray();
        }

        public void Update()
        {
            if (!ModConfig.Ins.NineSolsMode.Value)
                return;
            var player = HeroController.instance;
            if (player == null)
                return;

            // 分别更新蓄力状态和连击窗口状态
            switch (State)
            {
                case ChargeState.Charging:
                    UpdateCharging(player);
                    break;
                case ChargeState.Ready:
                    UpdateReady(player);
                    break;
            }
            if (ComboTimer > 0)
            {
                ComboTimer -= Time.deltaTime;
            }

            UpdateEffect(player);
        }

        public bool CheckSilk()
        {
            int silk = ModConfig.Ins.ParryCost.Value;
            if (silk == 0)
                return true;

            var player = HeroController.instance;
            var pd = player.playerData;
            if (Gameplay.FleaCharmTool.IsEquippedHud && pd.health >= pd.CurrentMaxHealth)
                silk--;

            if (player.playerData.silk >= silk)
            {
                player.TakeSilk(silk);
                return true;
            }
            return false;
        }
        public void StartCharging()
        {
            State = ChargeState.Charging;
            ChargeTimer = 0.5f; // 0.5秒蓄力时间
        }
        public void OnParryStart()
        {
            PlayerData.instance.TakeSilk(Cost);
            State = ChargeState.Charging;
            ChargeTimer = 0.5f; // 0.5秒蓄力时间
        }

        private void UpdateCharging(HeroController player)
        {
            // 检查输入 - 蓄力期间移动就取消
            if (CheckMovementInput(player))
            {
                State = ChargeState.Idle;
                return;
            }

            // 检查是否还按住招架键
            if (!player.inputHandler.inputActions.QuickCast.IsPressed)
            {
                State = ChargeState.Idle;
                return;
            }

            // 更新计时器
            ChargeTimer -= Time.deltaTime;

            if (ChargeTimer <= 0)
            {
                // 进入反击架势
                State = ChargeState.Ready;
                ReadyTimer = 2f; // 2秒反击窗口
            }
        }

        private void UpdateReady(HeroController player)
        {
            // 检查是否还按住招架键
            if (!player.inputHandler.inputActions.QuickCast.IsPressed)
            {
                // 放开招架键，执行允许反击的招架
                player.ThrowTool(true);
                State = ChargeState.Idle;
                return;
            }

            // 检查输入 - 反击架势期间移动就取消
            if (CheckMovementInput(player))
            {
                ReadyTimer = 0;
                State = ChargeState.Idle;
                return;
            }

            // 更新计时器
            ReadyTimer -= Time.deltaTime;

            if (ReadyTimer <= 0)
            {
                // 反击窗口结束
                State = ChargeState.Idle;
            }
        }

        private void UpdateEffect(HeroController player)
        {
            bool shouldShow = false;
            bool playFromStart = false;

            if (AllowCounter)
            {
                shouldShow = !Cross.Active;
                if (State == ChargeState.Ready)
                {
                    playFromStart = true;
                }
            }

            if (shouldShow && !Effect.activeSelf)
            {
                Effect.SetActive(true);
                if (playFromStart)
                    EffectAnim.PlayFromFrame(0);
                player.audioCtrl.PlaySound(GlobalEnums.HeroSounds.NAIL_ART_READY, true);
            }
            else if (!shouldShow && Effect.activeSelf)
            {
                Effect.SetActive(false);
                player.audioCtrl.StopSound(GlobalEnums.HeroSounds.NAIL_ART_READY);
            }
        }

        private bool CheckMovementInput(HeroController player)
        {
            var input = player.inputHandler.inputActions;
            return input.DreamNail.IsPressed || input.Dash.IsPressed
                /*|| input.Left.IsPressed || input.Right.IsPressed*/
                || input.Jump.IsPressed || input.Taunt.IsPressed || input.SuperDash.IsPressed
                || input.Cast.IsPressed || input.Attack.IsPressed /*|| input.Evade.IsPressed*/;
        }

        // 当招架成功时调用
        public bool OnParrySuccess()
        {
            var player = HeroController.instance;
            var config = ModConfig.Ins;
            int silk = config.ParrySilk.Value;
            player.AddSilk(silk, false);
            if (!config.NineSolsMode.Value)
            {
                var pd = player.playerData;
                int cost = pd.SilkSkillCost;
                if (pd.silk >= cost)
                {
                    player.TakeSilk(cost);
                    return true;
                }
                return false;
            }
            State = ChargeState.Idle;
            if (config.RefreshMoveAbility.Value)
            {
                player.airDashed = false;
                player.doubleJumped = false;
            }
            if (AllowCounter)
            {
                player.AddSilk(silk, false);
                ComboTimer = 2f;
                ReadyTimer = 0;
                return true;
            }
            ReadyTimer = 0;
            return false;
        }

        public void OnParryFailure()
        {
            ComboTimer = 0;
            ReadyTimer = 0;
        }
        public void OnHurt()
        {
            State = ChargeState.Idle;
            ReadyTimer = 0;
            ComboTimer = 0;
        }
    }
}