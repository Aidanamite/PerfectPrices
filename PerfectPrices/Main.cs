using BepInEx;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace PerfectPrices
{
    [BepInPlugin("Aidanamite.PerfectPrices", "PerfectPrices", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        internal static Assembly modAssembly = Assembly.GetExecutingAssembly();
        internal static string modName = $"{modAssembly.GetName().Name}";
        internal static string modDir = $"{Environment.CurrentDirectory}\\BepInEx\\{modName}";

        void Awake()
        {
            new Harmony($"com.Aidanamite.{modName}").PatchAll(modAssembly);
            Logger.LogInfo($"{modName} has loaded");
        }
    }

    [HarmonyPatch(typeof(NotebookPanel),"DetailItem")]
    public class Patch_NotebookItemDetails
    {
        static Color tooCheapDefault = Color.clear;
        static Color cheapDefault = Color.clear;
        static Color expensiveDefault = Color.clear;
        static Color tooExpensiveDefault = Color.clear;
        static void Postfix(NotebookPanel __instance, ItemMaster item, bool unlocked)
        {
            if (item == null)
                return;

            if (tooCheapDefault.a == 0)
                tooCheapDefault = __instance.textLastTooCheap.color;
            if (cheapDefault.a == 0)
                cheapDefault = __instance.textLastCheap.color;
            if (expensiveDefault.a == 0)
                expensiveDefault = __instance.textLastExpensive.color;
            if (tooExpensiveDefault.a == 0)
                tooExpensiveDefault = __instance.textLastTooExpensive.color;

            var pop = ItemPriceManager.Instance.GetPopularity(item);
            ItemPriceManager.Instance.SetPopularity(item, ItemPriceInfo.Popularity.Neutral);

            int current = ItemPriceManager.Instance.GetLastPrice(item, ItemPriceValoration.TooCheap);
            int best = ItemPriceManager.Instance.GetMinCorrectPrice(item);
            __instance.textLastTooCheap.color = (current == best) ? new Color(0,0.8f,0.2f) : tooCheapDefault;

            current = ItemPriceManager.Instance.GetLastPrice(item, ItemPriceValoration.Cheap);
            best = ItemPriceManager.Instance.GetMaxCorrectPrice(item);
            __instance.textLastCheap.color = (current == best) ? new Color(0, 0.8f, 0.2f) : cheapDefault;

            current = ItemPriceManager.Instance.GetLastPrice(item, ItemPriceValoration.Expensive);
            best = ItemPriceManager.Instance.GetMaxOverpricedPrice(item);
            __instance.textLastExpensive.color = (current == best) ? new Color(0, 0.8f, 0.2f) : expensiveDefault;

            current = ItemPriceManager.Instance.GetLastPrice(item, ItemPriceValoration.TooExpensive);
            best++;
            __instance.textLastTooExpensive.color = (current == best) ? new Color(0, 0.8f, 0.2f) : tooExpensiveDefault;

            ItemPriceManager.Instance.SetPopularity(item, pop);
        }
    }
}