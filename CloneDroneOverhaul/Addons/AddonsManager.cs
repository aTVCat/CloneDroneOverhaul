using CloneDroneOverhaul.PooledPrefabs;
using CloneDroneOverhaul.Utilities;
using ModLibrary;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;
using System.IO;
using System;

namespace CloneDroneOverhaul.Addons
{
    internal class AddonsManager : Modules.ModuleBase
    {
        public override void OnActivated()
        {
            base.OnActivated();
        }

        public override bool IsEnabled()
        {
            return true;
        }
    }
}
