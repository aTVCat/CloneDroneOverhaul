using OverhaulMod.Content.Personalization.Objects;
using OverhaulMod.Engine;
using OverhaulMod.Gameplay;
using OverhaulMod.Utils;
using System;
using System.IO;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemVerificationManager : Singleton<PersonalizationItemVerificationManager>
    {
        public bool DoesWeaponSkinSupportWeaponVariant(PersonalizationEditorPlacedObject root, WeaponVariant2 weaponVariant, out bool weaponDoesHaveThisVariant)
        {
            weaponDoesHaveThisVariant = false;
            if (!root)
                return false;

            switch (root.SpawnInfo.ItemInfo.Weapon)
            {
                case WeaponType.Sword:
                    weaponDoesHaveThisVariant = true;
                    break;
                case WeaponType.Bow:
                    weaponDoesHaveThisVariant = weaponVariant == WeaponVariant2.Normal;
                    break;
                case WeaponType.Hammer:
                case WeaponType.Spear:
                case ModWeaponsManager.SCYTHE_TYPE:
                    weaponDoesHaveThisVariant = weaponVariant == WeaponVariant2.Normal || weaponVariant == WeaponVariant2.OnFire;
                    break;
            }

            PersonalizationEditorCVMModel[] cvmModels = root.GetComponentsInChildren<PersonalizationEditorCVMModel>(true);
            foreach (PersonalizationEditorCVMModel cvmModel in cvmModels)
            {
                System.Collections.Generic.Dictionary<WeaponVariant2, CVMModelPreset> d = cvmModel.Presets;
                if (d.IsNullOrEmpty())
                    continue;

                if (d.ContainsKey(weaponVariant))
                {
                    CVMModelPreset cvmModelPreset = d[weaponVariant];
                    if (cvmModelPreset != null && !cvmModelPreset.CvmFilePath.IsNullOrEmpty() && File.Exists(Path.Combine(root.SpawnInfo.ItemInfo.RootFolderPath, cvmModelPreset.CvmFilePath)))
                    {
                        return true;
                    }
                }
            }

            PersonalizationEditorVoxModel[] volumes = root.GetComponentsInChildren<PersonalizationEditorVoxModel>(true);
            foreach (PersonalizationEditorVoxModel volume in volumes)
            {
                System.Collections.Generic.Dictionary<WeaponVariant2, VolumeSettingsPreset> d = volume.VolumeSettingPresets;
                if (d.IsNullOrEmpty())
                    continue;

                if (d.ContainsKey(weaponVariant))
                {
                    VolumeSettingsPreset volumeSettingsPreset = d[weaponVariant];
                    if (volumeSettingsPreset != null && !volumeSettingsPreset.VoxFilePath.IsNullOrEmpty() && File.Exists(Path.Combine(root.SpawnInfo.ItemInfo.RootFolderPath, volumeSettingsPreset.VoxFilePath)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void SendItemToVerification(PersonalizationItemInfo personalizationItemInfo, Action successCallback, Action<string> errorCallback)
        {
            PersonalizationEditorDataManager.Instance.ExportItem(personalizationItemInfo, out string dest, Path.GetTempPath());
            PostmanManager.Instance.SendVerificationRequest(dest, personalizationItemInfo, successCallback, errorCallback);
        }
    }
}
