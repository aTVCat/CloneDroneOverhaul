using UnityEngine;

namespace OverhaulMod.Combat
{
    public class CharacterInventory : MonoBehaviour
    {
        public int LastServerFrameDoubleJumped;

        private FirstPersonMover m_owner;
        public FirstPersonMover owner
        {
            get
            {
                if (!m_owner)
                {
                    m_owner = base.GetComponent<FirstPersonMover>();
                }
                return m_owner;
            }
        }

        public bool hasDoubleJumpAbility
        {
            get;
            private set;
        }

        private void Start()
        {
            OnUpgradesRefreshed(owner._upgradeCollection);
        }

        private void Update()
        {
            FirstPersonMover firstPersonMover = owner;
            if (!firstPersonMover || !firstPersonMover.IsMainPlayer())
                return;

            float scroll = Input.mouseScrollDelta.y;
            if(scroll != 0f)
            {
                if(scroll > 0f)
                {
                    // todo: scroll to switch weapons
                }
                else
                {

                }
            }
        }

        public void OnUpgradesRefreshed(UpgradeCollection upgrades)
        {
            hasDoubleJumpAbility = upgrades.HasUpgrade(ModUpgradesManager.DOUBLE_JUMP_UPGRADE);
        }
    }
}
