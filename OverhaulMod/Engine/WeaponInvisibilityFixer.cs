using UnityEngine;

namespace OverhaulMod.Engine
{
    public class WeaponInvisibilityFixer : MonoBehaviour
    {
        private FirstPersonMover m_owner;

        private void Update()
        {
            if (!ModTime.Instance.HasFixedUpdated())
                return;

            FirstPersonMover firstPersonMover = m_owner;
            if (!firstPersonMover || !firstPersonMover._characterModel || firstPersonMover.IsMainPlayer() || firstPersonMover.IsDetached())
                return;

            WeaponModel weaponModel = firstPersonMover._characterModel.GetWeaponModel(firstPersonMover.GetEquippedWeaponType());
            if (weaponModel)
            {
                weaponModel.gameObject.SetActive(true);
            }
        }

        public void Initialize(FirstPersonMover firstPersonMover)
        {
            m_owner = firstPersonMover;
        }
    }
}
