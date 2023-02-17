using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class RobotAccessoryBehaviour : MonoBehaviour
    {
        public FirstPersonMover Owner;

        public SerializeTransform TargetTransform;

        public RobotAccessoryItemDefinition Item;

        private void Awake()
        {
            Item = RobotAccessoriesController.GetAccessoryByPrefabName(base.gameObject.name);
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
            if (other == null) return;

            MeleeImpactArea a = other.GetComponent<MeleeImpactArea>();
            LavaFloor fl = other.GetComponent<LavaFloor>();
            if (fl != null || (a && a.IsDamageActive() && (Owner == null || a.Owner != Owner)))
            {
                if (Owner == null) return;
                RobotAccessoriesWearer w = FirstPersonMoverExtention.GetExtention<RobotAccessoriesWearer>(Owner);
                if (w == null) return;

                _ = PooledPrefabController.SpawnObject<RobotAccessoryDestroy_VFX>(RobotAccessoriesController.AccessoryDestroyVFX_ID, base.transform.position, base.transform.eulerAngles);
                _ = AudioManager.Instance.PlayClipAtTransform(RobotAccessoriesController.AccessoryDestroyedSound, base.transform, 0f, false, 2f, 0.8f, 0.1f);

                w.UnregisterAccessory(base.gameObject);
                Destroy(base.gameObject);
            }
        }
    }
}