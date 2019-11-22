using Verse;

namespace RWBYRemnant
{
    public class Command_TargetWithRadius : Command_Target
    {
        public override void GizmoUpdateOnMouseover()
        {
            if (Find.CurrentMap == null)
            {
                return;
            }

            if (range < (float)(Find.CurrentMap.Size.x + Find.CurrentMap.Size.z) && range < GenRadial.MaxRadialPatternRadius)
            {
                GenDraw.DrawRadiusRing(center, range);
            }

        }

        public float range;

        public IntVec3 center;
    }
}
