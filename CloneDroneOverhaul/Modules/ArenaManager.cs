using System;
using ModLibrary;
using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
	public class ArenaManager : ModuleBase
	{
		public Transform SpawnedCamera { get; private set; }
		public Transform ArenaCamera { get; private set; }
		public Transform ArenaTransform { get; private set; }
        public ArenaManager.ArenaParts ArenaInterior { get; private set; }

		public static void SetArenaVisible(bool val)
		{
			LevelManager.Instance.SetPrivateField<bool>("_currentLevelHidesTheArena", val);
			foreach(HideIfLevelHidesArena hide in GameObject.FindObjectsOfType<HideIfLevelHidesArena>())
			{
				hide.CallPrivateMethod("refreshVisibility");
			}
			WorldRoot.Instance.gameObject.SetActive(false);
			AudienceManager.Instance.enabled = val;
        }
        public static void SetLogoVisible(bool val)
        {
			ArenaCameraManager.Instance.SetTitleScreenLogoVisible(val);
        }

        public override void Start()
        {
			this.ArenaTransform = UnityEngine.Object.FindObjectOfType<HideIfLevelHidesArena>().transform;
            this.ArenaInterior = default(ArenaManager.ArenaParts).GetObjects(this.ArenaTransform);
			spawnCommentatorsDecor();

            ReleaseRenderTextureOnMainMenuExit[] cameras = UnityEngine.Object.FindObjectsOfType<ReleaseRenderTextureOnMainMenuExit>();
			return;
            foreach (ReleaseRenderTextureOnMainMenuExit cam in cameras)
			{
				bool flag2 = cam.gameObject.name == "ArenaCamera";
				if (flag2)
				{
					return;
					this.ArenaCamera = cam.gameObject.transform;
					bool flag3 = false;//FastPrefabs.NewArenaDrone == null;
					if (flag3)
					{
						//FastPrefabs.NewArenaDrone = AssetLoader.GetObjectFromFile("rp_newobjects", "FlyingCameraPrefabFull").transform;
					}
					//this.SpawnedCamera = UnityEngine.Object.Instantiate<Transform>(FastPrefabs.NewArenaDrone, cam.transform, false);
					this.SpawnedCamera.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
					this.SpawnedCamera.transform.localPosition = new Vector3(0f, 0f, -2f);
					break;
				}
			}
		}

		private void spawnCommentatorsDecor()
		{
			this.Microphone = AssetLoader.GetObjectFromFile<GameObject>("rp_newobjects", "MicrophoneModelVox").transform;
			this.Coffee = AssetLoader.GetObjectFromFile<GameObject>("rp_newobjects", "CoffeeModelVox").transform;
			this.Laptop = AssetLoader.GetObjectFromFile<GameObject>("rp_newobjects", "ComputerModelVox").transform;
			this.LotsOfPapers = AssetLoader.GetObjectFromFile<GameObject>("rp_newobjects", "LotsOfPapers").transform;
			Transform trans = UnityEngine.Object.Instantiate<Transform>(this.Microphone, this.ArenaInterior.CommentatorBox);
			trans.localPosition = new Vector3(6f, 4.6f, -0.8f);
			trans.localEulerAngles = new Vector3(0f, 32f, 0f);
			Transform trans2 = UnityEngine.Object.Instantiate<Transform>(this.Coffee, this.ArenaInterior.CommentatorBox);
			trans2.localPosition = new Vector3(6f, 4.6f, -5.6f);
			trans2.localEulerAngles = new Vector3(0f, 282f, 0f);
			Transform trans3 = UnityEngine.Object.Instantiate<Transform>(this.Laptop, this.ArenaInterior.CommentatorBox);
			trans3.localPosition = new Vector3(6f, 4.645f, 3.9f);
			trans3.localEulerAngles = new Vector3(0f, 316f, 0f);
			Transform trans4 = UnityEngine.Object.Instantiate<Transform>(this.LotsOfPapers, this.ArenaInterior.CommentatorBox);
			trans4.localPosition = new Vector3(4.8f, 4.6f, -10f);
			trans4.localEulerAngles = new Vector3(0f, 305f, 0f);
			trans4.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			Transform transCam = TransformUtils.FindChildRecursive(this.ArenaInterior.ArenaCameraMain, "CommentatorTarget");
			transCam.localPosition = new Vector3(-65.35f, 37.4f, 0.27f);
			this.ArenaInterior.CommentatorBox.GetChild(0).GetComponent<CommentatorAnimator>().EyeGlowBase = 4f;
			this.ArenaInterior.CommentatorBox.GetChild(0).GetComponent<CommentatorAnimator>().SetIntensity(0f);
			this.ArenaInterior.CommentatorBox.GetChild(1).GetComponent<CommentatorAnimator>().EyeGlowBase = 1f;
			this.ArenaInterior.CommentatorBox.GetChild(1).GetComponent<CommentatorAnimator>().SetIntensity(0f);
			Transform transformBC = UnityEngine.Object.Instantiate<Transform>(Singleton<EnemyFactory>.Instance.Enemies[56].EnemyPrefab.GetComponent<BattleCruiserController>().CharacterModelPrefab.transform);
			foreach (BattleCruiserProjectileWeapon proj in transformBC.GetComponentsInChildren<BattleCruiserProjectileWeapon>())
			{
				UnityEngine.Object.Destroy(proj);
			}
			TransformUtils.HideAllChildren(transformBC);
			transformBC.GetChild(0).gameObject.SetActive(true);
			this.ArenaInterior.BattleCruiser.GetComponent<MeshRenderer>().enabled = false;
			transformBC.SetParent(this.ArenaInterior.BattleCruiser, false);
			transformBC.localPosition = new Vector3(0f, 0f, -0.65f);
			transformBC.localEulerAngles = Vector3.zero;
			transformBC.localScale = new Vector3(0.35f, 0.35f, 0.35f);
		}

		private Transform Microphone = null;
		private Transform Coffee = null;
		private Transform Laptop = null;
		private Transform LotsOfPapers = null;

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

			public ArenaManager.ArenaParts GetObjects(Transform original)
			{
				this.GroundArrows = original.GetChild(0);
				this.EditorFloor = original.GetChild(1);
				this.Floor = original.GetChild(2);
				this.Arena2019 = original.GetChild(3);
				this.RemovedAudienceRoot2019 = original.GetChild(4);
				this.ArenaOG = original.GetChild(5);
				this.EmperorSection = this.ArenaOG.GetChild(5);
				this.BattleCruiser = this.EmperorSection.GetChild(4);
				this.StartArea = original.GetChild(6);
				this.UpgradeRoom = original.GetChild(7);
				this.CommentatorBox = original.GetChild(8);
				this.ArenaLift = original.GetChild(9).GetComponentInChildren<ArenaLift>();
				this.Sky = original.GetChild(10);
				this.ArenaCameraMain = original.GetChild(11);
				this.GarbageShuteAndRoom = original.GetChild(12);
				this.GarbageSortingRoom = original.GetChild(13);
				this.SawbladeRoom = original.GetChild(14);
				this.RobotCity = original.GetChild(15);
				this.ArenaTVs = original.GetChild(16);
				this.RemovedAudienceRootOLD = original.GetChild(17);
				this.Chapter1EndingStuff = original.GetChild(19).GetComponent<GhostWin1Cutscene>();
				return this;
			}
		}
	}
}
