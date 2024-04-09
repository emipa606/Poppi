using HarmonyLib;
using RimWorld;
using Verse;

namespace Poppi;

[HarmonyPatch(typeof(CompMilkable), nameof(CompMilkable.CompInspectStringExtra), null)]
public class CompMilkable_Patch
{
    public static void Postfix(CompMilkable __instance, ref string __result)
    {
        if (__instance.parent.def.defName == "Poppi")
        {
            __result = $"Chemical extraction: {__instance.Fullness.ToStringPercent()}";
        }
    }
}