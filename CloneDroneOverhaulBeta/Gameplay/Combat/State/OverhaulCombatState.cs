using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public static class OverhaulCombatState
    {
        public static event Action OnSwordBlockAreaCollidedWithEnvironment;
        private static SwordBlockArea m_SwordBlockAreaCollidedWithEnvironment;
        public static SwordBlockArea SwordBlockAreaCollidedWithEnvironment
        {
            get
            {
                return m_SwordBlockAreaCollidedWithEnvironment;
            }
            set
            {
                m_SwordBlockAreaCollidedWithEnvironment = value;
                if(OnSwordBlockAreaCollidedWithEnvironment != null) OnSwordBlockAreaCollidedWithEnvironment();
            }
        }

        private static WeaponSkinItemDefinitionV2 m_SwordBlockAreaEnvCollisionSkinItem;
        public static WeaponSkinItemDefinitionV2 SwordBlockAreaEnvCollisionSkinItem
        {
            get
            {
                return m_SwordBlockAreaEnvCollisionSkinItem;
            }
            set
            {
                m_SwordBlockAreaEnvCollisionSkinItem = value;
            }
        }

        private static Vector3 m_SwordBlockAreaEnvCollisionPosition;
        public static Vector3 SwordBlockAreaEnvCollisionPosition
        {
            get
            {
                return m_SwordBlockAreaEnvCollisionPosition;
            }
            set
            {
                m_SwordBlockAreaEnvCollisionPosition = value;
            }
        }
    }
}
