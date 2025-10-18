using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

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
                if (!ModConfig.Ins.NineSolsMode.Value)
                {
                    __result = true;
                    return false;
                }
                __result = PlayerData.instance.silk >= CounterAttackCheck.Cost;
                return false;
            }
            return true;
        }
        [HarmonyPatch(nameof(HeroController.Start))]
        [HarmonyPostfix]
        private static void Start(HeroController __instance)
        {
            CounterAttackCheck.Ins = __instance.gameObject.AddComponent<CounterAttackCheck>();
        }

        /*[HarmonyPatch(nameof(HeroController.CheckParry))]
        [HarmonyTranspiler]
        private static List<CodeInstruction> CheckParry(IEnumerable<CodeInstruction> codes)
        {
            var list = codes.ToList();
            GetLabel(list, out var label);
            if (label == null)
            {
                Debug.Log("Not found label");
                return list;
            }
            Debug.Log("Parry Match");
            MatchCode(list, true, label);
            MatchCode(list, false, label);
            return list;
        }

        [HarmonyPatch(nameof(HeroController.TakeDamage))]
        [HarmonyTranspiler]
        private static List<CodeInstruction> TakeDamage(IEnumerable<CodeInstruction> codes)
        {
            var list = codes.ToList();
            GetLabel(list, out var label);
            if (label == null)
            {
                Debug.Log("Not found label");
                return list;
            }
            Debug.Log("Damage Match");
            MatchCode(list, true, label);
            MatchCode(list, false, label);
            return list;
        }

        private static bool CheckDirection(HeroController player, bool right)
        {
            if (!ModConfig.Ins.ParryDirection.Value)
                return true;
            if (CounterAttackCheck.Ins.AllowCounter)
                return true;
            Debug.Log((player.cState.facingRight, right));
            return player.cState.facingRight == right;

        }
        private static void GetLabel(List<CodeInstruction> list, out object? label)
        {
            label = null;
            for (int i = 0; i < list.Count; i++)
            {
                var code = list[i];
                if (code.opcode != OpCodes.Ldfld)
                    continue;
                if (!code.operand.ToString().Contains("parrying"))
                    continue;
                label = list[i + 1].operand;
                return;
            }
        }
        private static void MatchCode(List<CodeInstruction> list, bool right, object label)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var code = list[i];
                if (code.opcode != OpCodes.Call)
                    continue;
                var oprand = code.operand.ToString();
                string face = right ? "FaceRight" : "FaceLeft";
                if (oprand.Contains(face))
                {
                    list.InsertRange(i - 1, AddDirIL(right, label));
                    Debug.Log($"Add dir check {face}");
                    break;
                }
            }
        }
        private static List<CodeInstruction> AddDirIL(bool right, object label)
        {
            return new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(right ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(HeroControllerPatch),nameof(CheckDirection))),
                new CodeInstruction(OpCodes.Brfalse_S, label)
            };
        }*/
    }
}
