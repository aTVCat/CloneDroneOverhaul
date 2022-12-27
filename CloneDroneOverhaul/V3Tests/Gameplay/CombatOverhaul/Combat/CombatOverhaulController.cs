using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CloneDroneOverhaul.V3Tests.Base;
using CloneDroneOverhaul.Utilities;
using System.Linq;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    /// <summary>
    /// A full reimagination of combat (i don't think so), coming in 0.3 or later.
    /// </summary>
    public class CombatOverhaulController : V3_ModControllerBase
    {
        private const string FIRSTPERSONMOVER_ONSPAWN_EVENT_NAME = "firstPersonMover.OnSpawn";

        /// <summary>
        /// May combat overhaul patch the game?
        /// </summary>
        public static bool ShouldWork
        {
            get
            {
                return !GameModeManager.IsMultiplayer() && OverhaulDescription.TEST_FEATURES_ENABLED;
            }
        }

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
        public static void ConfigureFirstPersonMover(in FirstPersonMover mover)
        {

        }

        #region Event listeners

        public override void OnEvent(in string eventName, in object[] args)
        {
            if (!ShouldWork)
            {
                return;
            }

            if(eventName == FIRSTPERSONMOVER_ONSPAWN_EVENT_NAME)
            {
                if (args[0] is RobotShortInformation robot && !robot.IsNull && robot.IsFPM)
                {
                    FirstPersonMover firstPersonMover = robot.Instance as FirstPersonMover;
                    ConfigureFirstPersonMover(firstPersonMover);
                }
            }
        }

        #endregion
    }
}
