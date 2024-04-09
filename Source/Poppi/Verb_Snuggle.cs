using RimWorld;
using UnityEngine;
using Verse;

namespace Poppi;

public class Verb_Snuggle : Verb_MeleeAttack
{
    protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
    {
        var damageResult = new DamageWorker.DamageResult();
        var dinfo = new DamageInfo(RimWorld.DamageDefOf.Scratch, (int)tool.power, 0f, -1f, base.CasterPawn);
        damageResult.hitThing = target.Thing;
        damageResult.totalDamageDealt = Mathf.Min(target.Thing.HitPoints, dinfo.Amount);
        for (var i = 0; i < 8; i++)
        {
            var c = target.Cell + GenAdj.AdjacentCells[i];
            var pawn = c.GetFirstPawn(target.Thing.Map);
            if (!(pawn != null && pawn.Faction != caster.Faction))
            {
                continue;
            }

            var dinfo2 = new DamageInfo(DamageDefOf.Poppi_Cleave, (int)(tool.power * 0.6f), 0f, -1f,
                base.CasterPawn);
            pawn.TakeDamage(dinfo2);
            FleckMaker.ThrowMicroSparks(pawn.Position.ToVector3(), target.Thing.Map);
            Poppi_MoteMaker.ThrowCrossStrike(pawn.Position.ToVector3Shifted(), pawn.Map, 0.4f);
            Poppi_MoteMaker.ThrowBloodSquirt(pawn.Position.ToVector3Shifted(), pawn.Map, 1f);
        }

        Poppi_MoteMaker.ThrowCrossStrike(target.Thing.Position.ToVector3Shifted(), target.Thing.Map, 0.5f);
        Poppi_MoteMaker.ThrowBloodSquirt(target.Thing.Position.ToVector3Shifted(), target.Thing.Map, 1.2f);
        target.Thing.TakeDamage(dinfo);
        return damageResult;
    }
}