using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace RWBYRemnant
{
    public class Aura_Ren : Aura
    {
        public override Color GetColor()
        {
            return new Color(1.0f, 0.6f, 1.0f);
        }
    }
}
