using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using System.Collections;

namespace OverhaulMod.Gameplay
{
    public class ModCharacterManager : Singleton<ModCharacterManager>
    {
        public void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            StartCoroutine(waitForRobotInitialization(firstPersonMover));
        }

        public void OnUpgradesStartedRefreshing(FirstPersonMover firstPersonMover, UpgradeCollection collection)
        {
            firstPersonMover.AddWeaponToEquipppedIfHasUpgradeAndModelPresent(ModUpgradesManager.SCYTHE_UNLOCK_UPGRADE, ModWeaponsManager.SCYTHE_TYPE);
            firstPersonMover.RefreshModWeaponModels();

            CharacterExtension characterExtension = ComponentCacheManager.Instance.GetCharacterExtension(firstPersonMover.transform);
            if (characterExtension) characterExtension.OnUpgradesRefreshed(collection);
        }

        public void OnUpgradesRefreshed(FirstPersonMover firstPersonMover, UpgradeCollection collection)
        {
            PersonalizationController personalizationController = ComponentCacheManager.Instance.GetPersonalizationController(firstPersonMover.transform);
            if (personalizationController) personalizationController.OnUpgrade();

            RobotWeaponBag robotWeaponBag = firstPersonMover.GetComponent<RobotWeaponBag>();
            if (robotWeaponBag) robotWeaponBag.OnUpgrade();
        }

        private IEnumerator waitForRobotInitialization(FirstPersonMover firstPersonMover)
        {
            while (firstPersonMover && (!firstPersonMover.gameObject.activeInHierarchy || !firstPersonMover.HasCharacterModel()))
                yield return null;

            for (int i = 0; i < 3; i++) yield return null;

            if (!firstPersonMover || !firstPersonMover.IsAttachedAndAlive())
                yield break;

            CharacterExtension characterExtension = firstPersonMover.gameObject.AddComponent<CharacterExtension>();
            characterExtension.Initialize(firstPersonMover);

            PersonalizationController personalizationController = firstPersonMover.gameObject.AddComponent<PersonalizationController>();
            personalizationController.Initialize(firstPersonMover);

            bool isWeaponBagAllowedInCurrentGameMode = GameModeManager.IsSinglePlayer() || GameModeManager.IsCoop();
            if (!firstPersonMover.IsMindSpaceCharacter && (isWeaponBagAllowedInCurrentGameMode || firstPersonMover.IsMainPlayer()))
            {
                RobotWeaponBag weaponBag = firstPersonMover.gameObject.AddComponent<RobotWeaponBag>();
                weaponBag.Initialize(firstPersonMover, personalizationController);
            }

            if (ModBuild.IsInVRMode()) yield break;

            while (firstPersonMover && !firstPersonMover._playerCamera || ((GameModeManager.IsMultiplayerDuel() || GameModeManager.IsBattleRoyale()) && !firstPersonMover.HasConstructionFinished()))
                yield return null;

            if (firstPersonMover && firstPersonMover.HasCharacterModel() && firstPersonMover._playerCamera)
                CameraManager.Instance.AddControllers(firstPersonMover._playerCamera, firstPersonMover);

            yield break;
        }
    }
}
