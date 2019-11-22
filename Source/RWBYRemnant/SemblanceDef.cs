using AbilityUser;
using System.Text;
using Verse;

namespace RWBYRemnant
{
    public class SemblanceDef : AbilityDef
    {
        public ThingDef usesAmmunition = null;

        public float auraCost = 0f;

        public override string GetDescription()
        {
            string basics = this.GetBasics();
            string aoEDesc = this.GetAoEDesc();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(this.description);
            bool flag = basics != "";
            if (flag)
            {
                stringBuilder.AppendLine(basics);
            }
            bool flag2 = aoEDesc != "";
            if (flag2)
            {
                stringBuilder.AppendLine(aoEDesc);
            }
            bool flag3 = auraCost != 0;
            if (flag3)
            {
                stringBuilder.AppendLine("Aura Cost: " + (auraCost * 100f).ToString());
            }
            bool abilitySucceeded = usesAmmunition != null;
            if (abilitySucceeded)
            {
                stringBuilder.AppendLine("Uses Ammunition: " + usesAmmunition.label.CapitalizeFirst());
            }
            return stringBuilder.ToString();
        }
    }
}
