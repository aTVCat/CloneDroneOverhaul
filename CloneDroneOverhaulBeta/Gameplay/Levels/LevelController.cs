using ModLibrary;
using System;
using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class LevelController : ModController
    {
        private bool _arenaIsHidden;

        /// <summary>
        /// The state of arena visibility
        /// </summary>
        public bool ArenaIsHidden
        {
            get => _arenaIsHidden || LevelManager.Instance.IsCurrentLevelHidingTheArena();
            set
            {
                _arenaIsHidden = value;
                LevelManager.Instance.SetPrivateField<bool>("_currentLevelHidesTheArena", value);
                SetArenaVisibility(!value);
            }
        }

        private HideIfLevelHidesArena[] _arenaPartsToHide;

        public bool FoundHidableArenaParts => _arenaPartsToHide != null && _arenaPartsToHide.Length > 0;

        public override void Initialize()
        {
            _arenaPartsToHide = FindObjectsOfType<HideIfLevelHidesArena>();

            HasInitialized = true;
        }

        /// <summary>
        /// Deserialize .json file as <see cref="LevelEditorLevelData"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public LevelEditorLevelData GetLevelData(in string path)
        {
            return LevelManager.Instance.LoadLevelEditorLevelData(path);
        }

        /// <summary>
        /// Spawn level using given data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="levelName"></param>
        /// <returns></returns>
        public GameObject SpawnLevel(LevelEditorLevelData data, in string levelName, in bool updateArena = false, Action callback = null)
        {
            GameObject @object = new GameObject(levelName);

            IEnumerator spawnLevelAsync()
            {
                yield return LevelEditorDataManager.Instance.DeserializeInto(@object.transform, data, true);
                if (callback != null)
                {
                    callback();
                }

                yield break;
            }
            _ = StaticCoroutineRunner.StartStaticCoroutine(spawnLevelAsync());

            if (updateArena)
            {
                ArenaIsHidden = data.ArenaIsHidden;
            }

            return @object;
        }

        internal void SetArenaVisibility(in bool value)
        {
            if (!FoundHidableArenaParts)
            {
                return;
            }

            foreach (HideIfLevelHidesArena part in _arenaPartsToHide)
            {
                part.gameObject.SetActive(value);
            }
        }

        /// <summary>
        /// Dispatch <see cref="GlobalEvents.LevelSpawned"/> event
        /// </summary>
        public void DispatchLevelChangedEvent()
        {
            OverhaulEventManager.DispatchEvent(GlobalEvents.LevelSpawned, true);
        }

        /// <summary>
        /// Dispatch <see cref="GlobalEvents.HideArenaToggleChanged"/> event
        /// </summary>
        public void DispatchArenaVisibilityToggleUpdatedEvent()
        {
            OverhaulEventManager.DispatchEvent(GlobalEvents.HideArenaToggleChanged, true);
        }
    }
}