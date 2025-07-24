using HarmonyLib;
using RimWorld;
using Verse;

namespace Poppi;

[HarmonyPatch(typeof(StaggerHandler), nameof(StaggerHandler.StaggerFor))]
public class StaggerHandler_StaggerFor
{
    public static bool Prefix(Pawn ___parent)
    {
        return ___parent.def.defName != "Poppi";
    }
}