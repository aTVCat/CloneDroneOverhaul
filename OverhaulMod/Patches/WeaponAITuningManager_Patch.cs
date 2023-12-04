using HarmonyLib;
using ModLibrary;
using OverhaulMod.Combat.Weapons;
using System.Linq;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(WeaponAITuningManager))]
    internal static class WeaponAITuningManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("UpdateConfigForCurrentWeapon")]
        private static bool UpdateConfigForCurrentWeapon_Prefix(WeaponAITuningManager __instance, AISwordsmanController controller)
        {
            FirstPersonMover firstPersonMover = controller.GetComponent<FirstPersonMover>();
            if (!firstPersonMover)
                return false;

            WeaponModel weaponModel = firstPersonMover.GetEquippedWeaponModel();
            if (!weaponModel)
                return false;

            switch (weaponModel.WeaponType)
            {
                case WeaponType.Sword:
                    __instance.ApplyWeaponTuning(controller, __instance.SwordAITuning);
                    break;
                case WeaponType.Bow:
                    __instance.ApplyWeaponTuning(controller, __instance.BowAITuning);
                    break;
                case WeaponType.Hammer:
                    UpgradeCollection component = controller.GetComponent<UpgradeCollection>();
                    if (!component)
                        return false;

                    int upgradeLevel = component.GetUpgradeLevel(UpgradeType.Hammer);
                    HammerLevelAITuning hammerLevelAITuning = __instance.HammerLevelAITunings.SingleOrDefault((HammerLevelAITuning config) => config.HammerLevel == upgradeLevel);
                    if (hammerLevelAITuning != null)
                    {
                        __instance.ApplyWeaponTuning(controller, hammerLevelAITuning.MeleeWeaponAITuning);
                        return false;
                    }
                    break;
            }

            if (weaponModel.IsModded())
            {
                ModWeaponModel modWeaponModel = weaponModel.ModdedReference();
                if (modWeaponModel && modWeaponModel.AITuning != null)
                {
                    __instance.ApplyWeaponTuning(controller, modWeaponModel.AITuning);
                }
            }
            return false;
        }
    }
}
