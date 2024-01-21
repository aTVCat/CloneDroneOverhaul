using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    public class LevelEditorMoveObjectsByCoordsController : OverhaulController
    {
        private Dictionary<ObjectPlacedInLevel, Vector3> m_ObjectsAndOffsets;

        private ObjectPlacedInLevel m_MainObject;

        public bool HasDoneEverythingToMove => GameModeManager.IsInLevelEditor() && m_MainObject && m_ObjectsAndOffsets != null && m_ObjectsAndOffsets.Count >= 2;

        private static bool s_ToolEnabled;
        public static bool ToolEnabled
        {
            get => s_ToolEnabled;
            set
            {
                s_ToolEnabled = value;
                if (value)
                {
                    OverhaulFullscreenDialogueWindow.ShowOkWindow("Advanced objects posing mode enabled",
                    "Now you can move several objects using coordinates panel in inspector window",
                    500, 175,
                    OverhaulFullscreenDialogueWindow.IconType.None);
                }
                else
                {
                    OverhaulFullscreenDialogueWindow.ShowOkWindow("Advanced objects posing mode disabled",
                    string.Empty,
                    500, 175,
                    OverhaulFullscreenDialogueWindow.IconType.None);
                }
            }
        }

        private bool m_IgnoreSelections;

        public override void Initialize()
        {
            OverhaulEvents.AddEventListener(GlobalEvents.LevelEditorSelectionChanged, onSelectionChanged, true);
        }

        private void onSelectionChanged()
        {
            if (!ToolEnabled || m_IgnoreSelections)
                return;

            if (m_ObjectsAndOffsets == null)
                m_ObjectsAndOffsets = new Dictionary<ObjectPlacedInLevel, Vector3>();

            List<ObjectPlacedInLevel> selectedSceneObjects = LevelEditorObjectPlacementManager.Instance?.GetSelectedSceneObjects();
            if (selectedSceneObjects == null || selectedSceneObjects.Count < 2)
            {
                m_ObjectsAndOffsets.Clear();
                m_MainObject = null;
                return;
            }

            ObjectPlacedInLevel mainObject = selectedSceneObjects[0];
            if (!mainObject)
                return;

            m_MainObject = mainObject;

            m_ObjectsAndOffsets.Clear();
            int index = 1;
            do
            {
                var anObj = selectedSceneObjects[index];
                if (!anObj)
                {
                    index++;
                    continue;
                }

                m_ObjectsAndOffsets.Add(anObj, mainObject.transform.position - anObj.transform.position);
                index++;
            } while (index < selectedSceneObjects.Count);

            m_IgnoreSelections = true;
            LevelEditorObjectPlacementManager.Instance.DeselectEverything();
            LevelEditorObjectPlacementManager.Instance.Select(mainObject);
            m_IgnoreSelections = false;
        }

        private void LateUpdate()
        {
            if (!GameModeManager.IsInLevelEditor())
                return;

            if (HasDoneEverythingToMove)
                foreach (ObjectPlacedInLevel obj in m_ObjectsAndOffsets.Keys)
                {
                    if (!obj)
                        continue;

                    obj.transform.position = m_MainObject.transform.position - m_ObjectsAndOffsets[obj];
                }
        }
    }
}