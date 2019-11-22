using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace RWBYRemnant
{
    public class CompPropertiesThrowAble : CompProperties
    {
        public int throwAbleRange;
        public ThingDef projectile;

        public CompPropertiesThrowAble()
        {
            compClass = typeof(CompThrowAble);
        }
    }

    class CompThrowAble : CompUseEffect
    {
        public CompPropertiesThrowAble Props => (CompPropertiesThrowAble)props;

        public int cooldownTick = 0;

        public Pawn GetPawn()
        {
            if (parent.holdingOwner != null && parent.holdingOwner.Owner != null && parent.holdingOwner.Owner.ParentHolder != null && parent.holdingOwner.Owner.ParentHolder.GetType().Equals(typeof(Pawn))) return (Pawn)parent.holdingOwner.Owner.ParentHolder;
            return null;
        }

        public void ThrowAt(IntVec3 target)
        {
            if (GetPawn() == null) return;
            Verb_Shoot verb_Shoot = new Verb_Shoot();
            verb_Shoot.caster = GetPawn();
            VerbProperties verbProperties = new VerbProperties
            {
                verbClass = typeof(Verb_Shoot),
                hasStandardCommand = true,
                defaultProjectile = Props.projectile,
                warmupTime = 0,
                range = Props.throwAbleRange,
            };
            verb_Shoot.verbProps = verbProperties;

            if (verb_Shoot.TryFindShootLineFromTo(GetPawn().Position, target, out ShootLine shootLine))
            {
                Projectile projectile = (Projectile)GenSpawn.Spawn(Props.projectile, GetPawn().Position, GetPawn().Map);
                projectile.Launch(GetPawn(), target, target, ProjectileHitFlags.IntendedTarget);
                RWBYDefOf.Throw_Cinder_Spear.PlayOneShot(new TargetInfo(GetPawn().Position, GetPawn().Map, false));
                cooldownTick = Find.TickManager.TicksGame;
                parent.stackCount -= 1;
                if (parent.stackCount == 0) parent.Destroy();
            }
                
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (GetPawn() == null) yield break;
            if (!GetPawn().IsColonist) yield break;
            if (!GetPawn().Drafted && !GetPawn().IsFighting()) yield break;

            bool disabled = false;
            if (Find.TickManager.TicksGame <= cooldownTick) disabled = true;

            yield return new Command_TargetWithRadius
            {
                defaultLabel = "ThrowDustLabel".Translate(parent.def.label),
                defaultDesc = "ThrowDustDescription".Translate(parent.def.label),
                icon = parent.def.uiIcon,
                defaultIconColor = parent.def.uiIconColor,
                targetingParams = TargetingParametersRWBY.OnGround(),
                action = delegate (Thing target)
                {
                    ThrowAt(UI.MouseMapPosition().ToIntVec3());
                },
                center = GetPawn().Position,
                range = Props.throwAbleRange,
                disabled = disabled
            };
        }
    }
}
