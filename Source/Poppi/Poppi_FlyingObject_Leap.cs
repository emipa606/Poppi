using RimWorld;
using UnityEngine;
using Verse;

namespace Poppi;

[StaticConstructorOnStartup]
public class Poppi_FlyingObject_Leap : ThingWithComps
{
    private readonly float speed = 28f;

    private Thing assignedTarget;

    private bool damageLaunched = true;

    private Vector3 destination;

    private bool drafted = false;

    private bool explosion;

    private Thing flyingThing;

    private DamageInfo? impactDamage;

    private bool initialize = true;

    private Vector3 origin;

    private Pawn pawn;

    private int ticksToImpact;

    public int weaponDmg = 0;

    private int StartingTicksToImpact
    {
        get
        {
            var num = Mathf.RoundToInt((origin - destination).magnitude / (speed / 100f));
            if (num < 1)
            {
                num = 1;
            }

            return num;
        }
    }

    private IntVec3 DestinationCell => new IntVec3(destination);

    protected virtual Vector3 ExactPosition
    {
        get
        {
            var vector = (destination - origin) * (1f - (ticksToImpact / (float)StartingTicksToImpact));
            return origin + vector + (Vector3.up * def.Altitude);
        }
    }

    protected virtual Quaternion ExactRotation => Quaternion.LookRotation(destination - origin);

    public override Vector3 DrawPos => ExactPosition;

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

    private void Initialize()
    {
        if (pawn != null)
        {
            FleckMaker.ThrowDustPuff(pawn.Position, pawn.Map, Rand.Range(1.2f, 1.8f));
        }

        initialize = false;
    }

    public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing, DamageInfo? impactDamage)
    {
        Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing, impactDamage);
    }

    public void Launch(Thing launcher, LocalTargetInfo targ, Thing flyingThing)
    {
        Launch(launcher, Position.ToVector3Shifted(), targ, flyingThing);
    }

    private void Launch(Thing launcher, Vector3 origin, LocalTargetInfo targ, Thing flyingThing,
        DamageInfo? newDamageInfo = null)
    {
        var spawned = flyingThing.Spawned;
        pawn = launcher as Pawn;
        if (spawned)
        {
            flyingThing.DeSpawn();
        }

        this.origin = origin;
        impactDamage = newDamageInfo;
        this.flyingThing = flyingThing;
        if (targ.Thing != null)
        {
            assignedTarget = targ.Thing;
        }

        destination = targ.Cell.ToVector3Shifted();
        ticksToImpact = StartingTicksToImpact;
        Initialize();
    }

    public override void Tick()
    {
        base.Tick();
        ticksToImpact--;
        if (!ExactPosition.InBounds(Map))
        {
            ticksToImpact++;
            Position = ExactPosition.ToIntVec3();
            Destroy();
        }
        else
        {
            Position = ExactPosition.ToIntVec3();
            if (Find.TickManager.TicksGame % 2 == 0)
            {
                FleckMaker.ThrowDustPuff(Position, Map, Rand.Range(0.6f, 0.8f));
            }

            if (!(ticksToImpact <= 0))
            {
                return;
            }

            if (DestinationCell.InBounds(Map))
            {
                Position = DestinationCell;
            }

            ImpactSomething();
        }
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        if (flyingThing != null)
        {
            if (flyingThing is Pawn)
            {
                if (!DrawPos.ToIntVec3().IsValid)
                {
                    return;
                }

                if (flyingThing is Pawn thing)
                {
                    thing.DrawNowAt(drawLoc, flip);
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

    private void ImpactSomething()
    {
        if (assignedTarget != null)
        {
            Impact(assignedTarget is Pawn target && target.GetPosture() != PawnPosture.Standing &&
                   (origin - destination).MagnitudeHorizontalSquared() >= 20.25f && Rand.Value > 0.2f
                ? null
                : assignedTarget);
        }
        else
        {
            Impact(null);
        }
    }

    protected virtual void Impact(Thing hitThing)
    {
        if (hitThing == null)
        {
            Pawn thing;
            if ((thing = Position.GetThingList(Map).FirstOrDefault(x => x == assignedTarget) as Pawn) !=
                null)
            {
                hitThing = thing;
            }
        }

        if (impactDamage != null)
        {
            hitThing?.TakeDamage(impactDamage.Value);
        }

        GenSpawn.Spawn(flyingThing, Position, Map);
        Destroy();
    }
}