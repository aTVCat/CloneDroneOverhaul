using CloneDroneOverhaul.V3Tests.Gameplay;
using CloneDroneOverhaul.V3Tests.Utilities;
using System.Collections.Generic;
using UnityEngine;


namespace CloneDroneOverhaul.V3Tests.Base
{
    /// <summary>
    /// New version of main mod controller class with optimized (just trying to make it so) code
    /// </summary>
    public class V3_MainModController : V3_ModControllerBase
    {
        private static GameObject _controllersGameObject;
        private static List<V3_ModControllerBase> _spawnedControllers = new List<V3_ModControllerBase>();

        /// <summary>
        /// Called every time gameplay scene is loaded
        /// </summary>
        public static void Initialize()
        {
            _controllersGameObject = null;
            _spawnedControllers.Clear();

            OverModesController.InitializeForCurrentScene();

            GameObject newMainGameObject = new GameObject("CloneDroneOverhaul");

            GameObject newControllersGameObject = new GameObject("OverhaulModControllers");
            newControllersGameObject.transform.SetParent(newMainGameObject.transform);
            _controllersGameObject = newControllersGameObject;

            V3_MainModController mainController = AddManager<V3_MainModController>("MainModController");
            ModDataController dataControll = AddManager<ModDataController>("DataController");

            FakePrefabSystem.DataController = dataControll;
        }

        /// <summary>
        /// Adds controller monobehaviour to main gameobject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="managerName"></param>
        /// <returns></returns>
        public static T AddManager<T>(in string managerName) where T : V3_ModControllerBase
        {
            GameObject newGO = new GameObject(managerName);
            newGO.transform.SetParent(_controllersGameObject.transform);

            T result = newGO.AddComponent<T>();
            _spawnedControllers.Add(result);

            return result;
        }

        Texture _testTexture;
        public void Text_AsyncLoadTexture()
        {
            OverhaulUtilities.TextureAndMaterialUtils.LoadTextureAsync(OverhaulDescription.GetModFolder() + "Assets/Textures/TestTexture.png", delegate (Texture2D tex)
            {
                _testTexture = tex;
            });
        }

        public GameObject Test_Volume()
        {
            return VoxelUtils.CreateVolume("TestVolume", 10, 10, 10).gameObject;
        }

        public VoxReader.Interfaces.IVoxFile Test_Vox()
        {
            return VoxelUtils.ReadVoxFile(OverhaulDescription.GetModFolder() + "Assets/Vox/CloneDroneLogoV1.vox");
        }

        public void Text_ApplyVoxToVol(GameObject vol, VoxReader.Interfaces.IVoxFile vox)
        {
            VoxelUtils.ApplyVoxFileToVolume(vox, vol.GetComponent<PicaVoxel.Volume>());
        }
    }
}
