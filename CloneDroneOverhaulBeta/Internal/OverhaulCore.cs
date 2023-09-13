using Bolt;
using CDOverhaul.CustomMultiplayer;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Editors.Personalization;
using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Gameplay.Overmodes;
using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.HUD;
using CDOverhaul.Visuals;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulCore : GlobalEventListener
    {
        [OverhaulSettingAttribute_Old("Gameplay.Multiplayer.Relay Connection", false, false, "This one fixes connection issues, but also increases/decreases ping for some users")]
        public static bool IsRelayConnectionEnabled;

        /// <summary>
        /// The mod directory path.
        /// Ends with '/'
        /// </summary>
        public string ModDirectory => OverhaulMod.Base.ModInfo.FolderPath;
        public static string ModDirectoryStatic => OverhaulMod.Base.ModInfo.FolderPath;

        private List<IGenericStringEventListener> m_GenericStringEventListeners = new List<IGenericStringEventListener>();

        public override void OnEvent(GenericStringForModdingEvent moddedEvent)
        {
            foreach (IGenericStringEventListener listener in m_GenericStringEventListeners)
                if (listener != null)
                    listener.OnGenericStringEvent(moddedEvent);

            if (moddedEvent == null || string.IsNullOrEmpty(moddedEvent.EventData) || !moddedEvent.EventData.StartsWith(OverhaulPlayerInfosSystem.EVENT_PREFIX))
                return;

            string[] split = moddedEvent.EventData.Split('@');
            if (split[1] != OverhaulPlayerInfosSystem.VERSION)
                return;

            OverhaulPlayerInfoRefreshEventData eventData;
            try
            {
                eventData = moddedEvent.BinaryData.DeserializeObject<OverhaulPlayerInfoRefreshEventData>();
            }
            catch
            {
                return;
            }

            // Exceptions
            if (eventData == default)
            {
                OverhaulWebhooks.ExecuteErrorsWebhook("Event data is DEFAULT! Version: " + split[1]);
                return;
            }
            else if (eventData.IsRequest && eventData.IsAnswer)
            {
                OverhaulWebhooks.ExecuteErrorsWebhook("The event is defined as Answer and Request at the same time! Version: " + split[1]);
                return;
            }

            if (eventData.ReceiverPlayFabID == OverhaulPlayerInfoRefreshEventData.RECEIVER_EVERYONE || eventData.ReceiverPlayFabID == OverhaulPlayerIdentifier.GetLocalPlayFabID())
                foreach (OverhaulPlayerInfo overhaulPlayerInfo in OverhaulPlayerInfo.AllOverhaulPlayerInfos)
                    if (overhaulPlayerInfo)
                        overhaulPlayerInfo.OnGenericStringEvent(eventData);
        }

        internal void TryInitialize()
        {
            try
            {
                if (OverhaulMod.Core != null)
                    return;

                OverhaulMod.Core = this;
                _ = OverhaulAPI.OverhaulAPICore.LoadAPI();
                DontDestroyOnLoad(gameObject);

                GameObject controllers = new GameObject("Controllers");
                controllers.transform.SetParent(base.transform);
                OverhaulController.InitializeStatic(controllers);

                ModInitialize modInitialize = new ModInitialize();
                modInitialize.LoadMainFramework();

                if(!UIOverhaulStartupScreen.EnableStartupScreen)
                {
                    loadAssets(modInitialize);
                    return;
                }
                UIOverhaulStartupScreen.Show(modInitialize);
            }
            catch (Exception ex)
            {
                onInitFail(ex.ToString());
            }
        }

        private void onInitFail(string exc)
        {
            OverhaulMod.IsLoadedIncorrectly = true;
            ModdedObject obj = Instantiate(OverhaulAssetLoader.GetAsset<GameObject>("LoadErrorCanvas", OverhaulAssetPart.Main)).GetComponent<ModdedObject>();
            obj.GetObject<Text>(0).text = exc;
            obj.GetObject<Button>(1).onClick.AddListener(delegate
            {
                ErrorManager.Instance._hasCrashed = false;
                TimeManager.Instance.OnGameUnPaused();
                GameUIRoot.Instance.ErrorWindow.Hide();

                Destroy(obj.gameObject);
            });
            _ = EnableCursorController.DisableCursor();
        }

        private void Update()
        {
            OverhaulDebugConsole debugConsole = OverhaulDebugConsole.ConsoleInstance;
            if (!debugConsole)
                return;

            if (Input.GetKeyDown(KeyCode.F6))
            {
                bool isVisible = debugConsole.gameObject.activeSelf;
                if (isVisible)
                {
                    debugConsole.Hide();
                }
                else
                {
                    debugConsole.Show();
                }
            }
        }

        private void loadAssets(ModInitialize modInitialize)
        {
            base.StartCoroutine(modInitialize.LoadAssetsFramework(false));
        }

        public static string ReadText(string filePath)
        {
            if (!File.Exists(filePath))
                return string.Empty;

            string result = string.Empty;
            using (StreamReader r = File.OpenText(filePath))
                result = r.ReadToEnd();

            return result;
        }

        public static bool TryReadText(string filePath, out string content, out Exception exception)
        {
            exception = null;
            content = string.Empty;
            try
            {
                content = ReadText(filePath);
            }
            catch (Exception exc)
            {
                exception = exc;
                return false;
            }
            return true;
        }

        public static async void WriteTextAsync(string filePath, string content, IOStateInfo iOStateInfo = null, bool clearContents = true)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            if (content == null)
                content = string.Empty;

            if (clearContents)
                File.WriteAllText(filePath, string.Empty);

            if (iOStateInfo != null)
                iOStateInfo.IsInProgress = true;

            byte[] toWrite = Encoding.UTF8.GetBytes(content);
            using (FileStream stream = File.OpenWrite(filePath))
            {
                await stream.WriteAsync(toWrite, 0, toWrite.Length);

                if (iOStateInfo != null)
                    iOStateInfo.IsInProgress = false;
            }
        }

        public static void WriteText(string filePath, string content, bool clearContents = true)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            if (content == null)
                content = string.Empty;

            if (clearContents)
                File.WriteAllText(filePath, string.Empty);

            byte[] toWrite = Encoding.UTF8.GetBytes(content);
            using (FileStream stream = File.OpenWrite(filePath))
            {
                stream.Write(toWrite, 0, toWrite.Length);
            }
        }

        public static bool TryWriteText(string filePath, string content, out Exception exception, bool clearContents = true)
        {
            exception = null;
            try
            {
                WriteText(filePath, content, clearContents);
            }
            catch (Exception exc)
            {
                exception = exc;
                return false;
            }
            return true;
        }

        public static bool UnZipFile(string path, string extractDirectory)
        {
            if (!File.Exists(path))
                return false;

            new FastZip().ExtractZip(path, extractDirectory, null);
            return true;
        }

        public class IOStateInfo
        {
            public bool IsInProgress;
        }
    }
}
