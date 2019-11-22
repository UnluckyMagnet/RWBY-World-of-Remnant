using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;
using Verse.AI;
using System.Linq;
using System.Reflection;

namespace RWBYRemnant
{
    public class Aura_Raven : Aura
    {
        public override void Tick()
        {
            base.Tick();
            cooldownTicks--;
        }

        public override Color GetColor()
        {
            return new Color(1.0f, 0f, 0f);
        }

        public bool RegisterBondedPawn(Pawn p)
        {
            if (!p.Dead && p.RaceProps.Humanlike)
            {
                if (pawn.relations.OpinionOf(p) < 30)
                {
                    Messages.Message("RavenBondOpinionLow".Translate(pawn.LabelShort, p.LabelShort), pawn, MessageTypeDefOf.NegativeEvent);
                    return false;
                }
                bondedPawns.Add(p);
                return true;
            }
            return false;
        }

        public void CreatePortal(Pawn p)
        {
            Thing portalA = ThingMaker.MakeThing(RWBYDefOf.Raven_Portal);
            GenSpawn.Spawn(portalA, pawn.Position, pawn.Map);
            Thing portalB = ThingMaker.MakeThing(RWBYDefOf.Raven_Portal);
            GenSpawn.Spawn(portalB, p.Position, p.Map);

            if (portalA is ThingWithComps_Portal tPortalA && tPortalA.Spawned)
            {
                tPortalA.RegisterConnectedPortal(portalB);
            }
            if (portalB is ThingWithComps_Portal tPortalB && tPortalB.Spawned)
            {
                tPortalB.RegisterConnectedPortal(portalA);
            }

            RWBYDefOf.Shoot_Fireball.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
            RWBYDefOf.Shoot_Fireball.PlayOneShot(new TargetInfo(p.Position, p.Map, false));
            cooldownTicks = GenTicks.SecondsToTicks(30);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            foreach (Pawn bondedPawn in bondedPawns)
            {
                if (bondedPawn.Dead) continue;
                yield return new Command_Action
                {
                    disabled = cooldownTicks > 0,
                    disabledReason = "RavenPortalDisabled".Translate(),
                    defaultLabel = "RavenPortalBondLabel".Translate(bondedPawn.LabelShort),
                    defaultDesc = "RavenPortalBondDescription".Translate(bondedPawn.LabelShort),
                    icon = RWBYDefOf.Raven_Portal.uiIcon,
                    action = delegate()
                    {
                        CreatePortal(bondedPawn);
                    }
                };
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref cooldownTicks, "cooldownTicks", 0, false);
            Scribe_Collections.Look(ref bondedPawns, false, "bondedPawns", LookMode.Reference);
            if (bondedPawns == null)
            {
                bondedPawns = new HashSet<Pawn>();
            }
        }

        private HashSet<Pawn> bondedPawns = new HashSet<Pawn>();
        private int cooldownTicks = 0;
    }
}
