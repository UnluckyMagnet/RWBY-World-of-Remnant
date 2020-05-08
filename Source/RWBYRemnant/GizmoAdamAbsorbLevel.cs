using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    [StaticConstructorOnStartup]
    class GizmoAdamAbsorbLevel : Gizmo
    {
        public GizmoAdamAbsorbLevel()
        {
            this.order = -99f;
        }
        
        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }
        
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            Rect overRect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(354613518 + aura.pawn.thingIDNumber, overRect, WindowLayer.GameUI, delegate
            {
                Rect rect = overRect.AtZero().ContractedBy(6f);
                Rect rect2 = rect;
                rect2.height = overRect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect2, this.label);
                Rect rect3 = rect;
                rect3.yMin = overRect.height / 2f;
                float fillPercent = this.currentAbsorbedDamage / 500f;
                Widgets.FillableBar(rect3, fillPercent, FullShieldBarTex, GizmoAdamAbsorbLevel.EmptyShieldBarTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect3, labelColor + (this.currentAbsorbedDamage).ToString("F0") + " / " + (500f).ToString("F0") + "</color>");
                Text.Anchor = TextAnchor.UpperLeft;
            }, true, false, 1f);
            return new GizmoResult(GizmoState.Clear);
        }

        public string label;

        public string labelColor;

        public Aura aura;

        public float currentAbsorbedDamage;
        
        public Texture2D FullShieldBarTex;
        
        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
    }
}
