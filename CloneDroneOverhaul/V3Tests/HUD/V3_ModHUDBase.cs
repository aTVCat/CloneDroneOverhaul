using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloneDroneOverhaul.V3Tests.Base;

namespace CloneDroneOverhaul.V3Tests.HUD
{
    public class V3_ModHUDBase : V3_ModControllerBase
    {
        public ModdedObject ModdedObject { get; private set; }

        public static T AddHUD<T>(in ModdedObject moddedObject) where T : V3_ModHUDBase
        {
            T result = null;

            result = V3_MainModController.AddManager<T>(null, moddedObject.transform);
            result.ModdedObject = moddedObject;
            result.gameObject.SetActive(true);

            return result;
        }
    }
}
