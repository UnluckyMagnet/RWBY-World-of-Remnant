using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RWBYRemnant
{
    public class Command_TargetWithRadius : Command_Target
    {
        public override void GizmoUpdateOnMouseover()
        {
            verb.verbProps.DrawRadiusRing(verb.caster.Position);
        }

        public override void ProcessInput(Event ev)
        {
            if (CurActivateSound != null)
            {
                CurActivateSound.PlayOneShotOnCamera(null);
            }
            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
            Targeter targeter = Find.Targeter;
            Find.Targeter.BeginTargeting(verb, null);
        }

        public float range;

        public IntVec3 center;

        public Verb_ShootWeaponAbility verb;
    }
}