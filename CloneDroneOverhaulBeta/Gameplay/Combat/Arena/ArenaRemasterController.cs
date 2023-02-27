using CDOverhaul.LevelEditor;
using UnityEngine;

namespace CDOverhaul.ArenaRemaster
{
    public class ArenaRemasterController : OverhaulController
    {
        public const bool SupportEnemiesLeftLabel = true;

        private bool m_DebugArenaToggleState;

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

            m_DebugArenaToggleState = true;
            GameObject spawnedPrefab = Instantiate(GetArenaRemasterPrefab());
            Transform spawnedPrefabTransform = spawnedPrefab.transform;
            spawnedPrefabTransform.SetParent(ArenaTransform);
            spawnedPrefabTransform.position = Vector3.zero;
            spawnedPrefabTransform.eulerAngles = Vector3.zero;
            ArenaRemaster = spawnedPrefab.GetComponent<ModdedObject>();

            setUpArrowsInterior();
            if(SupportEnemiesLeftLabel) setUpLabelsInterior();
        }

        protected override void OnDisposed()
        {
            WorldRootTransform = null;
            ArenaTransform = null;
            ArenaRemaster = null;
            EnemiesLeftPositionOverride = null;
        }

        public override void OnModDeactivated()
        {
            SetOriginalArenaInteriorVisible(true);
            DestroyBehaviour();
        }

        public GameObject GetArenaRemasterPrefab()
        {
            return AssetController.GetAsset("P_ArenaRemaster", OverhaulAssetsPart.Arena_Update);
        }

        /// <summary>
        /// Set "Arena2019" (Arena interior) gameobject visible
        /// </summary>
        /// <param name="value"></param>
        public void SetOriginalArenaInteriorVisible(in bool value)
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
            WorldRoot root = WorldRoot.Instance;
            if(root == null)
            {
                return false;
            }

            WorldRootTransform = root.transform;
            if(WorldRootTransform == null)
            {
                return false;
            }
            ArenaTransform = TransformUtils.FindChildRecursive(WorldRootTransform, "ArenaFinal");
            return ArenaTransform != null;
        }

        private void setUpArrowsInterior()
        {
            ArenaRemasterArrowBlinker b1 = ArenaRemaster.GetObject<Transform>(2).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            ArenaRemasterArrowBlinker b2 = ArenaRemaster.GetObject<Transform>(1).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            ArenaRemasterArrowBlinker b3 = ArenaRemaster.GetObject<Transform>(0).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            b1.Initialize(b1.GetComponent<ModdedObject>(), b2);
            b1.ChangeState(true);
            b2.Initialize(b2.GetComponent<ModdedObject>(), b3);
            b3.Initialize(b3.GetComponent<ModdedObject>(), b1);

            ArenaArrowManager.Instance.TopGotoElevatorArrows[0].SetActive(false);
            ArenaArrowManager.Instance.TopGotoElevatorArrows[0] = b1.transform.parent.gameObject;
        }

        private void setUpLabelsInterior()
        {
            ArenaRemasterEnemyCounter b1 = ArenaRemaster.GetObject<Transform>(3).gameObject.AddComponent<ArenaRemasterEnemyCounter>();
            b1.Initialize(b1.GetComponent<ModdedObject>(), this);
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            if (!OverhaulVersion.IsDebugBuild)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                m_DebugArenaToggleState = !m_DebugArenaToggleState;
                SetOriginalArenaInteriorVisible(m_DebugArenaToggleState);
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
