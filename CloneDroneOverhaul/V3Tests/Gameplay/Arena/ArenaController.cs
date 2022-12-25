using ModLibrary;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public static class ArenaController
    {
        public static ArenaParts ArenaInterior { get; private set; }

        /// <summary>
        /// Spawn decor like laptop, coffee on commentators desk
        /// </summary>
        public static void InitializeForCurrentScene()
        {
            ArenaInterior = new ArenaParts().GetObjects(UnityEngine.Object.FindObjectOfType<HideIfLevelHidesArena>().transform);

            Transform Microphone = AssetLoader.GetObjectFromFile<GameObject>("rp_newobjects", "MicrophoneModelVox").transform;
            Transform Coffee = AssetLoader.GetObjectFromFile<GameObject>("rp_newobjects", "CoffeeModelVox").transform;
            Transform Laptop = AssetLoader.GetObjectFromFile<GameObject>("rp_newobjects", "ComputerModelVox").transform;
            Transform LotsOfPapers = AssetLoader.GetObjectFromFile<GameObject>("rp_newobjects", "LotsOfPapers").transform;
            Transform trans = UnityEngine.Object.Instantiate<Transform>(Microphone, ArenaInterior.CommentatorBox);
            trans.localPosition = new Vector3(6f, 4.6f, -0.8f);
            trans.localEulerAngles = new Vector3(0f, 32f, 0f);
            Transform trans2 = UnityEngine.Object.Instantiate<Transform>(Coffee, ArenaInterior.CommentatorBox);
            trans2.localPosition = new Vector3(6f, 4.6f, -5.6f);
            trans2.localEulerAngles = new Vector3(0f, 282f, 0f);
            Transform trans3 = UnityEngine.Object.Instantiate<Transform>(Laptop, ArenaInterior.CommentatorBox);
            trans3.localPosition = new Vector3(6f, 4.645f, 3.9f);
            trans3.localEulerAngles = new Vector3(0f, 316f, 0f);
            Transform trans4 = UnityEngine.Object.Instantiate<Transform>(LotsOfPapers, ArenaInterior.CommentatorBox);
            trans4.localPosition = new Vector3(4.8f, 4.6f, -10f);
            trans4.localEulerAngles = new Vector3(0f, 305f, 0f);
            trans4.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            Transform transCam = TransformUtils.FindChildRecursive(ArenaInterior.ArenaCameraMain, "CommentatorTarget");
            transCam.localPosition = new Vector3(-65.35f, 37.4f, 0.27f);
            ArenaInterior.CommentatorBox.GetChild(0).GetComponent<CommentatorAnimator>().EyeGlowBase = 4f;
            ArenaInterior.CommentatorBox.GetChild(0).GetComponent<CommentatorAnimator>().SetIntensity(0f);
            ArenaInterior.CommentatorBox.GetChild(1).GetComponent<CommentatorAnimator>().EyeGlowBase = 1f;
            ArenaInterior.CommentatorBox.GetChild(1).GetComponent<CommentatorAnimator>().SetIntensity(0f);
            Transform transformBC = UnityEngine.Object.Instantiate<Transform>(Singleton<EnemyFactory>.Instance.Enemies[56].EnemyPrefab.GetComponent<BattleCruiserController>().CharacterModelPrefab.transform);
            foreach (BattleCruiserProjectileWeapon proj in transformBC.GetComponentsInChildren<BattleCruiserProjectileWeapon>())
            {
                UnityEngine.Object.Destroy(proj);
            }
            TransformUtils.HideAllChildren(transformBC);
            transformBC.GetChild(0).gameObject.SetActive(true);
            ArenaInterior.BattleCruiser.GetComponent<MeshRenderer>().enabled = false;
            transformBC.SetParent(ArenaInterior.BattleCruiser, false);
            transformBC.localPosition = new Vector3(0f, 0f, -0.65f);
            transformBC.localEulerAngles = Vector3.zero;
            transformBC.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            ArenaInterior.ArenaTVs.gameObject.SetActive(false);
        }

        /// <summary>
        /// Toggle arena visibility
        /// </summary>
        /// <param name="val"></param>
        public static void SetArenaVisible(bool val)
        {
            LevelManager.Instance.SetPrivateField<bool>("_currentLevelHidesTheArena", val);
            foreach (HideIfLevelHidesArena hide in GameObject.FindObjectsOfType<HideIfLevelHidesArena>())
            {
                hide.CallPrivateMethod("refreshVisibility");
            }
            WorldRoot.Instance.gameObject.SetActive(false);
            AudienceManager.Instance.enabled = val;
        }
        /// <summary>
        /// Toggle Clone drone logo
        /// </summary>
        /// <param name="val"></param>
        public static void SetLogoVisible(bool val)
        {
            ArenaCameraManager.Instance.SetTitleScreenLogoVisible(val);
        }

        /// <summary>
        /// Toggle title screen HUD
        /// </summary>
        /// <param name="val"></param>
        public static void SetRootAndLogoVisible(bool val)
        {
            GameUIRoot.Instance.TitleScreenUI.CallPrivateMethod("setLogoAndRootButtonsVisible", new object[] { val });
        }

        public struct ArenaParts
        {
            public Transform GroundArrows { get; private set; }
            public Transform EditorFloor { get; private set; }
            public Transform Floor { get; private set; }
            public Transform Arena2019 { get; private set; }
            public Transform RemovedAudienceRoot2019 { get; private set; }
            public Transform RemovedAudienceRootOLD { get; private set; }
            public GhostWin1Cutscene Chapter1EndingStuff { get; private set; }
            public Transform ArenaOG { get; private set; }
            public Transform ArenaBrokenup { get; private set; }
            public Transform ArenaMesh { get; private set; }
            public Transform ArenaBanners { get; private set; }
            public Transform CommentatorForcefield { get; private set; }
            public Transform EmperorForcefield { get; private set; }
            public Transform EmperorSection { get; private set; }
            public Transform BattleCruiser { get; private set; }
            public Transform Throne { get; private set; }
            public CharacterModel EmperorModel { get; private set; }
            public Transform StartArea { get; private set; }
            public Transform StartArea2019 { get; private set; }
            public Transform OldStartArea { get; private set; }
            public ArenaLift ArenaLift { get; private set; }
            public Transform UpgradeRoom { get; private set; }
            public Transform ArenaTVs { get; private set; }
            public Transform CommentatorBox { get; private set; }
            public Transform ArenaCameraMain { get; private set; }
            public Transform GarbageShuteAndRoom { get; private set; }
            public Transform GarbageSortingRoom { get; private set; }
            public Transform SawbladeRoom { get; private set; }
            public Transform RobotCity { get; private set; }
            public Transform Sky { get; private set; }
            public FalloutCatcher[] FalloutCatchers { get; private set; }
            public Transform FalloutLaser { get; private set; }

            public ArenaParts GetObjects(Transform original)
            {
                GroundArrows = original.GetChild(0);
                EditorFloor = original.GetChild(1);
                Floor = original.GetChild(2);
                Arena2019 = original.GetChild(3);
                RemovedAudienceRoot2019 = original.GetChild(4);
                ArenaOG = original.GetChild(5);
                EmperorSection = ArenaOG.GetChild(5);
                BattleCruiser = EmperorSection.GetChild(4);
                StartArea = original.GetChild(6);
                UpgradeRoom = original.GetChild(7);
                CommentatorBox = original.GetChild(8);
                ArenaLift = original.GetChild(9).GetComponentInChildren<ArenaLift>();
                Sky = original.GetChild(10);
                ArenaCameraMain = original.GetChild(11);
                GarbageShuteAndRoom = original.GetChild(12);
                GarbageSortingRoom = original.GetChild(13);
                SawbladeRoom = original.GetChild(14);
                RobotCity = original.GetChild(15);
                ArenaTVs = original.GetChild(16);
                RemovedAudienceRootOLD = original.GetChild(17);
                Chapter1EndingStuff = original.GetChild(19).GetComponent<GhostWin1Cutscene>();
                return this;
            }
        }
    }
}