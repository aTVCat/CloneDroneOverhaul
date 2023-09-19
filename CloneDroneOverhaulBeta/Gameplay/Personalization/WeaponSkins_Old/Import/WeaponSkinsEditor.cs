using OverhaulAPI;
using System.Collections.Generic;

namespace CDOverhaul.Gameplay
{
    public static class WeaponSkinsEditor
    {
        private static readonly List<IWeaponSkinItemDefinition> m_SelectedItems = new List<IWeaponSkinItemDefinition>();
        public static ModelOffset CopiedOffset;

        public static bool EditorFullyEnabled => OverhaulFeaturesSystem.IsFeatureUnlocked(OverhaulFeatureID.PermissionToManageSkins);

        public static void SetSkinItemSelectedState(IWeaponSkinItemDefinition itemDefinition, bool newState)
        {
            if (itemDefinition == null)
                return;

            bool isSelected = ItemIsSelected(itemDefinition);

            if (newState && !isSelected)
                m_SelectedItems.Add(itemDefinition);

            else if (!newState && isSelected)
                _ = m_SelectedItems.Remove(itemDefinition);
        }

        public static bool ItemIsSelected(IWeaponSkinItemDefinition itemDefinition) => itemDefinition != null && m_SelectedItems.Contains(itemDefinition);
        public static void DeselectAll() => m_SelectedItems.Clear();
    }
}
