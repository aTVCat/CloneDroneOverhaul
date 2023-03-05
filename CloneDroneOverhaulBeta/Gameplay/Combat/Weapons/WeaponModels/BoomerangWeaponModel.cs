using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class BoomerangWeaponModel : OverhaulWeaponModel
    {
        private bool m_IsPlayingAttackAnimation;
        private float m_TimeStartedAttacking;
        private float m_TimeToAllowRegisteringTheThrow;

        private bool m_IsThrown;
        private float m_TimeToAllowPickingUp;

        private Rigidbody m_RigidBody;
        private BoxCollider m_Collider;

        private Transform m_Parent;
        private Transform m_TransformToUse;

        private float m_ThrowStrength;
  

        public override void Start()
        {
            if (base.MeleeImpactArea == null)
            {
                BladeCutArea blade = base.gameObject.AddComponent<BladeCutArea>();
                blade.EdgePoint1 = GetComponent<ModdedObject>().GetObject<Transform>(0);
                blade.EdgePoint1.gameObject.layer = Layers.BodyPart;
                blade.EdgePoint2 = GetComponent<ModdedObject>().GetObject<Transform>(1);
                blade.EdgePoint2.gameObject.layer = Layers.BodyPart;
                blade.DamageSourceType = DamageSourceType.Sword;
                base.MeleeImpactArea = blade;
            }
            if (m_RigidBody == null)
            {
                m_RigidBody = gameObject.GetComponent<Rigidbody>();
            }
            if (m_Collider == null)
            {
                m_Collider = gameObject.GetComponent<BoxCollider>();
                base.MeleeImpactArea.SetPrivateField<Collider>("_collider", m_Collider);
                base.AddImmuneCharacter(GetOwner());
            }
            base.Start();
            m_TransformToUse = GetOwner().GetBodyPart(MechBodyPartType.Head).transform.parent;
        }

        public override void TryAttack()
        {
            if(m_IsThrown)
            {
                return;
            }

            if (AnimationController != null && (!AnimationController.IsPlayingCustomUpperAnimation || AnimationController.GetPlayingCustomAnimationName(CombatOverhaulAnimatorType.Upper).Equals("WeaponUse_PickUpBoomerang")))
            {
                m_TimeStartedAttacking = Time.time;
                m_TimeToAllowRegisteringTheThrow = Time.time + 0.2f;
                m_IsPlayingAttackAnimation = true;
                AllowRobotToSwitchWeapons = false;
                AnimationController.ForceSetIsPlayingUpperAnimation = true;
                AnimationController.PlayCustomAnimaton("WeaponUse_PrepareBoomerang");
            }
        }

        public void StopIncreasingStrength()
        {
            if (Time.time >= m_TimeToAllowRegisteringTheThrow)
            {
                m_IsPlayingAttackAnimation = false;
                AnimationController.ForceSetIsPlayingUpperAnimation = false;
                AnimationController.PlayCustomAnimaton("WeaponUse_ThrowBoomerang");
                DelegateScheduler.Instance.Schedule(Throw, 0.2f);
                return;
            }
            m_IsPlayingAttackAnimation = false;
            AllowRobotToSwitchWeapons = true;
            AnimationController.ForceSetIsPlayingUpperAnimation = false;
            AnimationController.StopPlayingCustomAnimations();
        }

        public void Throw()
        {
            m_ThrowStrength = Mathf.Clamp((Time.time - m_TimeStartedAttacking) / (m_TimeToAllowRegisteringTheThrow - m_TimeStartedAttacking + 1f), 0.45f, 1.5f);
            m_Parent = base.transform.parent;
            m_RigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            m_IsThrown = true;
            m_TimeToAllowPickingUp = Time.time + 0.3f;
            m_RigidBody.isKinematic = false;
            m_RigidBody.velocity = GetOwner().transform.forward * (40 * m_ThrowStrength);
            base.transform.SetParent(null, true);
            base.transform.eulerAngles = Vector3.zero;
            base.transform.position = GetOwner().transform.position + (GetOwner().transform.forward * 1.3f) + new Vector3(0, 1.6f + (Random.value * 0.5f), 0);
            base.MeleeImpactArea.SetDamageActive(true);
        }

        public void PickUp()
        {
            AnimationController.PlayCustomAnimaton("WeaponUse_PickUpBoomerang");
            base.MeleeImpactArea.SetDamageActive(false);
            m_IsThrown = false;
            m_RigidBody.isKinematic = true;
            base.transform.SetParent(m_Parent, false);
            base.transform.localPosition = ModelOffset.OffsetPosition;
            base.transform.localEulerAngles = ModelOffset.OffsetEulerAngles;
            AllowRobotToSwitchWeapons = true;
        }

        public override void OnUnequipped()
        {
            AnimationController.ForceSetIsPlayingUpperAnimation = false;
        }

        private void Update()
        {
            if(m_Collider != null) m_Collider.enabled = m_IsThrown;
            if (m_IsThrown)
            {
                MeleeImpactArea.SetPrivateField<bool>("_isDamageActive", true);
                if (Time.time > m_TimeToAllowPickingUp)
                {
                    m_RigidBody.velocity += ((GetOwner().transform.position - base.transform.position + new Vector3(0, 1, 0)) * Time.deltaTime * 0.5f).normalized;

                    if (Vector3.Distance(GetOwner().transform.position, base.transform.position) < 4)
                    {
                        PickUp();
                    }
                }
                base.transform.eulerAngles += new Vector3(0, 360, 0) * Time.deltaTime;

                if (Time.time > m_TimeToAllowPickingUp + 5f)
                {
                    PickUp();
                }
                m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, m_RigidBody.velocity.y * 0.01f, m_RigidBody.velocity.z);
            }

            if (!m_IsPlayingAttackAnimation)
            {
                return;
            }

            bool isIncreasingDistance = Input.GetMouseButton(0);
            if (!isIncreasingDistance)
            {
                StopIncreasingStrength();
            }
        }
    }
}