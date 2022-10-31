using UnityEngine;
using Verse;

namespace Poppi;

public class CompProperties_Leaper : CompProperties
{
    public readonly bool bouncingLeaper = false;

    public readonly bool deathOnDown = false;

    private readonly float explodingLeaperChance = 0.2f;

    public readonly float explodingLeaperRadius = 2f;

    public readonly float leapChance = 0.5f;

    public readonly float leapRangeMax = 8f;

    public readonly float leapRangeMin = 2f;

    public readonly bool textMotes = true;

    public readonly float ticksBetweenLeapChance = 100f;

    public bool explodingLeaper = false;

    public CompProperties_Leaper()
    {
        compClass = typeof(CompLeaper);
    }

    public float GetLeapChance => Mathf.Clamp01(leapChance);

    public float GetExplodingLeaperChance => Mathf.Clamp01(explodingLeaperChance);
}