using CDOverhaul.Gameplay.Combat;
using CDOverhaul.HUD;
using OverhaulAPI.SharedMonoBehaviours;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.DevTools
{
    public class OverhaulDevToolsUI : OverhaulUI
    {
        public static bool IsUIActive;

        private GameObject m_ProfilerGameObject;
        private OverhaulUI.PrefabAndContainer m_ScriptsTimeContainer;

        private GameObject m_DebugActionsGameObject;
        private OverhaulUI.PrefabAndContainer m_DebugActionsContainer;

        private Graphic[] m_ThemeLines;
        private Button m_CloseButton;

        public override void Initialize()
        {
            m_ProfilerGameObject = MyModdedObject.GetObject<Transform>(0).gameObject;
            _ = m_ProfilerGameObject.AddComponent<OverhaulDraggablePanel>();
            m_ScriptsTimeContainer = new PrefabAndContainer(MyModdedObject, 1, 2);

            m_DebugActionsGameObject = MyModdedObject.GetObject<Transform>(3).gameObject;
            _ = m_DebugActionsGameObject.AddComponent<OverhaulDraggablePanel>();
            m_DebugActionsContainer = new PrefabAndContainer(MyModdedObject, 4, 5);

            m_ThemeLines = new Graphic[2];
            m_ThemeLines[0] = MyModdedObject.GetObject<Graphic>(6);
            m_ThemeLines[1] = MyModdedObject.GetObject<Graphic>(7);
            m_CloseButton = MyModdedObject.GetObject<Button>(15);
            m_CloseButton.onClick.AddListener(delegate
            {
                SetEverythingActive(false);
            });

            SetEverythingActive(false);
        }

        private void Update()
        {
            if (!OverhaulVersion.IsDebugBuild)
                return;

            if (!IsUIActive && Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (ErrorManager.Instance.HasCrashed() || !GameModeManager.IsInLevelEditor())
                    SetEverythingActive(true);
            }

            if (IsUIActive)
            {
                updateProfiler();
                updateDebugActions();
            }
        }

        public void SetEverythingActive(bool value)
        {
            IsUIActive = value;

            foreach (Graphic graphic in m_ThemeLines)
            {
                if (graphic)
                {
                    graphic.gameObject.SetActive(value);
                    graphic.color = OverhaulCombatState.GetUIThemeColor(ParametersMenu.DefaultBarColor);
                }
            }

            m_ProfilerGameObject.SetActive(value);
            m_DebugActionsGameObject.SetActive(value);
            m_CloseButton.gameObject.SetActive(value);
            ShowCursor = value;
        }

        #region Profiler stuff

        private void updateProfiler()
        {
            int all = m_ScriptsTimeContainer.Container.childCount;
            int target = OverhaulProfiler.GetEntriesCount();

            if (all != target)
            {
                string[] entries = new string[OverhaulProfiler.GetEntriesCount()];
                int i = 0;
                do
                {
                    entries[i] = OverhaulProfiler.GetEntry(i);
                    i++;
                } while (i < target);

                m_ScriptsTimeContainer.ClearContainer();
                foreach (string str in entries)
                {
                    ModdedObject moddedObject = m_ScriptsTimeContainer.CreateNew();
                    OverhaulProfilerEntryDisplay entryDisplay = moddedObject.gameObject.AddComponent<OverhaulProfilerEntryDisplay>();
                    entryDisplay.MyEntry = str;
                }
            }
        }

        #endregion

        #region Debg actions stuff

        private void updateDebugActions()
        {
            if (m_DebugActionsContainer.Container.childCount != 0)
                return;

            List<Tuple<string, MethodInfo>> tuples = OverhaulDebugActions.GetAllDebugActions();
            foreach (Tuple<string, MethodInfo> tuple in tuples)
            {
                ModdedObject moddedObject = m_DebugActionsContainer.CreateNew();
                OverhaulDebugActionsEntryDisplay entryDisplay = moddedObject.gameObject.AddComponent<OverhaulDebugActionsEntryDisplay>();
                entryDisplay.MyEntry = tuple.Item1;
                entryDisplay.MyMethod = tuple.Item2;
            }
        }

        #endregion
    }
}
