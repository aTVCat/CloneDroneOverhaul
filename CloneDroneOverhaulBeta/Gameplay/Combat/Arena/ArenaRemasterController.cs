using CDOverhaul.LevelEditor;
using UnityEngine;

namespace CDOverhaul.ArenaRemaster
{
    public class ArenaRemasterController : OverhaulController
    {
        public const bool SupportEnemiesLeftLabel = true;

        private GameObject m_OgTopGotoElevatorArrows;
        private GameObject m_OgUpgradeRoomGoToArenaArrows;

        private bool m_DebugArenaToggleState;

        public Transform WorldRootTransform;
        public Transform ArenaTransform;
        public Transform OgArenaColliders;
        public ModdedObject ArenaRemaster;

        private Transform m_StandsRight;
        private Transform m_StandsLeft;

        public LevelEditorArenaEnemiesCounterPoser EnemiesLeftPositionOverride;

        private AudienceManager m_AudienceManager;

        public override void Initialize()
        {
            if (!OverhaulVersion.UseArenaRemaster || !getAllRequiredReferences())
            {
                return;
            }

            m_AudienceManager = AudienceManager.Instance;
            m_AudienceManager.enabled = false;
            m_DebugArenaToggleState = true;
            GameObject spawnedPrefab = Instantiate(GetArenaRemasterPrefab());
            Transform spawnedPrefabTransform = spawnedPrefab.transform;
            spawnedPrefabTransform.SetParent(ArenaTransform);
            spawnedPrefabTransform.position = Vector3.zero;
            spawnedPrefabTransform.eulerAngles = Vector3.zero;
            ArenaRemaster = spawnedPrefab.GetComponent<ModdedObject>();
            ArenaRemaster.gameObject.AddComponent<ArenaRemasterColorSwaper>();


            setUpStandsInterior();
            setUpArrowsInterior();
            if(SupportEnemiesLeftLabel) setUpLabelsInterior();

            SetOriginalArenaCollidersActive(false);
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
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (m_OgTopGotoElevatorArrows != null) ArenaArrowManager.Instance.TopGotoElevatorArrows[0] = m_OgTopGotoElevatorArrows;
            if(ArenaRemaster != null) Destroy(ArenaRemaster.gameObject);
            SetOriginalArenaInteriorVisible(true);
            DestroyBehaviour();
        }

        public GameObject GetArenaRemasterPrefab()
        {
            return AssetsController.GetAsset("P_ArenaRemaster", OverhaulAssetsPart.Arena_Update);
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

        public void SetOriginalArenaCollidersActive(in bool value)
        {
            OgArenaColliders.gameObject.SetActive(value);
        }

        private bool getAllRequiredReferences()
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
            OgArenaColliders = TransformUtils.FindChildRecursive(WorldRootTransform, "ArenaColliders");
            if(OgArenaColliders == null)
            {
                return false;
            }
            ArenaTransform = TransformUtils.FindChildRecursive(WorldRootTransform, "ArenaFinal");
            if(ArenaTransform == null)
            {
                return false;
            }
            m_StandsRight = TransformUtils.FindChildRecursive(ArenaTransform, "StandsRight");
            m_StandsLeft = TransformUtils.FindChildRecursive(ArenaTransform, "StandsLeft");
            return m_StandsLeft != null && m_StandsRight != null;
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

            ArenaRemasterArrowBlinker ur1 = ArenaRemaster.GetObject<Transform>(8).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            ArenaRemasterArrowBlinker ur2 = ArenaRemaster.GetObject<Transform>(7).gameObject.AddComponent<ArenaRemasterArrowBlinker>().Initialize(null, ur1);
            ArenaRemasterArrowBlinker ur3 = ArenaRemaster.GetObject<Transform>(6).gameObject.AddComponent<ArenaRemasterArrowBlinker>().Initialize(null, ur2);
            ur1.Initialize(null, ur3);
            ur1.ChangeState(true);

            ArenaRemasterArrowBlinker un1 = ArenaRemaster.GetObject<Transform>(14).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            ArenaRemasterArrowBlinker un2 = ArenaRemaster.GetObject<Transform>(13).gameObject.AddComponent<ArenaRemasterArrowBlinker>().Initialize(null, un1);
            ArenaRemasterArrowBlinker un3 = ArenaRemaster.GetObject<Transform>(12).gameObject.AddComponent<ArenaRemasterArrowBlinker>().Initialize(null, un2);
            ArenaRemasterArrowBlinker un4 = ArenaRemaster.GetObject<Transform>(11).gameObject.AddComponent<ArenaRemasterArrowBlinker>().Initialize(null, un3);
            ArenaRemasterArrowBlinker un5 = ArenaRemaster.GetObject<Transform>(10).gameObject.AddComponent<ArenaRemasterArrowBlinker>().Initialize(null, un4);
            un1.Initialize(null, un5);
            un1.ChangeState(true);

            m_OgTopGotoElevatorArrows = ArenaArrowManager.Instance.TopGotoElevatorArrows[0];
            ArenaArrowManager.Instance.TopGotoElevatorArrows[0] = b1.transform.parent.gameObject;
            ArenaArrowManager.Instance.TopGotoElevatorArrows[0].SetActive(m_OgTopGotoElevatorArrows.activeSelf); 
            m_OgTopGotoElevatorArrows.SetActive(false);

            m_OgUpgradeRoomGoToArenaArrows = ArenaArrowManager.Instance.UpgradeRoomGoToArenaArrows[0];
            ArenaArrowManager.Instance.UpgradeRoomGoToArenaArrows[0] = ur1.transform.parent.gameObject;
            ArenaArrowManager.Instance.UpgradeRoomGoToArenaArrows[0].SetActive(m_OgUpgradeRoomGoToArenaArrows.activeSelf);
            m_OgUpgradeRoomGoToArenaArrows.SetActive(false);
        }

        private void setUpLabelsInterior()
        {
            ArenaRemasterEnemyCounter b1 = ArenaRemaster.GetObject<Transform>(3).gameObject.AddComponent<ArenaRemasterEnemyCounter>();
            b1.Initialize(b1.GetComponent<ModdedObject>(), this);
        }

        private void setUpStandsInterior()
        {
            m_StandsLeft.gameObject.SetActive(false);
            m_StandsRight.gameObject.SetActive(false);
        }

#if DEBUG
        private void Update()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                m_DebugArenaToggleState = !m_DebugArenaToggleState;
                SetOriginalArenaInteriorVisible(m_DebugArenaToggleState);
            }
        }
#endif

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
