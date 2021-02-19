using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Poppi
{
    // Token: 0x02000008 RID: 8
    public class CompLeaper : ThingComp
    {
        // Token: 0x0400001B RID: 27
        public float explosionRadius = 2f;

        // Token: 0x0400001A RID: 26
        private bool initialized = true;

        // Token: 0x0400001C RID: 28
        private int nextLeap;

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x0600001C RID: 28 RVA: 0x00002C54 File Offset: 0x00000E54
        private Pawn Pawn
        {
            get
            {
                var pawn = parent as Pawn;
                var flag = pawn == null;
                var flag2 = flag;
                if (flag2)
                {
                    Log.Error("pawn is null");
                }

                return pawn;
            }
        }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000020 RID: 32 RVA: 0x000032E4 File Offset: 0x000014E4
        private CompProperties_Leaper Props => (CompProperties_Leaper) props;

        // Token: 0x0600001D RID: 29 RVA: 0x00002C8C File Offset: 0x00000E8C
        public override void CompTick()
        {
            base.CompTick();
            var spawned = Pawn.Spawned;
            if (!spawned)
            {
                return;
            }

            var flag = Find.TickManager.TicksGame % 10 == 0;
            if (flag)
            {
                var flag2 = Pawn.Downed && !Pawn.Dead && Props.deathOnDown;
                if (flag2)
                {
                    GenExplosion.DoExplosion(Pawn.Position, Pawn.Map,
                        Rand.Range(explosionRadius * 0.5f, explosionRadius * 1.5f), RimWorld.DamageDefOf.Burn, Pawn,
                        Rand.Range(6, 10), 0f);
                    Pawn.Kill(null);
                }
            }

            var flag3 = Find.TickManager.TicksGame % nextLeap == 0 && !Pawn.Downed && !Pawn.Dead;
            if (!flag3)
            {
                return;
            }

            LocalTargetInfo a = null;
            var flag4 = Pawn.CurJob != null && Pawn.CurJob.targetA != null;
            if (flag4)
            {
                a = Pawn.jobs.curJob.targetA.Thing;
            }

            var flag5 = a != null && a.Thing != null;
            if (!flag5)
            {
                return;
            }

            var thing = a.Thing;
            var flag6 = thing is Pawn && thing.Spawned && thing.def.defName != "Poppi";
            if (!flag6)
            {
                return;
            }

            var lengthHorizontal = (thing.Position - Pawn.Position).LengthHorizontal;
            var flag7 = lengthHorizontal <= Props.leapRangeMax && lengthHorizontal > Props.leapRangeMin;
            if (flag7)
            {
                var flag8 = Rand.Chance(Props.GetLeapChance);
                if (flag8)
                {
                    var flag9 = CanHitTargetFrom(Pawn.Position, thing);
                    if (flag9)
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

                    var flag10 = Rand.Chance(0.5f);
                    MoteMaker.ThrowText(Pawn.DrawPos, Pawn.Map, flag10 ? "grrr" : "hsss");
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
                var flag11 = thing?.Faction != null;
                if (flag11)
                {
                    faction = thing.Faction;
                }

                var enumerable =
                    GenRadial.RadialCellsAround(Pawn.Position, Props.leapRangeMax, false);
                for (var i = 0; i < enumerable.Count(); i++)
                {
                    Pawn pawn = null;
                    var c = enumerable.ToArray()[i];
                    var flag12 = c.InBounds(Pawn.Map) && c.IsValid;
                    if (flag12)
                    {
                        pawn = c.GetFirstPawn(Pawn.Map);
                        var flag13 = pawn != null && pawn != thing && !pawn.Downed && !pawn.Dead &&
                                     pawn.RaceProps != null;
                        if (flag13)
                        {
                            var flag14 = pawn.Faction != null && pawn.Faction == faction;
                            if (flag14)
                            {
                                var flag15 = Rand.Chance(1f - Props.leapChance);
                                if (flag15)
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

                    var flag16 = pawn != null;
                    if (flag16)
                    {
                        var flag17 = CanHitTargetFrom(Pawn.Position, thing);
                        if (flag17)
                        {
                            var flag18 = !pawn.Downed && !pawn.Dead;
                            if (flag18)
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

        // Token: 0x0600001E RID: 30 RVA: 0x00003160 File Offset: 0x00001360
        private void LeapAttack(LocalTargetInfo target)
        {
            var flag = target != null && target.Cell != default;
            var flag2 = flag;
            if (!flag2)
            {
                return;
            }

            var flag3 = Pawn != null && Pawn.Position.IsValid && Pawn.Spawned && Pawn.Map != null && !Pawn.Downed &&
                        !Pawn.Dead && !target.Thing.DestroyedOrNull();
            if (!flag3)
            {
                return;
            }

            Pawn.jobs.StopAll();
            var poppi_FlyingObject_Leap =
                (Poppi_FlyingObject_Leap) GenSpawn.Spawn(ThingDef.Named("Poppi_FlyingObject_Leap"),
                    Pawn.Position, Pawn.Map);
            poppi_FlyingObject_Leap.Launch(Pawn, target.Cell, Pawn);
        }

        // Token: 0x0600001F RID: 31 RVA: 0x00003268 File Offset: 0x00001468
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            initialized = true;
            nextLeap = Mathf.RoundToInt(Rand.Range(Props.ticksBetweenLeapChance * 0.75f,
                1.25f * Props.ticksBetweenLeapChance));
            explosionRadius = Props.explodingLeaperRadius * Rand.Range(0.8f, 1.25f);
        }

        // Token: 0x06000021 RID: 33 RVA: 0x00003301 File Offset: 0x00001501
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref initialized, "initialized", true);
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00003320 File Offset: 0x00001520
        private bool CanHitTargetFrom(IntVec3 pawn, LocalTargetInfo target)
        {
            var flag = target.IsValid && target.CenterVector3.InBounds(Pawn.Map) && !target.Cell.Fogged(Pawn.Map) &&
                       target.Cell.Walkable(Pawn.Map);
            return flag && TryFindShootLineFromTo(pawn, target, out _);
        }

        // Token: 0x06000023 RID: 35 RVA: 0x000033A4 File Offset: 0x000015A4
        private bool TryFindShootLineFromTo(IntVec3 root, LocalTargetInfo targ, out ShootLine resultingLine)
        {
            var flag = targ.HasThing && targ.Thing.Map != Pawn.Map;
            bool result;
            if (flag)
            {
                resultingLine = default;
                result = false;
            }
            else
            {
                resultingLine = new ShootLine(root, targ.Cell);
                var flag2 = !GenSight.LineOfSightToEdges(root, targ.Cell, Pawn.Map, true);
                result = !flag2;
            }

            return result;
        }
    }
}