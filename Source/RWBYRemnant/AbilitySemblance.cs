using AbilityUser;
using System.Linq;
using Verse;

namespace RWBYRemnant
{
    class AbilitySemblance : PawnAbility
    {
        public AbilitySemblance()
        {
        }

        public AbilitySemblance(CompAbilityUser abilityUser) : base(abilityUser)
        {
            
            this.abilityUser = (abilityUser as CompAbilityUserAura);
        }

        public AbilitySemblance(AbilityData abilityData) : base(abilityData)
        {
            this.abilityUser = (abilityData.Pawn.AllComps.FirstOrDefault((ThingComp x) => x.GetType() == abilityData.AbilityClass) as CompAbilityUserAura);
        }

        public AbilitySemblance(Pawn user, SemblanceDef pdef) : base(user, pdef)
        {
        }

        public CompAbilityUserAura AbilityUserAura
        {
            get
            {
                CompAbilityUserAura result;
                if ((result = (Pawn.GetComp<CompAbilityUserAura>())) != null)
                {
                    return result;
                }
                return null;
            }
        }

        public SemblanceDef SemblanceDef
        {
            get
            {
                return Def as SemblanceDef;
            }
        }

        //public override void Notify_AbilityFailed(bool refund)
        //{
        //    base.Notify_AbilityFailed(refund);
        //    if (refund)
        //    {
        //        if (SemblanceDef.usesAmmunition != null && Pawn.inventory.GetDirectlyHeldThings().ToList().Find(s => s.def == SemblanceDef.usesAmmunition) == null)
        //        {
        //            Pawn.inventory.GetDirectlyHeldThings().TryAdd(ThingMaker.MakeThing(SemblanceDef.usesAmmunition), 1);
        //        }
        //        if (SemblanceDef.auraCost > 0f)
        //        {
        //            AbilityUserAura.aura.currentEnergy += SemblanceDef.auraCost;
        //        }
        //    }
        //}

        public override bool CanCastPowerCheck(AbilityContext context, out string reason)
        {
            bool baseResult = base.CanCastPowerCheck(context, out reason);
            if (baseResult)
            {
                if (SemblanceDef.usesAmmunition != null && Pawn.inventory.GetDirectlyHeldThings().ToList().Find(s => s.def == SemblanceDef.usesAmmunition) == null)
                {
                    baseResult = false;
                    reason = "DisabledNoDustPowderAmmunition".Translate(SemblanceDef.usesAmmunition.label).CapitalizeFirst();
                }
                if (SemblanceDef.auraCost > 0f && SemblanceDef.auraCost >= AbilityUserAura.aura.currentEnergy)
                {
                    baseResult = false;
                    reason = "DisabledNotEnoughAura".Translate(Pawn.Name.ToStringShort);
                }
            }
            return baseResult;
        }
    }
}
