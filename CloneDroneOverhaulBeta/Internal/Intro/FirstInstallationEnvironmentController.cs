using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDOverhaul.Demo
{
    public static class FirstInstallationEnvironmentController
    {
        public static void Test_Scene()
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(OverhaulMod.Core.ModDirectory + "overhauldemostuff");
            string[] scenePath = bundle.GetAllScenePaths();
            SceneManager.LoadScene(scenePath[0]);
        }
    }
}
