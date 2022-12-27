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

        public const string CACHED_ASSET_PREFIX = "RootModAsset_";

        private static bool _hasLoadedAssets;
        public static bool HasLoadedAssets => _hasLoadedAssets;

        /// <summary>
        /// Called every time gameplay scene is loaded
        /// </summary>
        public static void Initialize()
        {
            _controllersGameObject = null;
            _spawnedControllers.Clear();

            ModdedObject modHUDModdedObject = OverhaulMain.ModGUICanvas.GetComponent<ModdedObject>();

            OverModesController.InitializeForCurrentScene();
            OverhaulGraphicsController.Initialize();

            Optimisation.OptimiseOnStartup.Initialize();
            ArenaController.InitializeForCurrentScene();

            GameObject newMainGameObject = new GameObject("CloneDroneOverhaul");
            GameObject newControllersGameObject = new GameObject("OverhaulModControllers");
            newControllersGameObject.transform.SetParent(newMainGameObject.transform);
            _controllersGameObject = newControllersGameObject;

            V3_MainModController mainController = AddManager<V3_MainModController>("MainModController");
            ModDataController dataControll = AddManager<ModDataController>("DataController");
            AdvancedCameraController aCameraController = AddManager<AdvancedCameraController>("AdvancedCameraController");
            ModdedUpgradesController moddedUpgradesController = AddManager<ModdedUpgradesController>("ModdedUpgradesController");
            GameStatisticsController statistics = AddManager<GameStatisticsController>("GameStatisticsController");
            ArenaWeatherController weather = AddManager<ArenaWeatherController>("ArenaWeatherController");
            OverhaulGraphicsController graphics = AddManager<OverhaulGraphicsController>("OverhaulGraphicsController");
            Optimisation.ObjectsLODController lod = AddManager<Optimisation.ObjectsLODController>("ObjectsLODController");
            LevelEditor.MetaDataController metaData = AddManager<LevelEditor.MetaDataController>("MetaDataController");
            CombatOverhaulController combatOverhaulController = AddManager<CombatOverhaulController>("CombatOverhaulController");

            HUD.V3_ModHUDBase.AddHUD<HUD.UIControlInstructions>(modHUDModdedObject.GetObjectFromList<ModdedObject>(17));
            HUD.V3_ModHUDBase.AddHUD<HUD.UIImageEffects>(modHUDModdedObject.GetObjectFromList<ModdedObject>(16));

            LoadAssets();

            FakePrefabSystem.DataController = dataControll;
        }

        private static void LoadAssets()
        {
            if (HasLoadedAssets)
            {
                CallEvent("overhaul.onAssetsLoadDone", new object[] { false });
                return;
            }

            ModDataController dataControll = ModDataController.GetInstance<ModDataController>();

            Sprite unknownUpgradeIconSprite = OverhaulUtilities.TextureAndMaterialUtils.FastSpriteCreate(dataControll.LoadModAsset<Texture2D>("Textures/Upgrades/Unknown-Upgrade-16x16.png", ModAssetFileType.Image));
            unknownUpgradeIconSprite.texture.filterMode = FilterMode.Point;
            OverhaulCacheManager.CacheObject<Sprite>(unknownUpgradeIconSprite, CACHED_ASSET_PREFIX + OverhaulUpgradeDescription.UPGRADE_ICON_ASSET_PREFIX + "Unknown");

            _hasLoadedAssets = true;
            CallEvent("overhaul.onAssetsLoadDone", new object[] { true });
        }

        /// <summary>
        /// Adds controller monobehaviour to main gameobject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="managerName"></param>
        /// <returns></returns>
        public static T AddManager<T>(in string managerName, in Transform transform = null) where T : V3_ModControllerBase
        {
            T result = null;
            if (transform == null)
            {
                GameObject newGO = new GameObject(managerName);
                newGO.transform.SetParent(_controllersGameObject.transform);
                result = newGO.AddComponent<T>();
                _spawnedControllers.Add(result);
            }
            else
            {
                result = transform.gameObject.AddComponent<T>();
                _spawnedControllers.Add(result);
            }

            return result;
        }

        public static T GetManager<T>() where T : V3_ModControllerBase
        {
            T result = null;
            foreach (V3_ModControllerBase c in _spawnedControllers)
            {
                if (c is T)
                {
                    result = (T)c;
                    break;
                }
            }
            return result;
        }

        public static void CallEvent(string eventName, object[] args)
        {
            foreach (V3_ModControllerBase controllers in _spawnedControllers)
            {
                controllers.OnEvent(eventName, args);
            }
        }

        public static void SendSettingWasRefreshed(string settingName, object value)
        {
            foreach (V3_ModControllerBase controllers in _spawnedControllers)
            {
                controllers.OnSettingRefreshed(settingName, value);
            }
        }

        private Texture _testTexture;
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

        public void Test_ApplyVoxToVol(GameObject vol, VoxReader.Interfaces.IVoxFile vox)
        {
            VoxelUtils.ApplyVoxFileToVolume(vox, vol.GetComponent<PicaVoxel.Volume>());
        }

        public void Test_Transition()
        {
            TransitionAction t = new TransitionAction
            {
                Type = TranstionType.SceneSwitch,
                SceneName = "Gameplay",
                HideOnComplete = true
            };
            SceneTransitionController.StartTranstion(t, "Loading...", "This may take a while...", true);
        }
    }
}
