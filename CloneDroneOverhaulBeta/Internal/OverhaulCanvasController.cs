using CDOverhaul.Enumerators;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulCanvasController : OverhaulController
    {
        /// <summary>
        /// The prefab name of HUD
        /// </summary>
        private const string OverhaulHUDName = "BetaOverhaulUI";

        /// <summary>
        /// The modded object with all UI references
        /// </summary>
        public ModdedObject HUDModdedObject
        {
            get;
            private set;
        }
        public GameObject[] HUDPrefabsArray
        {
            get;
            private set;
        }

        private GameObject m_CanvasFromPrefab;

        public bool HasSpawnedHUD => HUDModdedObject != null;

        public override void Initialize()
        {
            m_CanvasFromPrefab = Instantiate(AssetController.GetAsset(OverhaulHUDName, OverhaulAssetsPart.Part1));
            ModdedObject moddedObject = m_CanvasFromPrefab.GetComponent<ModdedObject>();
            HUDModdedObject = moddedObject.GetObject<ModdedObject>(0);
            ParentTransformToGameUIRoot(HUDModdedObject.transform);
            SetCanvasPixelPerfect(true);

            ModdedObject prefabsModdedObject = moddedObject.GetObject<ModdedObject>(1);
            HUDPrefabsArray = new GameObject[prefabsModdedObject.objects.Count];
            int index = 0;
            foreach (Object @object in prefabsModdedObject.objects)
            {
                HUDPrefabsArray[index] = @object as GameObject;
                index++;
            }
            prefabsModdedObject.gameObject.SetActive(false);

            _ = AddHUD<UIVersionLabel>(HUDModdedObject.GetObject<ModdedObject>(0));

            m_CanvasFromPrefab.GetComponent<Canvas>().enabled = false;
            m_CanvasFromPrefab.GetComponent<CanvasScaler>().enabled = false;
        }

        /// <summary>
        /// Add new HUD controller to <paramref name="moddedObject"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moddedObject"></param>
        /// <returns></returns>
        public T AddHUD<T>(in ModdedObject moddedObject) where T : OverhaulUI
        {
            return OverhaulController.InitializeController<T>(moddedObject.transform);
        }

        /// <summary>
        /// TBA
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

        /// <summary>
        /// Set <see cref="Canvas.pixelPerfect"/> value in <see cref="GameUIRoot"/>
        /// </summary>
        /// <param name="value"></param>
        public void SetCanvasPixelPerfect(in bool value)
        {
            GameUIRoot.Instance.GetComponent<Canvas>().pixelPerfect = value;
        }

        public override void OnModDeactivated()
        {
            destroyHUD();
        }

        private void destroyHUD()
        {
            if (!HasSpawnedHUD)
            {
                return;
            }

            Destroy(HUDModdedObject.gameObject);
            Destroy(m_CanvasFromPrefab);
        }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}
