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

        private GameObject m_JsonEditorGameObject;
        private OverhaulUI.PrefabAndContainer m_JsonEditorValuesContainer;
        private OverhaulUI.PrefabAndContainer m_JsonEditorNewValueButtonContainer;
        private Dropdown m_JsonObjectsDropdown;
        private Button m_NewJsonObjectButton;
        private Button m_SaveJsonObjectButton;
        private GameObject m_NewJsonObjectDialogueGameObject;
        private InputField m_NewJsonObjectName;
        private Dropdown m_NewJsonObjectPresets;
        private Button m_NewJsonObjectDoneButton;

        private Graphic[] m_ThemeLines;
        private Button m_CloseButton;

        private OverhaulJsonObject m_EditingJsonObject;
        private string m_EditingJsonObjectName;

        public override void Initialize()
        {
            m_ProfilerGameObject = MyModdedObject.GetObject<Transform>(0).gameObject;
            _ = m_ProfilerGameObject.AddComponent<OverhaulDraggablePanel>();
            m_ScriptsTimeContainer = new PrefabAndContainer(MyModdedObject, 1, 2);

            m_DebugActionsGameObject = MyModdedObject.GetObject<Transform>(3).gameObject;
            _ = m_DebugActionsGameObject.AddComponent<OverhaulDraggablePanel>();
            m_DebugActionsContainer = new PrefabAndContainer(MyModdedObject, 4, 5);

            m_JsonEditorGameObject = MyModdedObject.GetObject<Transform>(8).gameObject;
            _ = m_JsonEditorGameObject.AddComponent<OverhaulDraggablePanel>();
            m_JsonEditorValuesContainer = new PrefabAndContainer(MyModdedObject, 12, 14);
            m_JsonEditorNewValueButtonContainer = new PrefabAndContainer(MyModdedObject, 13, 14);
            m_JsonObjectsDropdown = MyModdedObject.GetObject<Dropdown>(11);
            m_JsonObjectsDropdown.onValueChanged.AddListener(selectJsonObject);
            m_NewJsonObjectButton = MyModdedObject.GetObject<Button>(9);
            m_NewJsonObjectButton.onClick.AddListener(newJsonObject);
            m_SaveJsonObjectButton = MyModdedObject.GetObject<Button>(10);
            m_SaveJsonObjectButton.onClick.AddListener(saveJsonObject);
            m_NewJsonObjectDialogueGameObject = MyModdedObject.GetObject<Transform>(16).gameObject;
            m_NewJsonObjectDialogueGameObject.SetActive(false);
            m_NewJsonObjectName = MyModdedObject.GetObject<InputField>(17);
            m_NewJsonObjectPresets = MyModdedObject.GetObject<Dropdown>(18);
            m_NewJsonObjectDoneButton = MyModdedObject.GetObject<Button>(19);
            m_NewJsonObjectDoneButton.onClick.AddListener(onDoneCreatingNewJsonObjectFile);

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
            m_JsonEditorGameObject.SetActive(value);
            m_CloseButton.gameObject.SetActive(value);
            ShowCursor = value;

            if (value)
            {
                updateJsonEditor();
            }
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

        #region Json editor stuff

        private void updateJsonEditor()
        {
            m_JsonObjectsDropdown.options = OverhaulJsonEditor.GetAllFileNamesOptionsData(true);
        }

        private void newJsonObject()
        {
            openNewJsonObjectDialogue();
        }

        private void saveJsonObject()
        {
            OverhaulJsonEditor.SaveJsonObject(m_EditingJsonObject, m_EditingJsonObjectName);
            OverhaulJsonObjectPreset preset = new OverhaulJsonObjectPreset(m_EditingJsonObject);
            OverhaulJsonEditor.SaveJsonObjectPreset(preset, m_EditingJsonObjectName);
        }

        private void selectJsonObject(int index)
        {
            m_EditingJsonObjectName = m_JsonObjectsDropdown.options[index].text;
            m_EditingJsonObject = OverhaulJsonEditor.GetJsonObject(m_EditingJsonObjectName);
            populateJsonEditor();
        }

        private void populateJsonEditor()
        {
            m_JsonEditorValuesContainer.ClearContainer();
            if (m_EditingJsonObject == null)
                return;

            foreach (string valueName in m_EditingJsonObject.Values.Keys)
            {
                createJsonEditorValueDisplay(valueName);
            }
            ModdedObject newButton = m_JsonEditorNewValueButtonContainer.CreateNew();
            Button button = newButton.GetComponent<Button>();
            button.onClick.AddListener(delegate
            {
                createJsonEditorValueDisplay("NewValue", true);
            });
        }

        private void createJsonEditorValueDisplay(string valueName, bool createNewKey = false)
        {
            if (createNewKey)
            {
                if (m_EditingJsonObject.Values.ContainsKey(valueName))
                    return;

                m_EditingJsonObject.Values.Add(valueName, string.Empty);
            }

            if (m_EditingJsonObject == null || !m_EditingJsonObject.Values.ContainsKey(valueName))
                return;

            ModdedObject moddedObject = m_JsonEditorValuesContainer.CreateNew();
            moddedObject.GetObject<InputField>(0).text = valueName;
            moddedObject.GetObject<InputField>(1).text = m_EditingJsonObject.Values[valueName];
            OverhaulJsonEditorEntryDisplay entryDisplay = moddedObject.gameObject.AddComponent<OverhaulJsonEditorEntryDisplay>();
            entryDisplay.EditingObject = m_EditingJsonObject;
            entryDisplay.EditingKey = valueName;

            if (createNewKey)
            {
                entryDisplay.transform.SetSiblingIndex(m_JsonEditorValuesContainer.Container.childCount - 2);
            }
        }

        private void openNewJsonObjectDialogue()
        {
            m_NewJsonObjectDialogueGameObject.SetActive(true);
            List<Dropdown.OptionData> data = new List<Dropdown.OptionData>()
            {
                new Dropdown.OptionData("NONE")
            };
            data.AddRange(OverhaulJsonEditor.GetAllFileNamesOptionsData(false));
            m_NewJsonObjectPresets.options = data;
        }

        private void onDoneCreatingNewJsonObjectFile()
        {
            string text = m_NewJsonObjectPresets.options[m_NewJsonObjectPresets.value].text;
            m_NewJsonObjectDialogueGameObject.SetActive(false);
            m_EditingJsonObject = text == "NONE" ? OverhaulJsonEditor.CreateAndSaveJsonObject(m_NewJsonObjectName.text) : OverhaulJsonEditor.CreateAndSaveJsonObjectFromPreset(m_NewJsonObjectName.text, text);
            m_EditingJsonObjectName = m_NewJsonObjectName.text;
            updateJsonEditor();
            populateJsonEditor();
        }

        #endregion
    }
}
