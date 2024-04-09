using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace Poppi;

[StaticConstructorOnStartup]
internal class HarmonyPatches
{
    private static readonly Type patchType = typeof(HarmonyPatches);

    static HarmonyPatches()
    {
        new Harmony("rimworld.torann.poppi").PatchAll(Assembly.GetExecutingAssembly());
    }
}