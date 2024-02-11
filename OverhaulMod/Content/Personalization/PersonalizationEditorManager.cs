using OverhaulMod.UI;
using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorManager : Singleton<PersonalizationEditorManager>
    {
        public PersonalizationController editingPersonalizationController
        {
            get;
            set;
        }

        public PersonalizationItemInfo editingItemInfo
        {
            get;
            set;
        }

        public string editingFolder
        {
            get;
            set;
        }

        private string m_editorId;
        public string editorId
        {
            get
            {
                if (!SteamManager.Instance || !SteamManager.Instance.Initialized)
                    return null;

                if(m_editorId == null)
                {
                    m_editorId = SteamUser.GetSteamID().ToString();
                }
                return m_editorId;
            }
        }

        public bool canVerifyItems
        {
            get
            {
                return true;
            }
        }

        public bool canEditNonOwnItems
        {
            get
            {
                return true;
            }
        }

        public void StartEditorGameMode()
        {
            GameFlowManager.Instance._gameMode = (GameMode)2500;

            LevelManager.Instance.CleanUpLevelThisFrame();
            GameFlowManager.Instance.HideTitleScreen(false);

            GameUIRoot.Instance.LoadingScreen.Show();

            GameDataManager.Instance.SaveHighScoreDataWithoutModifyingIt();
            CacheManager.Instance.CreateOrClearInstance();
            SingleplayerServerStarter.Instance.StartServerThenCall(delegate
            {
                UIPersonalizationEditor editorUi = ModUIConstants.ShowPersonalizationEditorUI();

                GameUIRoot.Instance.LoadingScreen.Hide();
                GlobalEventManager.Instance.Dispatch(GlobalEvents.LevelSpawned);

                GameObject cameraObject = Instantiate(PlayerCameraManager.Instance.DefaultGameCameraPrefab.gameObject);
                cameraObject.tag = "MainCamera";
                cameraObject.transform.position = new Vector3(-2.5f, 3f, 3f);
                cameraObject.transform.eulerAngles = new Vector3(5f, 120f, 0f);
                PersonalizationEditorCamera personalizationEditorCamera = cameraObject.AddComponent<PersonalizationEditorCamera>();
                personalizationEditorCamera.ToolBarTransform = editorUi.ToolBarTransform;
                personalizationEditorCamera.LeftPanelTransform = editorUi.LeftPanelTransform;

                LevelEditorLevelData levelEditorLevelData = null;
                try
                {
                    levelEditorLevelData = ModJsonUtils.DeserializeStream<LevelEditorLevelData>(ModCore.dataFolder + "levels/personalizationEditorLevel.json");
                }
                catch
                {
                    LevelManager.Instance._currentLevelHidesTheArena = false;
                }

                base.StartCoroutine(spawnLevelCoroutine(levelEditorLevelData));
            });
        }

        public void EditItem(PersonalizationItemInfo personalizationItemInfo, string folder)
        {
            editingItemInfo = personalizationItemInfo;
            editingFolder = folder;
        }

        public void SaveItem()
        {
            if (editingItemInfo == null)
                return;

            string folder = editingFolder;
            if (folder.IsNullOrEmpty())
                return;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            ModJsonUtils.WriteStream(folder + PersonalizationManager.ITEM_INFO_FILE, editingItemInfo);
        }

        public void SpawnBot()
        {
            base.StartCoroutine(spawnBotCoroutine());
        }

        private IEnumerator spawnBotCoroutine()
        {
            PersonalizationController personalizationController = editingPersonalizationController;
            if (personalizationController)
            {
                Destroy(personalizationController.gameObject);
            }

            GameObject spawnPoint = new GameObject();

            float timeOut = Time.unscaledTime + 5f;
            FirstPersonMover bot = GameFlowManager.Instance.SpawnPlayer(spawnPoint.transform, true, true);
            bot.transform.eulerAngles = Vector3.up * 90f;
            if (bot._playerCamera)
                bot._playerCamera.gameObject.SetActive(false);

            Destroy(spawnPoint);

            yield return new WaitUntil(() => !bot || bot.GetComponent<PersonalizationController>() || Time.unscaledTime >= timeOut);
            if (!bot || Time.unscaledTime >= timeOut)
                yield break;

            personalizationController = bot.GetComponent<PersonalizationController>();
            editingPersonalizationController = personalizationController;
            yield break;
        }

        private IEnumerator spawnLevelCoroutine(LevelEditorLevelData levelEditorLevelData)
        {
            if (levelEditorLevelData != null)
            {
                GameObject level = new GameObject();
                LevelManager.Instance._currentLevelHidesTheArena = true;
                yield return base.StartCoroutine(LevelEditorDataManager.Instance.DeserializeInto(level.transform, levelEditorLevelData));
            }
            else
            {
                LevelManager.Instance._currentLevelHidesTheArena = false;
            }
            GlobalEventManager.Instance.Dispatch(GlobalEvents.LevelSpawned);
            SpawnBot();
            yield break;
        }
    }
}
