using System;
using System.Collections.Generic;
using UnityEngine;
using ModLibrary;
using UnityEngine.Rendering;
using CloneDroneOverhaul.Utilities;
using CloneDroneOverhaul.PooledPrefabs;
using UnityStandardAssets.ImageEffects;

namespace CloneDroneOverhaul.Modules
{
    public class CinematicGameManager : ModuleBase
    {
        public override bool ShouldWork()
        {
            return true;
        }

        public static void SwitchHud()
        {
            bool shouldHide = CutSceneManager.Instance.IsInCutscene() || Utilities.PlayerUtilities.GetPlayerRobotInfo().IsNull;
            if (shouldHide)
            {
                GameUIRoot.Instance.SetPlayerHUDVisible(false);
                return;
            }
            GameUIRoot.Instance.SetPlayerHUDVisible(!GameUIRoot.Instance.EnergyUI.gameObject.activeInHierarchy);
        }
    }
}
