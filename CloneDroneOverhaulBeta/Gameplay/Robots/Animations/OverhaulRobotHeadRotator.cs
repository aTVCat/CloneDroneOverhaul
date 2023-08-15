using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class OverhaulRobotHeadRotator : OverhaulCharacterExpansion
    {
        private Transform m_HeadTransform;

        public override void Start()
        {
            base.Start();
            m_HeadTransform = Owner.GetBodyPartParent("Head");
        }

        private void LateUpdate()
        {
            if (!CanChangeHeadRotation())
                return;

            Vector3 vector = m_HeadTransform.eulerAngles;
            vector.x = GetCameraEulerAngles().x - 6f;
            m_HeadTransform.eulerAngles = vector;
        }

        public bool CanChangeHeadRotation()
        {
            return m_HeadTransform && Owner && Owner.IsGrabbedByGarbageBot();
        }

        public Vector3 GetCameraEulerAngles()
        {
            if (!Owner)
                return Vector3.zero;

            PlayerCameraMover cameraMover = Owner.GetCameraMover();
            return !cameraMover ? Vector3.zero : cameraMover.transform.eulerAngles;
        }
    }
}
