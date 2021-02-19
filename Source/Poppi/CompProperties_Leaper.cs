using UnityEngine;
using Verse;

namespace Poppi
{
    // Token: 0x02000007 RID: 7
    public class CompProperties_Leaper : CompProperties
    {
        // Token: 0x04000014 RID: 20
        public readonly bool bouncingLeaper = false;

        // Token: 0x04000015 RID: 21
        public readonly bool deathOnDown = false;

        // Token: 0x04000017 RID: 23
        private readonly float explodingLeaperChance = 0.2f;

        // Token: 0x04000018 RID: 24
        public readonly float explodingLeaperRadius = 2f;

        // Token: 0x04000012 RID: 18
        public readonly float leapChance = 0.5f;

        // Token: 0x04000010 RID: 16
        public readonly float leapRangeMax = 8f;

        // Token: 0x04000011 RID: 17
        public readonly float leapRangeMin = 2f;

        // Token: 0x04000019 RID: 25
        public readonly bool textMotes = true;

        // Token: 0x04000013 RID: 19
        public readonly float ticksBetweenLeapChance = 100f;

        // Token: 0x04000016 RID: 22
        public bool explodingLeaper = false;

        // Token: 0x0600001B RID: 27 RVA: 0x00002BD0 File Offset: 0x00000DD0
        public CompProperties_Leaper()
        {
            compClass = typeof(CompLeaper);
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000019 RID: 25 RVA: 0x00002B90 File Offset: 0x00000D90
        public float GetLeapChance => Mathf.Clamp01(leapChance);

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x0600001A RID: 26 RVA: 0x00002BB0 File Offset: 0x00000DB0
        public float GetExplodingLeaperChance => Mathf.Clamp01(explodingLeaperChance);
    }
}