using CDOverhaul.LevelEditor;
using ModLibrary;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.ArenaRemaster
{
    /// <summary>
    /// The prototype variant of new arena.
    /// </summary>
    [Obsolete("The prototyping phase has ended")]
    public class ArenaRemasterPrototypeController : OverhaulController
    {
        public const bool SupportEnemiesLeftLabel = true;

        private List<MeshCollider> m_ModdedColliders;

        public Transform WorldRootTransform;
        public Transform ArenaTransform;
        public Transform OgArenaColliders;
        public Transform OgBattleCruiser;
        public Transform OgArenaFloor;
        public Transform OgArenaLvlEditorFloor;
        public Transform OgArenaGarbageDoor;
        public Transform OgArenaGarbageDoorStatic;
        public Transform OgArenaLift;
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
            GameObject spawnedPrefab = Instantiate(GetArenaRemasterPrefab());
            Transform spawnedPrefabTransform = spawnedPrefab.transform;
            spawnedPrefabTransform.SetParent(ArenaTransform);
            spawnedPrefabTransform.position = Vector3.zero;
            spawnedPrefabTransform.eulerAngles = Vector3.zero;
            ArenaRemaster = spawnedPrefab.GetComponent<ModdedObject>();
            _ = ArenaRemaster.gameObject.AddComponent<ArenaRemasterColorSwaper>();

            setUpStandsInterior();
            setUpArrowsInterior();
            setUpBattleCruiser();
            setUpColliders();
            setUpFloor();
            setUpGarbageDoor();
            setUpLift();
            if(SupportEnemiesLeftLabel) setUpLabelsInterior();

            SetOriginalArenaInteriorVisible(false);
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
            if (IsDisposedOrDestroyed() || !OverhaulVersion.UseArenaRemaster)
            {
                return;
            }

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

            OgArenaFloor.gameObject.SetActive(value);
            OgArenaLvlEditorFloor.gameObject.SetActive(value && GameModeManager.IsInLevelEditor());
            OgArenaGarbageDoor.gameObject.SetActive(true);
        }

        public void SetOriginalArenaCollidersActive(in bool value)
        {
            OgArenaColliders.gameObject.SetActive(value);
            if (!m_ModdedColliders.IsNullOrEmpty())
            {
                foreach(MeshCollider c in m_ModdedColliders)
                {
                    c.enabled = !value;
                }
            }
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

        private void setUpBattleCruiser()
        {
            OgBattleCruiser = TransformUtils.FindChildRecursive(ArenaTransform, "Battlecruiser");

            Transform transformBC = UnityEngine.Object.Instantiate<Transform>(Singleton<EnemyFactory>.Instance.Enemies[56].EnemyPrefab.GetComponent<BattleCruiserController>().CharacterModelPrefab.transform);
            foreach (MonoBehaviour proj in transformBC.GetComponentsInChildren<MonoBehaviour>())
            {
                UnityEngine.Object.Destroy(proj);
            }
            foreach (Renderer proj in transformBC.GetComponentsInChildren<Renderer>())
            {
                proj.material = AssetsController.GetAsset<Material>("M_NoFog", OverhaulAssetsPart.Part2);
            }
            TransformUtils.HideAllChildren(transformBC);
            transformBC.GetChild(0).gameObject.SetActive(true);
            transformBC.SetParent(OgBattleCruiser, false);
            transformBC.localPosition = new Vector3(0f, 0f, -0.65f);
            transformBC.localEulerAngles = Vector3.zero;
            transformBC.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            OgBattleCruiser.GetComponent<MeshRenderer>().enabled = false;
        }

        private void setUpArrowsInterior()
        {
            ArenaRemasterArrowBlinker b1 = ArenaRemaster.GetObject<Transform>(2).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            ArenaRemasterArrowBlinker b2 = ArenaRemaster.GetObject<Transform>(1).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            ArenaRemasterArrowBlinker b3 = ArenaRemaster.GetObject<Transform>(0).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            _ = b1.Initialize(b1.GetComponent<ModdedObject>(), b2);
            b1.ChangeState(true);
            _ = b2.Initialize(b2.GetComponent<ModdedObject>(), b3);
            _ = b3.Initialize(b3.GetComponent<ModdedObject>(), b1);

            ArenaRemasterArrowBlinker ur1 = ArenaRemaster.GetObject<Transform>(8).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            ArenaRemasterArrowBlinker ur2 = ArenaRemaster.GetObject<Transform>(7).gameObject.AddComponent<ArenaRemasterArrowBlinker>().Initialize(null, ur1);
            ArenaRemasterArrowBlinker ur3 = ArenaRemaster.GetObject<Transform>(6).gameObject.AddComponent<ArenaRemasterArrowBlinker>().Initialize(null, ur2);
            _ = ur1.Initialize(null, ur3);
            ur1.ChangeState(true);

            ArenaRemasterArrowBlinker un1 = ArenaRemaster.GetObject<Transform>(14).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            ArenaRemasterArrowBlinker un2 = ArenaRemaster.GetObject<Transform>(13).gameObject.AddComponent<ArenaRemasterArrowBlinker>().Initialize(null, un1);
            ArenaRemasterArrowBlinker un3 = ArenaRemaster.GetObject<Transform>(12).gameObject.AddComponent<ArenaRemasterArrowBlinker>().Initialize(null, un2);
            ArenaRemasterArrowBlinker un4 = ArenaRemaster.GetObject<Transform>(11).gameObject.AddComponent<ArenaRemasterArrowBlinker>().Initialize(null, un3);
            ArenaRemasterArrowBlinker un5 = ArenaRemaster.GetObject<Transform>(10).gameObject.AddComponent<ArenaRemasterArrowBlinker>().Initialize(null, un4);
            _ = un1.Initialize(null, un5);
            un1.ChangeState(true);

            /*
            m_OgTopGotoElevatorArrows = ArenaArrowManager.Instance.TopGotoElevatorArrows[0];
            ArenaArrowManager.Instance.TopGotoElevatorArrows[0] = b1.transform.parent.gameObject;
            ArenaArrowManager.Instance.TopGotoElevatorArrows[0].SetActive(m_OgTopGotoElevatorArrows.activeSelf); 
            m_OgTopGotoElevatorArrows.SetActive(false);

            m_OgUpgradeRoomGoToArenaArrows = ArenaArrowManager.Instance.UpgradeRoomGoToArenaArrows[0];
            ArenaArrowManager.Instance.UpgradeRoomGoToArenaArrows[0] = ur1.transform.parent.gameObject;
            ArenaArrowManager.Instance.UpgradeRoomGoToArenaArrows[0].SetActive(m_OgUpgradeRoomGoToArenaArrows.activeSelf);
            m_OgUpgradeRoomGoToArenaArrows.SetActive(false);*/
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

        private void setUpColliders()
        {
            m_ModdedColliders = new List<MeshCollider>();
            foreach (MeshCollider c in ArenaRemaster.GetObject<Transform>(15).GetComponentsInChildren<MeshCollider>())
            {
                MeshFilter f = c.transform.GetChild(0).GetComponent<MeshFilter>();
                f.gameObject.layer = Layers.Environment;
                c.sharedMesh = f.mesh;
                c.gameObject.layer = Layers.Environment;
                m_ModdedColliders.Add(c);
            } 
        }

        private void setUpFloor()
        {
            OgArenaFloor = TransformUtils.FindChildRecursive(WorldRootTransform, "ArenaFloor");
            OgArenaLvlEditorFloor = TransformUtils.FindChildRecursive(WorldRootTransform, "EditorArenaFloor");
        }

        private void setUpGarbageDoor()
        {
            OgArenaGarbageDoorStatic = GarbageManager.Instance.Shute.StaticDoor.transform;
            OgArenaGarbageDoorStatic.gameObject.SetActive(false);
            GarbageManager.Instance.Shute.StaticDoor = ArenaRemaster.GetObject<Transform>(16).gameObject;

            OgArenaGarbageDoor = TransformUtils.FindChildRecursive(WorldRootTransform, "GarbageDoor2019");
        }

        private void setUpLift()
        {
            OgArenaLift = ArenaLiftManager.Instance.Lift.transform.GetChild(0);
            OgArenaLift.gameObject.SetActive(false);

            Transform newLift = ArenaRemaster.GetObject<Transform>(17);
            newLift.SetParent(ArenaLiftManager.Instance.Lift.transform, true);
            newLift.localScale = new Vector3(4.9f, 4.9f, 4.9f);
            newLift.localPosition = new Vector3(-98.3f, -122.5f, -7.85f);

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
