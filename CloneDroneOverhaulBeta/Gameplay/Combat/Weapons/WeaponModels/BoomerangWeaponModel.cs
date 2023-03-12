using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class BoomerangWeaponModel : OverhaulWeaponModel
    {
        public static readonly FireSpreadDefinition Level1FireSpreadParameters = new FireSpreadDefinition()
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

        public static float WaitBetweenVelocityCalculations => Time.fixedDeltaTime;

        private BladeCutArea m_BladeCutArena;
        private Transform m_FireVFXTransform;

        private bool m_IsPreparingToThrow;
        private float m_TimeStartedPreparing;
        private float m_TimeToAllowThrowing;
        private float m_ThrowStrength;

        private float m_TimeToAllowCalculatingEnvironmentCollisions;
        private bool m_HasAlreadyDisabledCollidersThisThrow;

        private bool m_IsThrown;
        private float m_TimeToAllowPickingUp;

        private Rigidbody m_RigidBody;
        private BoxCollider m_Collider;
        private BoxCollider m_EnvironmentCollider;

        private Transform m_InitialParentTransform;

        private bool m_HasCollidedWithEnvironment;
        private float m_TimeToAllowVFX = -1f;

        private bool m_AllowAutoTargeting;
        private Character m_TargetCharacter;
        private float m_TimeToRefreshTarget;

        private float m_ThrowRange;

        private Color m_MainColor;
        private Material m_Material;

        private float m_TimeToCalculateNewVelocityValue;

        public override void Initialize(FirstPersonMover newOwner)
        {
            m_ThrowRange = 30f;
            m_Material = base.GetComponent<ModdedObject>().GetObject<Renderer>(2).material;

            BladeCutArea blade = base.gameObject.AddComponent<BladeCutArea>();
            blade.EdgePoint1 = GetComponent<ModdedObject>().GetObject<Transform>(0);
            blade.EdgePoint1.gameObject.layer = Layers.BodyPart;
            blade.EdgePoint2 = GetComponent<ModdedObject>().GetObject<Transform>(1);
            blade.EdgePoint2.gameObject.layer = Layers.BodyPart;
            blade.DamageSourceType = DamageSourceType.Sword;
            m_BladeCutArena = blade;
            base.MeleeImpactArea = blade;

            SetOwner(newOwner);

            m_RigidBody = gameObject.GetComponent<Rigidbody>();
            m_RigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            BoxCollider[] c = gameObject.GetComponents<BoxCollider>();
            m_Collider = c[0];
            m_EnvironmentCollider = c[1];
            base.MeleeImpactArea.SetPrivateField<Collider>("_collider", m_Collider);
            base.AddImmuneCharacter(GetOwner());

            m_FireVFXTransform = base.GetComponent<ModdedObject>().GetObject<Transform>(3);
        }

        public override void ApplyOwnersFavouriteColor(Color color)
        {
            m_MainColor = color;
            m_Material.SetColor("_EmissionColor", m_MainColor);
        }

        public override void OnUpgradesRefreshed(UpgradeCollection collection)
        {
            m_AllowAutoTargeting = false;
            m_FireVFXTransform.gameObject.SetActive(false);

            if (collection.HasUpgrade((UpgradeType)6700))
            {
                SetCanBeEquiped();
            }

            if (collection.HasUpgrade((UpgradeType)6701))
            {
                m_BladeCutArena.SetFireSpreadDefinition(Level1FireSpreadParameters);
                m_FireVFXTransform.gameObject.SetActive(true);
            }

            if (collection.HasUpgrade((UpgradeType)6702))
            {
                m_AllowAutoTargeting = true;
            }

            if (collection.HasUpgrade((UpgradeType)6703))
            {
                switch (collection.GetUpgradeLevel((UpgradeType)6703))
                {
                    case 1:
                        m_ThrowRange = 40f;
                        break;
                    case 2:
                        m_ThrowRange = 55f;
                        break;
                    default:
                        m_ThrowRange = 30f;
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

            if (AnimationController != null &&
                (!AnimationController.IsPlayingCustomUpperAnimation ||
                  AnimationController.GetPlayingCustomAnimationName(CombatOverhaulAnimatorType.Upper).Equals("WeaponUse_PickUpBoomerang")))
            {
                m_TimeStartedPreparing = Time.time;
                m_TimeToAllowThrowing = Time.time + 0.2f;
                m_IsPreparingToThrow = true;

                AnimationController.ForceSetIsPlayingUpperAnimation = true;
                AnimationController.PlayCustomAnimaton("WeaponUse_PrepareBoomerang");
            }
        }

        public override void OnUnequipped()
        {
            AnimationController.ForceSetIsPlayingUpperAnimation = false;
            PickUp();
        }

        public void StopPreparing()
        {
            m_IsPreparingToThrow = false;
            AnimationController.ForceSetIsPlayingUpperAnimation = false;

            if (Time.time >= m_TimeToAllowThrowing)
            {
                AnimationController.PlayCustomAnimaton("WeaponUse_ThrowBoomerang");
                DelegateScheduler.Instance.Schedule(Throw, 0.235f);
                return;
            }

            AnimationController.StopPlayingCustomAnimations();
        }

        public void Throw()
        {
            m_InitialParentTransform = base.transform.parent;

            m_ThrowStrength = Mathf.Clamp((Time.time - m_TimeStartedPreparing) / (m_TimeToAllowThrowing - m_TimeStartedPreparing + 1f), 0.7f, 1.5f);
            m_TimeToAllowPickingUp = Time.time + 0.3f + (!m_AllowAutoTargeting ? 0 : 3f);
            m_HasAlreadyDisabledCollidersThisThrow = false;

            m_RigidBody.isKinematic = false;
            m_RigidBody.velocity = GetOwner().transform.forward * (m_ThrowRange * m_ThrowStrength);
            m_RigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

            m_Collider.enabled = true;

            base.transform.SetParent(null, true);
            base.transform.eulerAngles = new Vector3(Random.value * 20, base.transform.eulerAngles.y, base.transform.eulerAngles.z);
            base.transform.position = GetOwner().transform.position + (GetOwner().transform.forward * 1.3f) + new Vector3(0, 1.6f + (Random.value * 0.5f), 0);
            base.MeleeImpactArea.SetDamageActive(true);

            m_IsThrown = true;
        }

        public void PickUp()
        {
            base.MeleeImpactArea.SetDamageActive(false);
            m_RigidBody.isKinematic = true;

            base.transform.SetParent(m_InitialParentTransform, false);
            base.transform.localPosition = ModelOffset.OffsetPosition;
            base.transform.localEulerAngles = ModelOffset.OffsetEulerAngles;
            base.transform.localScale = ModelOffset.OffsetLocalScale;

            if(GetOwner().GetEquippedWeaponType() == base.WeaponType) AnimationController.PlayCustomAnimaton("WeaponUse_PickUpBoomerang");

            m_IsThrown = false;
        }

        public Vector3 GetVelocityForTransform(Transform transform)
        {
            float time = Time.time;
            if (time < m_TimeToCalculateNewVelocityValue)
            {
                return Vector3.zero;
            }
            m_TimeToCalculateNewVelocityValue = time + WaitBetweenVelocityCalculations;

            return ((transform.position - base.transform.position + new Vector3(0, 1, 0)) * (m_HasCollidedWithEnvironment ? 2f : 0.5f)).normalized;
        }

        private void LateUpdate()
        {
            float time = Time.time;

            if (m_AllowAutoTargeting)
            {
                if(time >= m_TimeToRefreshTarget)
                {
                    m_TimeToRefreshTarget = time + 1f;
                    m_TargetCharacter = CharacterTracker.Instance.GetClosestLivingEnemyCharacter(base.transform.position);
                    m_RigidBody.velocity *= 0.2f;
                }
                m_Material.SetColor("_EmissionColor", m_MainColor * (Mathf.PingPong(time, 0.75f) + 0.75f));
            }

            m_FireVFXTransform.transform.eulerAngles = Vector3.zero;
            m_EnvironmentCollider.enabled = time >= m_TimeToAllowCalculatingEnvironmentCollisions;

            if (m_IsThrown)
            {
                MeleeImpactArea.SetPrivateField("_isDamageActive", true);
                if (time > m_TimeToAllowPickingUp)
                {
                    if (!m_HasCollidedWithEnvironment) m_RigidBody.velocity += GetVelocityForTransform(GetOwner().transform);
                }
                else if (m_AllowAutoTargeting && m_TargetCharacter != null)
                {
                    if (!m_HasCollidedWithEnvironment) m_RigidBody.velocity += GetVelocityForTransform(m_TargetCharacter.transform) * 1.5f;
                }

                base.transform.eulerAngles -= new Vector3(0, 480, 0) * Time.deltaTime;
                tryPickUp();
            }

            if (!m_IsPreparingToThrow)
            {
                return;
            }

            bool isIncreasingDistance = Input.GetMouseButton(0);
            if (!isIncreasingDistance)
            {
                StopPreparing();
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!m_IsThrown)
            {
                return;
            }

            if (!m_HasAlreadyDisabledCollidersThisThrow && !collision.collider.CompareTag("Environment"))
            {
                m_TimeToAllowCalculatingEnvironmentCollisions = Time.time + 0.3f;
                m_HasAlreadyDisabledCollidersThisThrow = true;

                BaseBodyPart part = CacheManager.Instance.GetBaseBodyPart(collision.transform);
                if(part != null)
                {
                    _ = part.TryCutVolume(m_BladeCutArena.GetPrivateField<Vector3>("_lastEdgePosition1"), m_BladeCutArena.GetPrivateField<Vector3>("_lastEdgePosition2"),
                        m_BladeCutArena.EdgePoint1.transform.position, m_BladeCutArena.EdgePoint2.transform.position, -1, false, GetOwner(), DamageSourceType.Sword, null, true);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!m_IsThrown)
            {
                return;
            }

            if (collision.collider.CompareTag("Environment"))
            {
                if(Time.time >= m_TimeToAllowVFX)
                {
                    AttackManager.Instance.CreateSwordBlockVFX(base.transform.position);
                    _ = AudioManager.Instance.PlayClipAtTransform(AudioLibrary.Instance.SwordBlocks, base.transform, 0f, false, 1f, GetOwner().WeaponEnvironmentImpactPitch, 0f);
                    m_TimeToAllowVFX = Time.time + 1f;
                }
                m_HasCollidedWithEnvironment = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!m_IsThrown)
            {
                return;
            }

            m_HasCollidedWithEnvironment = false;
        }

        private void tryPickUp()
        {
            if (!m_IsThrown)
            {
                return;
            }

            float time = Time.time;

            if (time >= m_TimeToAllowPickingUp + 5 || (time > m_TimeToAllowPickingUp && Vector3.Distance(GetOwner().transform.position, base.transform.position) < 4))
            {
                PickUp();
            }
        }
    }
}