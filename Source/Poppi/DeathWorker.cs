using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Poppi;

public class DeathWorker : DeathActionWorker
{
    public override void PawnDied(Corpse corpse, Lord prevLord)
    {
        var num = 2f;
        if (corpse.InnerPawn.health.hediffSet.HasHediff(HediffDefOf.Anesthetic))
        {
            return;
        }

        var comp = corpse.InnerPawn.GetComp<CompLeaper>();
        if (comp != null)
        {
            num = comp.explosionRadius;
        }

        switch (corpse.InnerPawn.ageTracker.CurLifeStageIndex)
        {
            case 0:
                num *= Rand.Range(0.6f, 0.8f);
                break;
            case 1:
                num *= Rand.Range(1f, 1.5f);
                break;
            default:
                num *= Rand.Range(1.3f, 1.7f);
                break;
        }

        GenExplosion.DoExplosion(corpse.Position, corpse.Map, num, RimWorld.DamageDefOf.Burn, corpse.InnerPawn,
            Rand.Range(6, 10), 0f);
    }
}