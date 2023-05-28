using System;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public static class OverhaulCombatState
    {
        public static event Action OnSwordBlockAreaCollidedWithEnvironment;
        private static SwordBlockArea m_SwordBlockAreaCollidedWithEnvironment;
        public static SwordBlockArea SwordBlockAreaCollidedWithEnvironment
        {
            get => m_SwordBlockAreaCollidedWithEnvironment;
            set
            {
                m_SwordBlockAreaCollidedWithEnvironment = value;
                OnSwordBlockAreaCollidedWithEnvironment?.Invoke();
            }
        }

        public static WeaponSkinItemDefinitionV2 SwordBlockAreaEnvCollisionSkinItem { get; set; }

        private static Vector3 m_SwordBlockAreaEnvCollisionPosition;
        public static Vector3 SwordBlockAreaEnvCollisionPosition
        {
            get => m_SwordBlockAreaEnvCollisionPosition;
            set => m_SwordBlockAreaEnvCollisionPosition = value;
        }

        public static Color GetUIThemeColor(Color defaultColor)
        {
            if (CharacterTracker.Instance == null) return defaultColor;
            FirstPersonMover m = CharacterTracker.Instance.GetPlayerRobot();
            return m == null || !m || !m.HasCharacterModel() ? defaultColor : m.GetCharacterModel().GetFavouriteColor();
        }

        public static Color GetUIThemeColor(string defaultColorHEX)
        {
            return GetUIThemeColor(defaultColorHEX.ConvertHexToColor());
        }
    }
}
