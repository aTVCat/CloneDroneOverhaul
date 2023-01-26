using CDOverhaul.Enumerators;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class HUDControllers : ModController
    {
        /// <summary>
        /// The prefab name of HUD
        /// </summary>
        private const string OverhaulHUDName = "BetaOverhaulUI";

        /// <summary>
        /// The modded object with all UI references
        /// </summary>
        public ModdedObject HUDModdedObject { get; private set; }

        public GameObject[] HUDPrefabsArray { get; private set; }

        public bool HasSpawnedHUD => HUDModdedObject != null;

        public override void Initialize()
        {
            GameObject spawnedHud = Instantiate(AssetController.GetAsset(OverhaulHUDName, EModAssetBundlePart.Part1));
            ModdedObject moddedObject = spawnedHud.GetComponent<ModdedObject>();
            HUDModdedObject = moddedObject.GetObject<ModdedObject>(0);
            ParentTransformToGameUIRoot(HUDModdedObject.transform);
            setPixelPerfectValue(true);

            ModdedObject prefabsModdedObject = moddedObject.GetObject<ModdedObject>(1);
            HUDPrefabsArray = new GameObject[prefabsModdedObject.objects.Count];
            int index = 0;
            foreach (Object @object in prefabsModdedObject.objects)
            {
                HUDPrefabsArray[index] = @object as GameObject;
                index++;
            }
            prefabsModdedObject.gameObject.SetActive(false);

            OverhaulEventManager.AddListenerToEvent(OverhaulBase.ModDeactivatedEventString, destroyHUD);

            AddHUD<UIVersionLabel>(HUDModdedObject.GetObject<ModdedObject>(0));
            AddHUD<UISkinSelection>(HUDModdedObject.GetObject<ModdedObject>(1));

            spawnedHud.GetComponent<Canvas>().enabled = false;
            spawnedHud.GetComponent<CanvasScaler>().enabled = false;

            base.IsInitialized = true;
        }

        /// <summary>
        /// Add new HUD controller to <paramref name="moddedObject"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moddedObject"></param>
        /// <returns></returns>
        public T AddHUD<T>(in ModdedObject moddedObject) where T : UIBase
        {
            return ModControllerManager.NewController<T>(moddedObject.transform);
        }

        /// <summary>
        /// Get a <see cref="GameObject"/> from Overhaul spawned hud
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetHUDPrefab(in string name)
        {
            foreach (GameObject @object in HUDPrefabsArray)
            {
                if (@object == null)
                {
                    break;
                }
                else if (@object.name == name)
                {
                    return @object;
                }
            }
            return null;
        }

        /// <summary>
        /// Parent transform to <see cref="GameUIRoot"/>.Instance.transform and instantly fix position and scale
        /// </summary>
        /// <param name="transform"></param>
        public void ParentTransformToGameUIRoot(in Transform transform)
        {
            transform.SetParent(GameUIRoot.Instance.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        private void setPixelPerfectValue(in bool value)
        {
            GameUIRoot.Instance.GetComponent<Canvas>().pixelPerfect = value;
        }

        private void destroyHUD()
        {
            if (!HasSpawnedHUD)
            {
                return;
            }

            Destroy(HUDModdedObject.gameObject);
        }
    }
}
