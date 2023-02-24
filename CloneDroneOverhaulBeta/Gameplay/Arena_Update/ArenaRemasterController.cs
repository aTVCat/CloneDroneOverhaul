using CDOverhaul.LevelEditor;
using UnityEngine;

namespace CDOverhaul.ArenaRemaster
{
    public class ArenaRemasterController : OverhaulController
    {
        private bool _arenaToggleState = true;

        public Transform WorldRootTransform;
        public Transform ArenaTransform;
        public ModdedObject ArenaRemaster;

        public LevelEditorArenaEnemiesCounterPoser EnemiesLeftPositionOverride;

        public override void Initialize()
        {
            if (!OverhaulVersion.ArenaUpdateEnabled || !makeSureNewArenaWillWork())
            {
                return;
            }

            GameObject spawnedPrefab = Instantiate(GetArenaRemasterPrefab());
            Transform spawnedPrefabTransform = spawnedPrefab.transform;
            spawnedPrefabTransform.SetParent(ArenaTransform);
            spawnedPrefabTransform.position = Vector3.zero;
            spawnedPrefabTransform.eulerAngles = Vector3.zero;
            ArenaRemaster = spawnedPrefab.GetComponent<ModdedObject>();

            setUpArrows();
            setUpLabels();
        }

        public override void OnModDeactivated()
        {
            SetOgArenaInteriorVisible(true);
        }

        public GameObject GetArenaRemasterPrefab()
        {
            return AssetController.GetAsset("P_ArenaRemaster", Enumerators.ModAssetBundlePart.Arena_Update);
        }

        /// <summary>
        /// Set "Arena2019" (Arena interior) gameobject visible
        /// </summary>
        /// <param name="value"></param>
        public void SetOgArenaInteriorVisible(in bool value)
        {
            Transform arena2019 = TransformUtils.FindChildRecursive(ArenaTransform, "Arena2019");
            if (arena2019 != null)
            {
                arena2019.gameObject.SetActive(value);
            }

            Transform liftwall1 = TransformUtils.FindChildRecursive(ArenaTransform, "LiftWall (1)");
            if (liftwall1 != null)
            {
                liftwall1.gameObject.SetActive(value);
            }
        }

        private bool makeSureNewArenaWillWork()
        {
            WorldRootTransform = WorldRoot.Instance.transform;
            ArenaTransform = TransformUtils.FindChildRecursive(WorldRootTransform, "ArenaFinal");
            return ArenaTransform != null && WorldRootTransform != null;
        }

        private void setUpArrows()
        {
            ArenaRemasterArrowBlinker b1 = ArenaRemaster.GetObject<Transform>(2).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            ArenaRemasterArrowBlinker b2 = ArenaRemaster.GetObject<Transform>(1).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            ArenaRemasterArrowBlinker b3 = ArenaRemaster.GetObject<Transform>(0).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            b1.Initialize(b1.GetComponent<ModdedObject>(), b2);
            b1.ChangeState(true);
            b2.Initialize(b2.GetComponent<ModdedObject>(), b3);
            b3.Initialize(b3.GetComponent<ModdedObject>(), b1);
        }

        private void setUpLabels()
        {
            ArenaRemasterEnemyCounter b1 = ArenaRemaster.GetObject<Transform>(3).gameObject.AddComponent<ArenaRemasterEnemyCounter>();
            b1.Initialize(b1.GetComponent<ModdedObject>(), this);
        }

        private void Update()
        {
            if (!OverhaulVersion.ArenaUpdateEnabled)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                _arenaToggleState = !_arenaToggleState;
                SetOgArenaInteriorVisible(_arenaToggleState);
            }
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
