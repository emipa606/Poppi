using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Poppi
{
    // Token: 0x02000006 RID: 6
    [StaticConstructorOnStartup]
    public class Poppi_FlyingObject_Leap : ThingWithComps
    {
        // Token: 0x04000005 RID: 5
        private readonly float speed = 28f;

        // Token: 0x04000008 RID: 8
        private Thing assignedTarget;

        // Token: 0x0400000B RID: 11
        private bool damageLaunched = true;

        // Token: 0x04000004 RID: 4
        private Vector3 destination;

        // Token: 0x04000006 RID: 6
        private bool drafted = false;

        // Token: 0x0400000C RID: 12
        private bool explosion;

        // Token: 0x04000009 RID: 9
        private Thing flyingThing;

        // Token: 0x0400000A RID: 10
        private DamageInfo? impactDamage;

        // Token: 0x0400000E RID: 14
        private bool initialize = true;

        // Token: 0x04000003 RID: 3
        private Vector3 origin;

        // Token: 0x0400000F RID: 15
        private Pawn pawn;

        // Token: 0x04000007 RID: 7
        private int ticksToImpact;

        // Token: 0x0400000D RID: 13
        public int weaponDmg = 0;

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000008 RID: 8 RVA: 0x0000247C File Offset: 0x0000067C
        private int StartingTicksToImpact
        {
            get
            {
                var num = Mathf.RoundToInt((origin - destination).magnitude / (speed / 100f));
                var flag = num < 1;
                var flag2 = flag;
                if (flag2)
                {
                    num = 1;
                }

                return num;
            }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000009 RID: 9 RVA: 0x000024CC File Offset: 0x000006CC
        private IntVec3 DestinationCell => new IntVec3(destination);

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600000A RID: 10 RVA: 0x000024EC File Offset: 0x000006EC
        protected virtual Vector3 ExactPosition
        {
            get
            {
                var vector = (destination - origin) * (1f - (ticksToImpact / (float) StartingTicksToImpact));
                return origin + vector + (Vector3.up * def.Altitude);
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x0600000B RID: 11 RVA: 0x00002550 File Offset: 0x00000750
        protected virtual Quaternion ExactRotation => Quaternion.LookRotation(destination - origin);

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x0600000C RID: 12 RVA: 0x00002578 File Offset: 0x00000778
        public override Vector3 DrawPos => ExactPosition;

        // Token: 0x0600000D RID: 13 RVA: 0x00002590 File Offset: 0x00000790
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref origin, "origin");
            Scribe_Values.Look(ref destination, "destination");
            Scribe_Values.Look(ref ticksToImpact, "ticksToImpact");
            Scribe_Values.Look(ref damageLaunched, "damageLaunched", true);
            Scribe_Values.Look(ref explosion, "explosion");
            Scribe_References.Look(ref assignedTarget, "assignedTarget");
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Deep.Look(ref flyingThing, "flyingThing");
        }

        // Token: 0x0600000E RID: 14 RVA: 0x00002650 File Offset: 0x00000850
        private void Initialize()
        {
            var flag = pawn != null;
            if (flag)
            {
                MoteMaker.ThrowDustPuff(pawn.Position, pawn.Map, Rand.Range(1.2f, 1.8f));
            }

            initialize = false;
        }

        // Token: 0x0600000F RID: 15 RVA: 0x000026A0 File Offset: 0x000008A0
        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, DamageInfo? impactDamage)
        {
            Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing, impactDamage);
        }

        // Token: 0x06000010 RID: 16 RVA: 0x000026C8 File Offset: 0x000008C8
        public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing)
        {
            Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing);
        }

        // Token: 0x06000011 RID: 17 RVA: 0x000026F8 File Offset: 0x000008F8
        private void Launch(Thing launcher, Vector3 origin, LocalTargetInfo targ, Thing flyingThing,
            DamageInfo? newDamageInfo = null)
        {
            var spawned = flyingThing.Spawned;
            pawn = launcher as Pawn;
            var flag = spawned;
            if (flag)
            {
                flyingThing.DeSpawn();
            }

            this.origin = origin;
            impactDamage = newDamageInfo;
            this.flyingThing = flyingThing;
            var flag2 = targ.Thing != null;
            var flag3 = flag2;
            if (flag3)
            {
                assignedTarget = targ.Thing;
            }

            destination = targ.Cell.ToVector3Shifted();
            ticksToImpact = StartingTicksToImpact;
            Initialize();
        }

        // Token: 0x06000012 RID: 18 RVA: 0x0000278C File Offset: 0x0000098C
        public override void Tick()
        {
            base.Tick();
            ticksToImpact--;
            var flag = !ExactPosition.InBounds(Map);
            var flag2 = flag;
            if (flag2)
            {
                ticksToImpact++;
                Position = ExactPosition.ToIntVec3();
                Destroy();
            }
            else
            {
                Position = ExactPosition.ToIntVec3();
                var flag3 = Find.TickManager.TicksGame % 2 == 0;
                if (flag3)
                {
                    MoteMaker.ThrowDustPuff(Position, Map, Rand.Range(0.6f, 0.8f));
                }

                var flag4 = ticksToImpact <= 0;
                var flag5 = flag4;
                if (!flag5)
                {
                    return;
                }

                var flag6 = DestinationCell.InBounds(Map);
                var flag7 = flag6;
                if (flag7)
                {
                    Position = DestinationCell;
                }

                ImpactSomething();
            }
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00002894 File Offset: 0x00000A94
        public override void Draw()
        {
            var flag = flyingThing != null;
            var flag2 = flag;
            if (flag2)
            {
                var flag3 = flyingThing is Pawn;
                var flag4 = flag3;
                if (flag4)
                {
                    var flag5 = !DrawPos.ToIntVec3().IsValid;
                    var flag6 = flag5;
                    if (flag6)
                    {
                        return;
                    }

                    if (flyingThing is Pawn thing)
                    {
                        thing.Drawer.DrawAt(DrawPos);
                    }
                }
                else
                {
                    Graphics.DrawMesh(MeshPool.plane10, DrawPos, ExactRotation, flyingThing.def.DrawMatSingle, 0);
                }
            }
            else
            {
                if (flyingThing != null)
                {
                    Graphics.DrawMesh(MeshPool.plane10, DrawPos, ExactRotation, flyingThing.def.DrawMatSingle, 0);
                }
            }

            Comps_PostDraw();
        }

        // Token: 0x06000014 RID: 20 RVA: 0x0000297C File Offset: 0x00000B7C
        private void DrawEffects(Vector3 pawnVec, Pawn flyingPawn, int magnitude)
        {
            var flag = !pawn.Dead && !pawn.Downed;
            var flag2 = flag;
            if (flag2)
            {
            }
        }

        // Token: 0x06000015 RID: 21 RVA: 0x000029B0 File Offset: 0x00000BB0
        private void ImpactSomething()
        {
            if (assignedTarget != null)
            {
                var flag3 = assignedTarget is Pawn target && target.GetPosture() != PawnPosture.Standing &&
                            (origin - destination).MagnitudeHorizontalSquared() >= 20.25f && Rand.Value > 0.2f;
                Impact(flag3 ? null : assignedTarget);
            }
            else
            {
                Impact(null);
            }
        }

        // Token: 0x06000016 RID: 22 RVA: 0x00002A44 File Offset: 0x00000C44
        protected virtual void Impact(Thing hitThing)
        {
            var flag = hitThing == null;
            var flag2 = flag;
            if (flag2)
            {
                Pawn thing;
                var flag3 = (thing = Position.GetThingList(Map).FirstOrDefault(x => x == assignedTarget) as Pawn) !=
                            null;
                var flag4 = flag3;
                if (flag4)
                {
                    hitThing = thing;
                }
            }

            var flag5 = impactDamage != null;
            var flag6 = flag5;
            if (flag6)
            {
                hitThing?.TakeDamage(impactDamage.Value);
            }

            GenSpawn.Spawn(flyingThing, Position, Map);
            Destroy();
        }
    }
}