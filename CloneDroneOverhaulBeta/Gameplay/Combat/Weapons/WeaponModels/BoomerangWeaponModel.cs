using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class BoomerangWeaponModel : OverhaulWeaponModel
    {
        private static readonly FireSpreadDefinition lvl1Fire = new FireSpreadDefinition()
        {
            DamageSourceType = DamageSourceType.Sword,
            FireType = FireType.Weapon1,
            ImpactVoxelCount = 2,
            MinSpreadProbability = 0.1f,
            SpreadProbabilityDropOff = 0.4f,
            StartSpreadProbability = 0.5f,
            WaitBetweenSpreads = 0.3f,
            WaitBeforeDestroy = 0.65f
        };

        private BladeCutArea m_BladeCutArea;
        private Transform m_FireParticles;

        private bool m_IsPlayingAttackAnimation;
        private float m_TimeStartedAttacking;
        private float m_TimeToAllowRegisteringTheThrow;

        private float m_TimeToEnableColliders;
        private bool m_HasDisabledCollidersForThisThrow;

        private bool m_IsThrown;
        private float m_TimeToAllowPickingUp;

        private Rigidbody m_RigidBody;
        private BoxCollider m_Collider;
        private BoxCollider m_EnvCollider;

        private Transform m_Parent;

        private bool m_CollidedWithEnvironment;
        private float m_TimeToAllowVFX = -1f;

        private bool m_AllowTargeting;
        private Character m_CharacterToTarget;
        private float m_TimeToRefreshTarget;

        private float m_Range;

        private Color m_CurrentColor;
        private Material m_OwnMaterial;

        private float m_ThrowStrength;

        public override void Initialize(FirstPersonMover newOwner)
        {
            m_Range = 30f;
            m_OwnMaterial = base.GetComponent<ModdedObject>().GetObject<Renderer>(2).material;
            if (base.MeleeImpactArea == null)
            {
                BladeCutArea blade = base.gameObject.AddComponent<BladeCutArea>();
                blade.EdgePoint1 = GetComponent<ModdedObject>().GetObject<Transform>(0);
                blade.EdgePoint1.gameObject.layer = Layers.BodyPart;
                blade.EdgePoint2 = GetComponent<ModdedObject>().GetObject<Transform>(1);
                blade.EdgePoint2.gameObject.layer = Layers.BodyPart;
                blade.DamageSourceType = DamageSourceType.Sword;
                m_BladeCutArea = blade;
                base.MeleeImpactArea = blade;
            }
            SetOwner(newOwner);
            if (m_RigidBody == null)
            {
                m_RigidBody = gameObject.GetComponent<Rigidbody>();
                m_RigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
            if (m_Collider == null)
            {
                BoxCollider[] c = gameObject.GetComponents<BoxCollider>();
                m_Collider = c[0];
                m_EnvCollider = c[1];
                base.MeleeImpactArea.SetPrivateField<Collider>("_collider", m_Collider);
                base.AddImmuneCharacter(GetOwner());
            }
            m_FireParticles = base.GetComponent<ModdedObject>().GetObject<Transform>(3);
        }

        public override void ApplyColor(Color color)
        {
            m_CurrentColor = color;
            m_OwnMaterial.SetColor("_EmissionColor", m_CurrentColor);
        }

        public override void OnUpgradesRefreshed(UpgradeCollection collection)
        {
            m_AllowTargeting = false;
            m_FireParticles.gameObject.SetActive(false);
            if (collection.HasUpgrade((UpgradeType)6700))
            {
                SetCanBeEquiped();
            }
            if (collection.HasUpgrade((UpgradeType)6701))
            {
                m_BladeCutArea.SetFireSpreadDefinition(lvl1Fire);
                m_FireParticles.gameObject.SetActive(true);
            }
            if (collection.HasUpgrade((UpgradeType)6702))
            {
                m_AllowTargeting = true;
            }
            if (collection.HasUpgrade((UpgradeType)6703))
            {
                switch (collection.GetUpgradeLevel((UpgradeType)6703))
                {
                    case 1:
                        m_Range = 40f;
                        break;
                    case 2:
                        m_Range = 55f;
                        break;
                    default:
                        m_Range = 30f;
                        break;
                }
            }
        }

        public override void TryAttack()
        {
            if(m_IsThrown || GetOwner().IsUsingMagBoots())
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
                DelegateScheduler.Instance.Schedule(Throw, 0.235f);
                return;
            }
            m_IsPlayingAttackAnimation = false;
            AllowRobotToSwitchWeapons = true;
            AnimationController.ForceSetIsPlayingUpperAnimation = false;
            AnimationController.StopPlayingCustomAnimations();
        }

        public void Throw()
        {
            m_HasDisabledCollidersForThisThrow = false;
            m_ThrowStrength = Mathf.Clamp((Time.time - m_TimeStartedAttacking) / (m_TimeToAllowRegisteringTheThrow - m_TimeStartedAttacking + 1f), 0.7f, 1.5f);
            m_Parent = base.transform.parent;
            m_RigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            m_IsThrown = true;
            m_TimeToAllowPickingUp = Time.time + 0.3f + (!m_AllowTargeting ? 0 : 3f);
            m_RigidBody.isKinematic = false;
            m_RigidBody.velocity = GetOwner().transform.forward * (m_Range * m_ThrowStrength);
            base.transform.SetParent(null, true);
            base.transform.eulerAngles = new Vector3(Random.value * 20, base.transform.eulerAngles.y, base.transform.eulerAngles.z);
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
            base.transform.localScale = ModelOffset.OffsetLocalScale;
            AllowRobotToSwitchWeapons = true;
        }

        private void checkPickUp()
        {
            if (Time.time > m_TimeToAllowPickingUp && Vector3.Distance(GetOwner().transform.position, base.transform.position) < 4)
            {
                PickUp();
            }
        }

        public override void OnUnequipped()
        {
            AnimationController.ForceSetIsPlayingUpperAnimation = false;
        }

        private void LateUpdate()
        {
            if(m_AllowTargeting && Time.time >= m_TimeToRefreshTarget)
            {
                m_TimeToRefreshTarget = Time.time + 1f;
                m_CharacterToTarget = CharacterTracker.Instance.GetClosestLivingEnemyCharacter(base.transform.position);
                m_RigidBody.velocity *= 0.2f;
            }
            if (m_AllowTargeting)
            {
                m_OwnMaterial.SetColor("_EmissionColor", m_CurrentColor * (Mathf.PingPong(Time.time, 0.75f) + 0.75f));
            }

            m_FireParticles.transform.eulerAngles = Vector3.zero;
            if (m_Collider != null) m_Collider.enabled = m_IsThrown;
            if (m_EnvCollider != null) m_EnvCollider.enabled = Time.time >= m_TimeToEnableColliders;
            if (m_IsThrown)
            {
                MeleeImpactArea.SetPrivateField<bool>("_isDamageActive", true);
                if (Time.time > m_TimeToAllowPickingUp)
                {
                    if (!m_CollidedWithEnvironment) m_RigidBody.velocity += GetVelocityForTransform(GetOwner().transform);
                    checkPickUp();
                }
                else if (m_AllowTargeting && m_CharacterToTarget != null)
                {
                    if (!m_CollidedWithEnvironment) m_RigidBody.velocity += GetVelocityForTransform(m_CharacterToTarget.transform) * 1.5f;
                    checkPickUp();
                }

                base.transform.eulerAngles -= new Vector3(0, 480, 0) * Time.deltaTime;

                if (Time.time > m_TimeToAllowPickingUp + 5f)
                {
                    PickUp();
                }
                //m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, m_RigidBody.velocity.y * 0.03f, m_RigidBody.velocity.z);
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

        private Vector3 GetVelocityForTransform(Transform transform)
        {
            return ((transform.position - base.transform.position + new Vector3(0, 1, 0)) * Time.deltaTime * (m_CollidedWithEnvironment ? 2f : 0.5f)).normalized;
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!m_HasDisabledCollidersForThisThrow && !collision.collider.CompareTag("Environment"))
            {
                m_TimeToEnableColliders = Time.time + 0.3f;
                m_HasDisabledCollidersForThisThrow = true;

                BaseBodyPart part = CacheManager.Instance.GetBaseBodyPart(collision.transform);
                if(part != null)
                {
                    _ = part.TryCutVolume(m_BladeCutArea.GetPrivateField<Vector3>("_lastEdgePosition1"), m_BladeCutArea.GetPrivateField<Vector3>("_lastEdgePosition2"),
                        m_BladeCutArea.EdgePoint1.transform.position, m_BladeCutArea.EdgePoint2.transform.position, -1, false, GetOwner(), DamageSourceType.Sword, null, true);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Environment"))
            {
                if(Time.time >= m_TimeToAllowVFX)
                {
                    AttackManager.Instance.CreateSwordBlockVFX(base.transform.position);
                    _ = AudioManager.Instance.PlayClipAtTransform(AudioLibrary.Instance.SwordBlocks, base.transform, 0f, false, 1f, GetOwner().WeaponEnvironmentImpactPitch, 0f);
                    m_TimeToAllowVFX = Time.time + 1f;
                }
                m_CollidedWithEnvironment = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            m_CollidedWithEnvironment = false;
        }
    }
}