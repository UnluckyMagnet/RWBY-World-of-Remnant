using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RWBYRemnant
{
    public static class GrimmUtility
    {
        public static readonly List<PawnKindDef> grimmList = new List<PawnKindDef>
        {
            RWBYDefOf.Grimm_Boarbatusk,
            RWBYDefOf.Grimm_Beowolf,
            RWBYDefOf.Grimm_Ursa,
            RWBYDefOf.Grimm_Griffon,
            RWBYDefOf.Grimm_Nevermore,
            RWBYDefOf.Grimm_Lancer,
            RWBYDefOf.Grimm_LancerQueen,
            RWBYDefOf.Grimm_DeathStalker,
            RWBYDefOf.Grimm_Nuckelavee,
            RWBYDefOf.Grimm_Apathy
        };

        public static bool IsGrimm(Pawn pawn)
        {
            if (pawn.RaceProps.FleshType == RWBYDefOf.Grimm) return true;
            return false;
        }

        public static PawnKindDef GetRandomGrimmBalanced()
        {
            List<PawnKindDef> pawnKindDefs = new List<PawnKindDef>
            {
                 RWBYDefOf.Grimm_Boarbatusk,
                 RWBYDefOf.Grimm_Boarbatusk,
                 RWBYDefOf.Grimm_Boarbatusk,
                 RWBYDefOf.Grimm_Beowolf,
                 RWBYDefOf.Grimm_Beowolf,
                 RWBYDefOf.Grimm_Beowolf,
                 RWBYDefOf.Grimm_Ursa,
                 RWBYDefOf.Grimm_Ursa,
                 RWBYDefOf.Grimm_Ursa,
                 RWBYDefOf.Grimm_Griffon,
                 RWBYDefOf.Grimm_Griffon,
                 RWBYDefOf.Grimm_Griffon,
                 RWBYDefOf.Grimm_Nevermore,
                 RWBYDefOf.Grimm_Lancer,
                 RWBYDefOf.Grimm_Lancer,
                 RWBYDefOf.Grimm_Lancer,
                 RWBYDefOf.Grimm_LancerQueen,
                 RWBYDefOf.Grimm_DeathStalker
            };
            return pawnKindDefs.RandomElement();
        }
    }
}
