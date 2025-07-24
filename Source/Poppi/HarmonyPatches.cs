using System.Reflection;
using HarmonyLib;
using Verse;

namespace Poppi;

[StaticConstructorOnStartup]
internal class HarmonyPatches
{
    static HarmonyPatches()
    {
        new Harmony("rimworld.torann.poppi").PatchAll(Assembly.GetExecutingAssembly());
    }
}