using OverhaulAPI;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class NewWeaponsController : OverhaulGameplayController
    {
        public static WeaponType GarbagePickerWeaponType;

        private static readonly List<AddedWeaponModel> m_AlwaysEquippedWeaponModels = new List<AddedWeaponModel>();
        private static readonly List<AddedWeaponModel> m_AddedWeaponModels = new List<AddedWeaponModel>();
        private static readonly ModelOffset m_BoomerangModelOffset = new ModelOffset(new Vector3(0.25f, 0.06f, -0.05f), new Vector3(-130f, -90f, 95f), Vector3.one * 0.6f);

        public override void Initialize()
        {
            base.Initialize();

            m_AddedWeaponModels.Clear();
            m_AlwaysEquippedWeaponModels.Clear();

            addUpgrades();

            BoomerangWeaponModel boomerang = WeaponsAdder.AddWeaponModel<BoomerangWeaponModel>(AssetsController.GetAsset("P_WM_Boomerang_2", OverhaulAssetsPart.Part2).transform, m_BoomerangModelOffset);
            m_AddedWeaponModels.Add(boomerang);

            if (!OverhaulVersion.JuneDemoEnabled)
            {
                return;
            }
            GameObject garbagePickerWeaponModelObject = new GameObject("GarbagePickerWeapon");
            GarbagePickerWeaponModel garbage = WeaponsAdder.AddWeaponModel<GarbagePickerWeaponModel>(garbagePickerWeaponModelObject.transform, ModelOffset.Default);
            GarbagePickerWeaponType = garbage.WeaponType;
            m_AlwaysEquippedWeaponModels.Add(garbage);
        }

        private void addUpgrades()
        {
            _ = UpgradesAdder.AddUpgrade<UpgradeDescription>(OverhaulMod.Base,
                (UpgradeType)6700,
                1,
                "Boomerang",
                "TBA",
                null);
            UpgradeDescription fire = UpgradesAdder.AddUpgrade<UpgradeDescription>(OverhaulMod.Base,
                (UpgradeType)6701,
                1,
                "Fire",
                "TBA",
                null, (UpgradeType)6700);
            fire.SkillPointCostDefault = 2;
           /* UpgradeDescription autoTargeting = UpgradesAdder.AddUpgrade<UpgradeDescription>(OverhaulMod.Base,
                (UpgradeType)6702,
                1,
                "Auto-Targeting",
                "This one will be probably scrapped",
                null, (UpgradeType)6700);
            autoTargeting.SkillPointCostDefault = 4;*/
            UpgradeDescription throwRange1 = UpgradesAdder.AddUpgrade<UpgradeDescription>(OverhaulMod.Base,
                (UpgradeType)6703,
                1,
                "Throw range 1",
                "TBA",
                null, (UpgradeType)6700);
            throwRange1.SkillPointCostDefault = 2;
            UpgradeDescription throwRange2 = UpgradesAdder.AddUpgrade<UpgradeDescription>(OverhaulMod.Base,
                (UpgradeType)6703,
                2,
                "Throw range 2",
                "TBA",
                null, (UpgradeType)6703);
            throwRange2.SkillPointCostDefault = 2;
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel || !OverhaulGamemodeManager.SupportsCombatOverhaul())
            {
                return;
            }

            WeaponsAdder.AddWeaponModelsToFirstPersonMover(firstPersonMover, m_AddedWeaponModels, false, out List<AddedWeaponModel> models);
            NewWeaponsRobotExpansion exp = firstPersonMover.gameObject.AddComponent<NewWeaponsRobotExpansion>();
            exp.AllCustomWeapons = models;
            if (OverhaulVersion.JuneDemoEnabled)
            {
                WeaponsAdder.AddWeaponModelsToFirstPersonMover(firstPersonMover, m_AlwaysEquippedWeaponModels, true, out List<AddedWeaponModel> models1);
                exp.AllCustomWeapons.AddRange(models1);
            }
        }
    }
}