using Verse;

namespace RWBYRemnant
{
    public class CompProperties_StealAura : CompProperties
    {
        public string AbilityLabelSteal;
        public string AbilityDescSteal;
        public string AbilityIconSteal;
        public string AbilityLabelConsume;
        public string AbilityIconConsume;

        public CompProperties_StealAura()
        {
            compClass = typeof(CompStealAura);
        }
    }
}
