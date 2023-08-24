using OverhaulAPI.SharedMonoBehaviours;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulDebugConsole : OverhaulBehaviour
    {
        public static OverhaulDebugConsole ConsoleInstance;

        private OverhaulUI.PrefabAndContainer m_LogsContainer;

        private readonly List<(string, LogType)> m_Logs = new List<(string, LogType)>(); // 1-log 2-type 3-repeat times

        public static void Initialize()
        {
            if (ConsoleInstance)
                return;

            GameObject prefab = OverhaulAssetsController.GetAsset("OverhaulDebugConsole", "overhaulassets_debug", false);
            GameObject spawnedConsole = Instantiate(prefab);
            _ = spawnedConsole.transform.GetChild(0).gameObject.AddComponent<OverhaulDraggablePanel>();
            DontDestroyOnLoad(spawnedConsole);
            ConsoleInstance = spawnedConsole.AddComponent<OverhaulDebugConsole>();
            ConsoleInstance.m_LogsContainer = new OverhaulUI.PrefabAndContainer(spawnedConsole.GetComponent<ModdedObject>(), 1, 2);
        }

        public override void Awake()
        {
            Application.logMessageReceived += handleLog;
            base.gameObject.SetActive(false);
        }

        protected override void OnDisposed()
        {
            m_Logs.Clear();
            Application.logMessageReceived -= handleLog;
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        private void handleLog(string logString, string stackTrace, LogType type)
        {
            if (!OverhaulVersion.IsDebugBuild)
                return;

            if (!logString.StartsWith(OverhaulDebug.PREFIX))
                return;

            string toLog = logString.Replace(OverhaulDebug.PREFIX, string.Empty);
            (string, LogType) tuple = (toLog, type);
            m_Logs.Add(tuple);
            PopulateLog(tuple);
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
        }

        public void PopulateLog((string, LogType) tuple)
        {
            ModdedObject logDisplay = m_LogsContainer.CreateNew();
            logDisplay.transform.SetAsFirstSibling();
            Text text = logDisplay.GetObject<Text>(0);
            text.text = tuple.Item1;
            switch (tuple.Item2)
            {
                case LogType.Log:
                    text.color = Color.white;
                    break;
                case LogType.Warning:
                    text.color = Color.yellow;
                    break;
                case LogType.Error:
                    text.color = Color.red;
                    break;
            }
        }
    }
}
