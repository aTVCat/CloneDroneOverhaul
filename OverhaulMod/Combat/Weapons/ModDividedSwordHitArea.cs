using System.Collections.Generic;

namespace OverhaulMod.Combat.Weapons
{
    internal class ModDividedSwordHitArea : SwordHitArea
    {
        public List<BladeEdgePoints> Blades;

        public override bool tryDamageBodyPart(BaseBodyPart bodyPart)
        {
            bool flag = false;
            foreach (BladeEdgePoints bladeEdgePoints in Blades)
            {
                flag |= base.tryCutBodyPart(bodyPart, bladeEdgePoints.GetLastPosition1(), bladeEdgePoints.GetLastPosition2(), bladeEdgePoints.EdgePoint1.position, bladeEdgePoints.EdgePoint2.position);
            }
            return flag;
        }

        public override bool wouldAttackDamageBodyPart(BaseBodyPart bodyPart, Character attacker)
        {
            foreach (BladeEdgePoints bladeEdgePoints in Blades)
            {
                if (bodyPart.WouldCutVolume(bladeEdgePoints.GetLastPosition1(), bladeEdgePoints.GetLastPosition2(), bladeEdgePoints.EdgePoint1.position, bladeEdgePoints.EdgePoint2.position))
                {
                    return true;
                }
            }
            return false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            foreach (BladeEdgePoints bladeEdgePoints in Blades)
            {
                bladeEdgePoints.CaptureLastPositions();
            }
        }
    }
}