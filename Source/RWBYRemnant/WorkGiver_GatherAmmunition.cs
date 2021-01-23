using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RWBYRemnant
{
    class WorkGiver_GatherAmmunition : WorkGiver_Scanner
    {
        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.None;
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            List<ThingDef> thingDefs = AmmunitionUtility.NeedsAmmunition(pawn).ToList();
            if (thingDefs.Count() == 0) return new List<Thing>();
            return pawn.Map.GetDirectlyHeldThings().Where(t => !t.IsForbidden(pawn) && thingDefs.Contains(t.def) && (!pawn.inventory.GetDirectlyHeldThings().Any(p => p.def == t.def) || pawn.inventory.GetDirectlyHeldThings().Any(p => p.def == t.def && p.stackCount < 5)));
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            if (!LoadedModManager.GetMod<RemnantMod>().GetSettings<RemnantModSettings>().autoCollectAmmunition) return true;
            if (pawn.NonHumanlikeOrWildMan()) return true;
            List<ThingDef> thingDefs = AmmunitionUtility.NeedsAmmunition(pawn).ToList();
            thingDefs.RemoveAll(t => pawn.inventory.GetDirectlyHeldThings().Any(d => d.def == t && d.stackCount >= 5));
            if (thingDefs.Count() == 0) return true;
            if (pawn.Map.GetDirectlyHeldThings().Where(t => !t.IsForbidden(pawn) && thingDefs.Contains(t.def)).Count() == 0) return true;
            foreach (ThingDef thingDef in thingDefs)
            {
                if ((!pawn.inventory.GetDirectlyHeldThings().Any(t => t.def == thingDef) || pawn.inventory.GetDirectlyHeldThings().Any(t => t.def == thingDef && t.stackCount < 5)) && pawn.Map.GetDirectlyHeldThings().Any(t => t.def == thingDef)) return false;
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!forced && MassUtility.WillBeOverEncumberedAfterPickingUp(pawn, t, Mathf.Min(10, t.stackCount))) return null;
            if (!GoodThingToHaul(t, pawn)) return null;

            Job job = new Job(RWBYDefOf.RWBY_PickUp, t);
            return job;
        }

        public static bool GoodThingToHaul(Thing t, Pawn pawn)
        {
            return t.Spawned && !t.IsForbidden(pawn) && pawn.CanReserveAndReach(t, PathEndMode.Touch, Danger.Some);
        }
    }
}
