using OverhaulAPI;
using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class OverhaulWeaponModel : AddedWeaponModel
    {
        private CharacterModdedAnimationsExpansion m_CharacterModdedAnimationsExpansion;
        public CharacterModdedAnimationsExpansion AnimationController
        {
            get
            {
                FirstPersonMover mover = GetOwner();
                if(mover == null)
                {
                    return null;
                }

                if(m_CharacterModdedAnimationsExpansion == null)
                {
                    m_CharacterModdedAnimationsExpansion = mover.GetComponent<CharacterModdedAnimationsExpansion>();
                }
                return m_CharacterModdedAnimationsExpansion;
            }
        }

        private NewWeaponsRobotExpansion m_NewWeaponsRobotExpansion;
        public NewWeaponsRobotExpansion NewWeaponsController
        {
            get
            {
                FirstPersonMover mover = GetOwner();
                if (mover == null)
                {
                    return null;
                }

                if (m_NewWeaponsRobotExpansion == null)
                {
                    m_NewWeaponsRobotExpansion = mover.GetComponent<NewWeaponsRobotExpansion>();
                }
                return m_NewWeaponsRobotExpansion;
            }
        }

        public bool AllowChangingWeapon { get; set; }

        public virtual void Start()
        {
            AllowChangingWeapon = true;
        }

        public virtual void TryAttack()
        {

        }

        public virtual void OnEquipped()
        {

        }

        public virtual void OnUnequipped()
        {

        }
    }
}