using RimWorld;
using Verse;

namespace Poppi
{
    // Token: 0x02000009 RID: 9
    public class DeathWorker : DeathActionWorker
    {
        // Token: 0x06000025 RID: 37 RVA: 0x0000344C File Offset: 0x0000164C
        public override void PawnDied(Corpse corpse)
        {
            var num = 2f;
            var flag = !corpse.InnerPawn.health.hediffSet.HasHediff(HediffDefOf.Anesthetic);
            if (!flag)
            {
                return;
            }

            var comp = corpse.InnerPawn.GetComp<CompLeaper>();
            var flag2 = comp != null;
            if (flag2)
            {
                num = comp.explosionRadius;
            }

            var flag3 = corpse.InnerPawn.ageTracker.CurLifeStageIndex == 0;
            var flag4 = flag3;
            if (flag4)
            {
                num *= Rand.Range(0.6f, 0.8f);
            }
            else
            {
                var flag5 = corpse.InnerPawn.ageTracker.CurLifeStageIndex == 1;
                var flag6 = flag5;
                if (flag6)
                {
                    num *= Rand.Range(1f, 1.5f);
                }
                else
                {
                    num *= Rand.Range(1.3f, 1.7f);
                }
            }

            GenExplosion.DoExplosion(corpse.Position, corpse.Map, num, RimWorld.DamageDefOf.Burn, corpse.InnerPawn,
                Rand.Range(6, 10), 0f);
        }
    }
}