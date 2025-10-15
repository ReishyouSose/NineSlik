using HutongGames.PlayMaker;
using System.Linq;
using UnityEngine;

namespace NineSlik.FsmStateActions
{
    public class CounterAttackCheck
    {
        public static CounterAttackCheck Ins = null!;
        // 蓄力状态枚举
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

        public tk2dSpriteAnimator EffectAnim = null!;
        private GameObject effect = null!;
        public GameObject Effect
        {
            get
            {
                if (effect == null)
                {
                    var hc = HeroController.instance;
                    effect = Object.Instantiate(hc.artChargedEffect, hc.transform);
                    effect.GetComponent<tk2dSprite>().color = new Color(0.5f, 1f, 0.5f, 0.8f);
                    EffectAnim = effect.GetComponent<tk2dSpriteAnimator>();
                }
                return effect;
            }
        }
        private FsmState cross = null!;
        public FsmState Cross => cross ??= HeroController.instance.silkSpecialFSM.FsmStates.First(x => x.Name == "Parry Cross Slash");

        public bool AllowCounter => ReadyTimer > 0 || ComboTimer > 0;

        public void Update()
        {
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

        // 开始蓄力
        public void StartCharging()
        {
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
                player.audioCtrl.PlaySound(GlobalEnums.HeroSounds.NAIL_ART_READY, false);
            }
        }

        private bool CheckMovementInput(HeroController player)
        {
            var input = player.inputHandler.inputActions;
            return input.DreamNail.IsPressed || input.Dash.IsPressed || input.Left.IsPressed || input.Right.IsPressed
                || input.Jump.IsPressed || input.Taunt.IsPressed || input.SuperDash.IsPressed
                || input.Cast.IsPressed || input.Attack.IsPressed || input.Evade.IsPressed;
        }

        // 当招架成功时调用
        public bool OnParrySuccess()
        {
            var player = HeroController.instance;
            var ns = NineSilkMod.Ins;
            int silk = ns.ParrySilk.Value;
            player.AddSilk(silk, false);
            State = ChargeState.Idle;
            if (ns.RefreshMoveAbility.Value)
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