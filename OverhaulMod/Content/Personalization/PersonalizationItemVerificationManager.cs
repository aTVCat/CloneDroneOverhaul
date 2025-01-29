﻿using OverhaulMod.Combat;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.IO;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemVerificationManager : Singleton<PersonalizationItemVerificationManager>
    {
        public bool DoesWeaponSkinSupportWeaponVariant(PersonalizationEditorObjectBehaviour root, WeaponVariant weaponVariant, out bool weaponDoesHaveThisVariant)
        {
            weaponDoesHaveThisVariant = false;
            if (!root)
                return false;

            switch (root.ControllerInfo.ItemInfo.Weapon)
            {
                case WeaponType.Sword:
                    weaponDoesHaveThisVariant = true;
                    break;
                case WeaponType.Bow:
                    weaponDoesHaveThisVariant = weaponVariant == WeaponVariant.Normal;
                    break;
                case WeaponType.Hammer:
                case WeaponType.Spear:
                case ModWeaponsManager.SCYTHE_TYPE:
                    weaponDoesHaveThisVariant = weaponVariant == WeaponVariant.Normal || weaponVariant == WeaponVariant.OnFire;
                    break;
            }

            PersonalizationEditorObjectCVMModel[] cvmModels = root.GetComponentsInChildren<PersonalizationEditorObjectCVMModel>(true);
            foreach (PersonalizationEditorObjectCVMModel cvmModel in cvmModels)
            {
                System.Collections.Generic.Dictionary<WeaponVariant, CVMModelPreset> d = cvmModel.presets;
                if (d.IsNullOrEmpty())
                    continue;

                if (d.ContainsKey(weaponVariant))
                {
                    CVMModelPreset cvmModelPreset = d[weaponVariant];
                    if (cvmModelPreset != null && !cvmModelPreset.CvmFilePath.IsNullOrEmpty() && File.Exists(Path.Combine(root.ControllerInfo.ItemInfo.RootFolderPath, cvmModelPreset.CvmFilePath)))
                    {
                        return true;
                    }
                }
            }

            PersonalizationEditorObjectVolume[] volumes = root.GetComponentsInChildren<PersonalizationEditorObjectVolume>(true);
            foreach (PersonalizationEditorObjectVolume volume in volumes)
            {
                System.Collections.Generic.Dictionary<WeaponVariant, VolumeSettingsPreset> d = volume.volumeSettingPresets;
                if (d.IsNullOrEmpty())
                    continue;

                if (d.ContainsKey(weaponVariant))
                {
                    VolumeSettingsPreset volumeSettingsPreset = d[weaponVariant];
                    if (volumeSettingsPreset != null && !volumeSettingsPreset.VoxFilePath.IsNullOrEmpty() && File.Exists(Path.Combine(root.ControllerInfo.ItemInfo.RootFolderPath, volumeSettingsPreset.VoxFilePath)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void SendItemToVerification(PersonalizationItemInfo personalizationItemInfo, Action successCallback, Action<string> errorCallback)
        {
            PersonalizationEditorManager.Instance.ExportItem(personalizationItemInfo, out string dest, Path.GetTempPath());
            PostmanManager.Instance.SendVerificationRequest(dest, personalizationItemInfo, successCallback, errorCallback);
        }
    }
}
