using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;

namespace RWBYRemnant
{
    public class CompShadowClone : ThingComp
    {
        public CompProperties_ShadowClone Props => (CompProperties_ShadowClone)props;

        public Pawn shadowClone;

        public Rot4 shadowCloneAngle;

        public Color shadowCloneColor;

        public int ticksToLive;

        public bool useColor;

        public void SetData(Pawn pawn, int tmpTicksToLive, Color color)
        {
            shadowClone = pawn;
            shadowCloneAngle = pawn.Rotation;
            if (color != new Color())
            {
                useColor = true;
                shadowCloneColor = color;
            }
            ticksToLive = tmpTicksToLive;
        }

        public void MovePawnOut()
        {
            if (parent.Position.GetFirstPawn(parent.Map) is Pawn pawn && pawn.Position == parent.Position)
            {
                bool pawnMoved = false;
                if (pawn.Rotation == Rot4.North)
                {
                    IntVec3 intVec = pawn.Position + new IntVec3(0, 0, -1);
                    if (PawnCanOccupy(intVec, pawn))
                    {
                        pawn.Position = intVec;
                        pawn.Notify_Teleported(true, false);
                        pawnMoved = true;
                    }
                }
                else if (pawn.Rotation == Rot4.East)
                {
                    IntVec3 intVec = pawn.Position + new IntVec3(-1, 0, 0);
                    if (PawnCanOccupy(intVec, pawn))
                    {
                        pawn.Position = intVec;
                        pawn.Notify_Teleported(true, false);
                        pawnMoved = true;
                    }
                }
                else if (pawn.Rotation == Rot4.South)
                {
                    IntVec3 intVec = pawn.Position + new IntVec3(0, 0, 1);
                    if (PawnCanOccupy(intVec, pawn))
                    {
                        pawn.Position = intVec;
                        pawn.Notify_Teleported(true, false);
                        pawnMoved = true;
                    }
                }
                else if (pawn.Rotation == Rot4.West)
                {
                    IntVec3 intVec = pawn.Position + new IntVec3(1, 0, 0);
                    if (PawnCanOccupy(intVec, pawn))
                    {
                        pawn.Position = intVec;
                        pawn.Notify_Teleported(true, false);
                        pawnMoved = true;
                    }
                }

                if (pawnMoved) return;

                for (int i = 1; i < 5; i++)
                {
                    IntVec3 intVec;

                    intVec = pawn.Position + new IntVec3(0, 0, -i); // move south
                    if (PawnCanOccupy(intVec, pawn))
                    {
                        if (intVec == pawn.Position)
                        {
                            return;
                        }
                        pawn.Position = intVec;
                        pawn.Notify_Teleported(true, false);
                        pawnMoved = true;
                        break;
                    }
                    intVec = pawn.Position + new IntVec3(-i, 0, 0); // move west
                    if (PawnCanOccupy(intVec, pawn))
                    {
                        if (intVec == pawn.Position)
                        {
                            return;
                        }
                        pawn.Position = intVec;
                        pawn.Notify_Teleported(true, false);
                        pawnMoved = true;
                        break;
                    }
                    intVec = pawn.Position + new IntVec3(0, 0, i); // move east
                    if (PawnCanOccupy(intVec, pawn))
                    {
                        if (intVec == pawn.Position)
                        {
                            return;
                        }
                        pawn.Position = intVec;
                        pawn.Notify_Teleported(true, false);
                        pawnMoved = true;
                        break;
                    }
                    intVec = pawn.Position + new IntVec3(i, 0, 0); // move north
                    if (PawnCanOccupy(intVec, pawn))
                    {
                        if (intVec == pawn.Position)
                        {
                            return;
                        }
                        pawn.Position = intVec;
                        pawn.Notify_Teleported(true, false);
                        pawnMoved = true;
                        break;
                    }
                }

                if (!pawnMoved) Log.Warning("Couldn´t move pawn " + pawn.Name.ToStringFull + " out of shadow clone.");
            }
        }

        private bool PawnCanOccupy(IntVec3 c, Pawn pawn)
        {
            if (!c.Walkable(pawn.Map))
            {
                return false;
            }
            Building edifice = c.GetEdifice(pawn.Map);
            if (edifice != null)
            {
                Building_Door building_Door = edifice as Building_Door;
                if (building_Door != null && !building_Door.PawnCanOpen(pawn) && !building_Door.Open)
                {
                    return false;
                }
            }
            return true;
        }

        public override void CompTick()
        {
            if (shadowClone == null) parent.Destroy();
            if (useColor && ticksToLive % 5 == 0) FleckMaker.ThrowDustPuffThick(parent.DrawPos, shadowClone.Map, 2, shadowCloneColor);
            ticksToLive--;
            if (ticksToLive < 1 || parent.Map != shadowClone.Map)
            {
                parent.Destroy();
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (parent.Map != shadowClone.Map) return;
            shadowClone.Drawer.renderer.GetType().GetMethod("RenderPawnInternal", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool), typeof(bool) }, null).Invoke(shadowClone.Drawer.renderer, new object[] { parent.DrawPos, 0f, true, shadowCloneAngle, shadowCloneAngle, RotDrawMode.Fresh, false, false, false });
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref ticksToLive, "ticksAlive", 1, false);
            Scribe_Values.Look<bool>(ref useColor, "useColor", false, false);
            Scribe_Values.Look<Rot4>(ref shadowCloneAngle, "shadowCloneAngle", Rot4.South, false);
            Scribe_Values.Look<Color>(ref shadowCloneColor, "shadowCloneColor", new Color(), false);
            Scribe_References.Look<Pawn>(ref shadowClone, "shadowClone", false);
        }
    }
}
