using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    /// <summary>
    /// The accessory gameobject controller
    /// </summary>
    public class RobotAccessoryBehaviour : OverhaulMonoBehaviour
    {
        private FirstPersonMover m_Owner;
        private IPlayerAccessoryItemDefinition m_Item;

        protected override void OnDisposed()
        {
            m_Owner = null;
        }

        public void Initialize(FirstPersonMover mover, IPlayerAccessoryItemDefinition item)
        {
            m_Item = item;
            m_Owner = mover;
        }

        private void OnCollisionEnter(Collision collision)
        {
            TryDestroyAccessory(collision.collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            TryDestroyAccessory(other);
        }

        public void TryDestroyAccessory(Collider other)
        {
            if (IsDisposedOrDestroyed() || other == null)
            {
                return;
            }

            MeleeImpactArea a = other.GetComponent<MeleeImpactArea>();
            LavaFloor fl = other.GetComponent<LavaFloor>();
            if (fl != null || (a && a.IsDamageActive() && (m_Owner == null || a.Owner != m_Owner)))
            {
                _ = PooledPrefabController.SpawnObject<RobotAccessoryDestroyVFX>(PlayerOutfitController.AccessoryDestroyVFX_ID, base.transform.position, base.transform.eulerAngles);
                _ = AudioManager.Instance.PlayClipAtTransform(PlayerOutfitController.AccessoryDestroyedSound, base.transform, 0f, false, 2f, 0.75f, 0.13f);

                if(m_Owner != null)
                {
                    RobotOutfitWearerExpansion exp = m_Owner.GetComponent<RobotOutfitWearerExpansion>();
                    if(exp != null)
                    {
                        exp.RemoveAccessory(m_Item);
                    }
                }

                DestroyGameObject();
            }
        }
    }
}