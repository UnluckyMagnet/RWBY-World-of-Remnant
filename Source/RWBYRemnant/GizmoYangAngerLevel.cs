using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    [StaticConstructorOnStartup]
    class GizmoYangAngerLevel : Gizmo
    {
        public GizmoYangAngerLevel()
        {
            this.Order = -99f; // fix variable name is changed
        }

        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        // add method argument
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect overRect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(354613517 + aura.pawn.thingIDNumber, overRect, WindowLayer.GameUI, delegate
            {
                Rect rect = overRect.AtZero().ContractedBy(6f);
                Rect rect2 = rect;
                rect2.height = overRect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect2, this.label);
                Rect rect3 = rect;
                rect3.yMin = overRect.height / 2f;
                float fillPercent = this.currentAbsorbedDamage / 100f;
                Widgets.FillableBar(rect3, fillPercent, FullShieldBarTex, GizmoYangAngerLevel.EmptyShieldBarTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect3, labelColor + (this.currentAbsorbedDamage).ToString("F0") + " / " + (100f).ToString("F0") + "</color>");
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
