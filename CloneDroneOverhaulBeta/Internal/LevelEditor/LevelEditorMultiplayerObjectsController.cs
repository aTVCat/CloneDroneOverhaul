using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    public class LevelEditorMultipleObjectsController : OverhaulController
    {
        private bool _isInLevelEditor;

        private readonly Dictionary<ObjectPlacedInLevel, Vector3> _objectsToMove = new Dictionary<ObjectPlacedInLevel, Vector3>();
        private ObjectPlacedInLevel _mainObject;
        private bool _attachedToMainObject;

        public bool AttachedToObjectAndReadyToMove => _isInLevelEditor && _attachedToMainObject && _mainObject != null && _objectsToMove.Count > 1;

        private bool _ignoreNewSelections;

        public static bool ToolEnabled;

        public override void Initialize()
        {
            if (!OverhaulVersion.Upd3JunePreview)
            {
                return;
            }

            _ = OverhaulEventManager.AddEventListener(GlobalEvents.LevelEditorStarted, onLevelEditorStarted, true);
            _ = OverhaulEventManager.AddEventListener(GlobalEvents.LevelEditorSelectionChanged, scheduleOnSelectionChanged, true);
        }

        private void onLevelEditorStarted()
        {
            _isInLevelEditor = true;
        }

        private void scheduleOnSelectionChanged()
        {
            if (!ToolEnabled || _ignoreNewSelections)
            {
                return;
            }
            DelegateScheduler.Instance.Schedule(onSelectionChanged, 0.1f);
        }
        private void onSelectionChanged()
        {
            if (!ToolEnabled || _ignoreNewSelections)
            {
                return;
            }

            List<ObjectPlacedInLevel> selectedSceneObjects = LevelEditorObjectPlacementManager.Instance.GetSelectedSceneObjects();
            if (selectedSceneObjects.Count == 0)
            {
                _attachedToMainObject = false;
                _objectsToMove.Clear();
                _mainObject = null;
            }
            if (selectedSceneObjects.Count < 2)
            {
                return;
            }

            ObjectPlacedInLevel mainObject = selectedSceneObjects[0];
            _mainObject = mainObject;

            _objectsToMove.Clear();
            int index = 1;
            do
            {
                _objectsToMove.Add(selectedSceneObjects[index], mainObject.transform.position - selectedSceneObjects[index].transform.position);
                index++;
            } while (index < selectedSceneObjects.Count);

            _ignoreNewSelections = true;
            LevelEditorObjectPlacementManager.Instance.DeselectEverything();
            LevelEditorObjectPlacementManager.Instance.Select(mainObject);
            _ignoreNewSelections = false;
            _attachedToMainObject = true;
        }

        private void Update()
        {
            if (!_isInLevelEditor)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                ToolEnabled = !ToolEnabled;
            }

            if (AttachedToObjectAndReadyToMove && Time.frameCount % 2 == 0)
            {
                foreach (ObjectPlacedInLevel obj in _objectsToMove.Keys)
                {
                    obj.transform.position = _mainObject.transform.position - _objectsToMove[obj];
                }
            }
        }

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