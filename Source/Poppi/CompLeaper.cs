using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Poppi;

public class CompLeaper : ThingComp
{
    public float explosionRadius = 2f;

    private bool initialized = true;

    private int nextLeap;

    private Pawn Pawn
    {
        get
        {
            if (parent is Pawn pawn)
            {
                return pawn;
            }

            Log.Error("pawn is null");
            return null;
        }
    }

    private CompProperties_Leaper Props => (CompProperties_Leaper)props;

    public override void CompTick()
    {
        base.CompTick();
        var spawned = Pawn.Spawned;
        if (!spawned)
        {
            return;
        }

        if (Find.TickManager.TicksGame % 10 == 0)
        {
            if (Pawn.Downed && !Pawn.Dead && Props.deathOnDown)
            {
                GenExplosion.DoExplosion(Pawn.Position, Pawn.Map,
                    Rand.Range(explosionRadius * 0.5f, explosionRadius * 1.5f), RimWorld.DamageDefOf.Burn, Pawn,
                    Rand.Range(6, 10), 0f);
                Pawn.Kill(null);
            }
        }

        if (!(Find.TickManager.TicksGame % nextLeap == 0 && !Pawn.Downed && !Pawn.Dead))
        {
            return;
        }

        LocalTargetInfo a = null;
        if (Pawn.CurJob != null && Pawn.CurJob.targetA != null)
        {
            a = Pawn.jobs.curJob.targetA.Thing;
        }

        if (!(a != null && a.Thing != null))
        {
            return;
        }

        var thing = a.Thing;
        if (!(thing is Pawn && thing.Spawned && thing.def.defName != "Poppi"))
        {
            return;
        }

        var lengthHorizontal = (thing.Position - Pawn.Position).LengthHorizontal;
        if (lengthHorizontal <= Props.leapRangeMax && lengthHorizontal > Props.leapRangeMin)
        {
            if (Rand.Chance(Props.GetLeapChance))
            {
                if (CanHitTargetFrom(Pawn.Position, thing))
                {
                    LeapAttack(thing);
                }
            }
            else
            {
                var textMotes = Props.textMotes;
                if (!textMotes)
                {
                    return;
                }

                MoteMaker.ThrowText(Pawn.DrawPos, Pawn.Map, Rand.Chance(0.5f) ? "grrr" : "hsss");
            }
        }
        else
        {
            var bouncingLeaper = Props.bouncingLeaper;
            if (!bouncingLeaper)
            {
                return;
            }

            Faction faction = null;
            if (thing?.Faction != null)
            {
                faction = thing.Faction;
            }

            var enumerable =
                GenRadial.RadialCellsAround(Pawn.Position, Props.leapRangeMax, false);
            for (var i = 0; i < enumerable.Count(); i++)
            {
                Pawn pawn = null;
                var c = enumerable.ToArray()[i];
                if (c.InBounds(Pawn.Map) && c.IsValid)
                {
                    pawn = c.GetFirstPawn(Pawn.Map);
                    if (pawn != null && pawn != thing && !pawn.Downed && !pawn.Dead &&
                        pawn.RaceProps != null)
                    {
                        if (pawn.Faction != null && pawn.Faction == faction)
                        {
                            if (Rand.Chance(1f - Props.leapChance))
                            {
                                i = enumerable.Count();
                            }
                            else
                            {
                                pawn = null;
                            }
                        }
                        else
                        {
                            pawn = null;
                        }
                    }
                    else
                    {
                        pawn = null;
                    }
                }

                if (pawn != null)
                {
                    if (CanHitTargetFrom(Pawn.Position, thing))
                    {
                        if (!pawn.Downed && !pawn.Dead)
                        {
                            LeapAttack(pawn);
                        }

                        LeapAttack(pawn);
                        break;
                    }
                }

                enumerable.GetEnumerator().MoveNext();
            }
        }
    }

    private void LeapAttack(LocalTargetInfo target)
    {
        if (!(target != null && target.Cell != default))
        {
            return;
        }

        if (!(Pawn is { Position.IsValid: true, Spawned: true, Map: { }, Downed: false, Dead: false } &&
              !target.Thing.DestroyedOrNull()))
        {
            return;
        }

        Pawn.jobs.StopAll();
        var poppi_FlyingObject_Leap =
            (Poppi_FlyingObject_Leap)GenSpawn.Spawn(ThingDef.Named("Poppi_FlyingObject_Leap"),
                Pawn.Position, Pawn.Map);
        poppi_FlyingObject_Leap.Launch(Pawn, target.Cell, Pawn);
    }

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        initialized = true;
        nextLeap = Mathf.RoundToInt(Rand.Range(Props.ticksBetweenLeapChance * 0.75f,
            1.25f * Props.ticksBetweenLeapChance));
        explosionRadius = Props.explodingLeaperRadius * Rand.Range(0.8f, 1.25f);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref initialized, "initialized", true);
    }

    private bool CanHitTargetFrom(IntVec3 pawn, LocalTargetInfo target)
    {
        return target.IsValid && target.CenterVector3.InBounds(Pawn.Map) && !target.Cell.Fogged(Pawn.Map) &&
               target.Cell.Walkable(Pawn.Map) && TryFindShootLineFromTo(pawn, target, out _);
    }

    private bool TryFindShootLineFromTo(IntVec3 root, LocalTargetInfo targ, out ShootLine resultingLine)
    {
        bool result;
        if (targ.HasThing && targ.Thing.Map != Pawn.Map)
        {
            resultingLine = default;
            result = false;
        }
        else
        {
            resultingLine = new ShootLine(root, targ.Cell);
            result = GenSight.LineOfSightToEdges(root, targ.Cell, Pawn.Map, true);
        }

        return result;
    }
}