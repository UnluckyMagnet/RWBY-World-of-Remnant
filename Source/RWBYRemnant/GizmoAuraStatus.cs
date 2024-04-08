using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    [StaticConstructorOnStartup]
    class GizmoAuraStatus : Gizmo
    {
        public GizmoAuraStatus()
        {
            this.Order = -100f; // fix variable name is changed
        }
        
        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        // add method argument
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect overRect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(354613516 + aura.pawn.thingIDNumber, overRect, WindowLayer.GameUI, delegate
            {
                Rect rect = overRect.AtZero().ContractedBy(6f);
                Rect rect2 = rect;
                rect2.height = overRect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect2, this.label);
                Rect rect3 = rect;
                rect3.yMin = overRect.height / 2f;
                float fillPercent = this.aura.currentEnergy / this.aura.maxEnergy;
                Widgets.FillableBar(rect3, fillPercent, FullShieldBarTex, GizmoAuraStatus.EmptyShieldBarTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect3, aura.GetLabelColor() + (this.aura.currentEnergy * 100f).ToString("F0") + " / " + (this.aura.maxEnergy * 100f).ToString("F0") + "</color>");
                Text.Anchor = TextAnchor.UpperLeft;
            }, true, false, 1f);
            return new GizmoResult(GizmoState.Clear);
        }
        
        public Aura aura;

        public string label;
        
        public Texture2D FullShieldBarTex;
        
        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
    }
}
