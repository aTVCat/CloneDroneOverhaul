using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    public class LevelEditorMoveObjectsByCoordsController : OverhaulController
    {
        private readonly Dictionary<ObjectPlacedInLevel, Vector3> m_ObjectsAndOffsets = new Dictionary<ObjectPlacedInLevel, Vector3>();
        private ObjectPlacedInLevel m_MainObject;
        public bool HasSelectedMainObject => m_MainObject;
        public bool HasDoneEverythingToMove => GameModeManager.IsInLevelEditor() && HasSelectedMainObject && m_ObjectsAndOffsets.Count > 1;

        private static bool s_ToolEnabled;
        public static bool ToolEnabled
        {
            get => s_ToolEnabled;
            set
            {
                s_ToolEnabled = value;
                if (value)
                {
                    OverhaulFullscreenDialogueWindow.ShowOkWindow("Object movement by coords mode enabled",
                    "Now you can move several objects using coordinates panel in inspector window.",
                    500, 175,
                    OverhaulFullscreenDialogueWindow.IconType.None);
                }
                else
                {
                    OverhaulFullscreenDialogueWindow.ShowOkWindow("Object movement by coords mode disabled",
                    "The usual way of object movement is back.",
                    500, 175,
                    OverhaulFullscreenDialogueWindow.IconType.None);
                }
            }
        }

        private bool m_IgnoreSelections;

        public override void Initialize()
        {
            _ = OverhaulEventsController.AddEventListener(GlobalEvents.LevelEditorSelectionChanged, scheduleOnSelectionChanged, true);
        }

        private void scheduleOnSelectionChanged()
        {
            if (!ToolEnabled || m_IgnoreSelections)
                return;

            DelegateScheduler.Instance.Schedule(onSelectionChanged, 0.1f);
        }
        private void onSelectionChanged()
        {
            if (!ToolEnabled || m_IgnoreSelections)
                return;

            List<ObjectPlacedInLevel> selectedSceneObjects = LevelEditorObjectPlacementManager.Instance.GetSelectedSceneObjects();
            if (selectedSceneObjects.Count == 0)
            {
                m_ObjectsAndOffsets.Clear();
                m_MainObject = null;
            }
            if (selectedSceneObjects.Count < 2)
                return;

            ObjectPlacedInLevel mainObject = selectedSceneObjects[0];
            m_MainObject = mainObject;

            m_ObjectsAndOffsets.Clear();
            int index = 1;
            do
            {
                m_ObjectsAndOffsets.Add(selectedSceneObjects[index], mainObject.transform.position - selectedSceneObjects[index].transform.position);
                index++;
            } while (index < selectedSceneObjects.Count);

            m_IgnoreSelections = true;
            LevelEditorObjectPlacementManager.Instance.DeselectEverything();
            LevelEditorObjectPlacementManager.Instance.Select(mainObject);
            m_IgnoreSelections = false;
        }

        private void Update()
        {
            if (!GameModeManager.IsInLevelEditor())
                return;

            if (HasDoneEverythingToMove)
                foreach (ObjectPlacedInLevel obj in m_ObjectsAndOffsets.Keys)
                    obj.transform.position = m_MainObject.transform.position - m_ObjectsAndOffsets[obj];
        }
    }
}