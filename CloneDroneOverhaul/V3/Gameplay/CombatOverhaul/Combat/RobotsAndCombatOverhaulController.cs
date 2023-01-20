using CloneDroneOverhaul.V3;
using CloneDroneOverhaul.V3.Base;
using CloneDroneOverhaul.V3.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using ModLibrary;

namespace CloneDroneOverhaul.V3.Gameplay
{
    /// <summary>
    /// A full reimagination of combat (i don't think so), coming in 0.3 or later.
    /// </summary>
    public class RobotsAndCombatOverhaulController : V3_ModControllerBase
    {
        private const string FIRSTPERSONMOVER_ONSPAWN_EVENT_NAME = "firstPersonMover.OnSpawn";

        /// <summary>
        /// May combat overhaul patch the game?
        /// </summary>
        public static bool ShouldWork => !GameModeManager.IsMultiplayer();// && OverhaulDescription.TEST_FEATURES_ENABLED;

        /// <summary>
        /// All weapons in game, including modded
        /// </summary>
        public static List<WeaponType> AllWeaponTypes
        {
            get
            {
                List<WeaponType> result = new List<WeaponType>(Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>());
                result.AddRange(ModdedWeaponTypes);
                return result;
            }
        }

        /// <summary>
        /// All weapons in vanilla game
        /// </summary>
        public static List<WeaponType> VanillaWeaponTypes
        {
            get
            {
                List<WeaponType> result = new List<WeaponType>(Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>());
                return result;
            }
        }

        /// <summary>
        /// All modded weapons
        /// </summary>
        public static List<WeaponType> ModdedWeaponTypes
        {
            get
            {
                List<WeaponType> result = _moddedWeapons;
                return result;
            }
        }

        private static List<WeaponType> _moddedWeapons;

        /// <summary>
        /// Called when first person mover is initialized
        /// </summary>
        /// <param name="mover"></param>
        public static void ConfigureFirstPersonMover(FirstPersonMover mover)
        {
            if (!ShouldWork)
            {
                return;
            }

            DelegateScheduler.Instance.Schedule(delegate
            {
                AddBetterMovement(mover);
            }, 0.2f);
        }

        #region Event listeners

        public override void OnEvent(in string eventName, in object[] args)
        {
            if (!ShouldWork)
            {
                return;
            }

            if (eventName == FIRSTPERSONMOVER_ONSPAWN_EVENT_NAME)
            {
                if (args[0] is RobotShortInformation robot && !robot.IsNull && robot.IsFPM)
                {
                    FirstPersonMover firstPersonMover = robot.Instance as FirstPersonMover;
                    ConfigureFirstPersonMover(firstPersonMover);
                }
            }
        }

        #endregion

        #region Movement patch

        public static void AddBetterMovement(in FirstPersonMover mover)
        {
            // Implement dash upgrade to upgrade tree
            // Don't give dash on start

            if(mover == null)
            {
                return;
            }

            RobotShortInformation info = mover.GetRobotInfo();
            if (info.IsFPM)
            {
                UpgradeCollection collection = info.UpgradeCollection;
                if(collection != null)
                {
                    Dictionary<UpgradeType, int> d = collection.GetUpgradesDictionary();
                    if (d != null && d.ContainsKey(UpgradeType.Dash))
                    {
                        mover.gameObject.AddComponent<MovementController>().Initialize(mover);
                        if(mover.CharacterType == EnemyType.Swordsman1)
                        {
                            d[UpgradeType.Dash] = 0;
                        }
                        collection.SetPrivateField<Dictionary<UpgradeType, int>>("_upgradeLevels", d);
                        mover.RefreshUpgrades();
                    }
                }
            }
        }

        #endregion
    }
}
