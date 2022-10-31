using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Poppi;

[StaticConstructorOnStartup]
internal class HarmonyPatches
{
    private static readonly Type patchType = typeof(HarmonyPatches);

    static HarmonyPatches()
    {
        var harmonyInstance = new Harmony("rimworld.torann.poppi");
        harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
    }

    [HarmonyPatch(typeof(CompMilkable), "CompInspectStringExtra", null)]
    public class CompMilkable_Patch
    {
        public static void Postfix(CompMilkable __instance, ref string __result)
        {
            if (__instance.parent.def.defName == "Poppi")
            {
                __result = "Chemical extraction: " + __instance.Fullness.ToStringPercent();
            }
        }
    }
}