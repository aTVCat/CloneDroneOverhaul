using UnityEngine;

namespace OverhaulMod.Engine
{
    public class WeaponInvisibilityFixer : MonoBehaviour
    {
        private FirstPersonMover _owner;

        private void Update()
        {
            if (!ModTime.Instance.HasFixedUpdatedThisFrame())
                return;

            FirstPersonMover firstPersonMover = _owner;
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
            _owner = firstPersonMover;
        }
    }
}
