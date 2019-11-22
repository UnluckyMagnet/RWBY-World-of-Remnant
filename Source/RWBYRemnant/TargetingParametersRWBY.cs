using RimWorld;

namespace RWBYRemnant
{
    public class TargetingParametersRWBY : TargetingParameters
    {
        public static TargetingParameters OnGround()
        {
            return new TargetingParameters
            {
                canTargetLocations = true,
                canTargetPawns = true,
                canTargetBuildings = true,
                canTargetItems = true,
                mapObjectTargetsMustBeAutoAttackable = false
            };
        }
    }
}
