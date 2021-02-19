using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Poppi
{
    // Token: 0x02000002 RID: 2
    [StaticConstructorOnStartup]
    internal class HarmonyPatches
    {
        // Token: 0x04000001 RID: 1
        private static readonly Type patchType = typeof(HarmonyPatches);

        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        static HarmonyPatches()
        {
            var harmonyInstance = new Harmony("rimworld.torann.poppi");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

        // Token: 0x0200000A RID: 10
        [HarmonyPatch(typeof(CompMilkable), "CompInspectStringExtra", null)]
        public class CompMilkable_Patch
        {
            // Token: 0x06000027 RID: 39 RVA: 0x00003570 File Offset: 0x00001770
            public static void Postfix(CompMilkable __instance, ref string __result)
            {
                var flag = __instance.parent.def.defName == "Poppi";
                if (flag)
                {
                    __result = "Chemical extraction: " + __instance.Fullness.ToStringPercent();
                }
            }
        }
    }
}