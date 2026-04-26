using OverhaulMod.Visuals;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    /// <summary>
    /// For performing heavy operations like refreshing skins on multiple robots not in a single frame
    /// </summary>
    public class CharacterUpdateScheduler : Singleton<CharacterUpdateScheduler>
    {
        public const float WAIT_BETWEEN_UPDATES = 0.1f;

        private float _timeLeftForNextUpdate;

        private List<CharacterUpdateInfo> _scheduledUpdates;

        public override void Awake()
        {
            base.Awake();
            _scheduledUpdates = new List<CharacterUpdateInfo>();
        }

        private void Update()
        {
            _timeLeftForNextUpdate = Mathf.Max(0, _timeLeftForNextUpdate - Time.deltaTime);
            if (_timeLeftForNextUpdate == 0f)
            {
                _timeLeftForNextUpdate = WAIT_BETWEEN_UPDATES;
                performUpdate();
            }
        }

        public void UpdateCharacter(Character character, CharacterUpdateRequest request)
        {
            if (!character) throw new System.ArgumentNullException(nameof(character), "The character is null or may have been destroyed.");

            if (_scheduledUpdates.Count == 0)
            {
                CharacterUpdateInfo updateInfo = createNewUpdateInfo(character, getImportanceOfUpdate(character), request);
                _scheduledUpdates.Add(updateInfo);
                return;
            }

            CharacterUpdateInfo characterUpdate = getUpdateInfo(character);
            if (characterUpdate == null)
            {
                CharacterUpdateImportance importance = getImportanceOfUpdate(character);
                characterUpdate = createNewUpdateInfo(character, importance, request);
                int index = 0;
                if (importance != CharacterUpdateImportance.MainPlayer) index = getLastIndexOfScheduledUpdateWithImportance(importance);
                _scheduledUpdates.Insert(index, characterUpdate);
            }
            else
            {
                characterUpdate.Request.Append(request);
            }
            characterUpdate.RefreshReferences();
        }

        private void performUpdate()
        {
            if (_scheduledUpdates.Count == 0) return;

            CharacterUpdateInfo update = _scheduledUpdates[0];
            if (update.UpdateWeaponSkins()) return;
            if (update.UpdateWeaponBag()) return;
            if (update.UpdateAccessories()) return;
            if (update.UpdatePets()) return;

            _scheduledUpdates.RemoveAt(0);
        }

        private CharacterUpdateImportance getImportanceOfUpdate(Character character)
        {
            if (character.IsMainPlayer())
            {
                return CharacterUpdateImportance.MainPlayer;
            }
            else if (character.IsPlayer() || character.IsClone())
            {
                return CharacterUpdateImportance.Player;
            }
            else
            {
                return CharacterUpdateImportance.Enemy;
            }
        }

        private int getLastIndexOfScheduledUpdateWithImportance(CharacterUpdateImportance characterUpdateImportance)
        {
            if (_scheduledUpdates.Count == 0 || characterUpdateImportance == CharacterUpdateImportance.MainPlayer) return 0;

            bool foundEntry = false;
            for (int i = 0; i < _scheduledUpdates.Count; i++)
            {
                CharacterUpdateInfo update = _scheduledUpdates[i];
                if (foundEntry)
                {
                    if (update.Importance != characterUpdateImportance)
                    {
                        return i;
                    }
                }
                else if (update.Importance == characterUpdateImportance)
                {
                    foundEntry = true;
                }
            }
            return _scheduledUpdates.Count;
        }

        private CharacterUpdateInfo createNewUpdateInfo(Character character, CharacterUpdateImportance updateImportance, CharacterUpdateRequest request)
        {
            return new CharacterUpdateInfo()
            {
                ReferenceCharacter = character,
                Importance = updateImportance,
                Request = request
            };
        }

        private CharacterUpdateInfo getUpdateInfo(Character character)
        {
            for (int i = 0; i < _scheduledUpdates.Count; i++)
            {
                CharacterUpdateInfo update = _scheduledUpdates[i];
                if (update.ReferenceCharacter == character)
                    return update;
            }
            return null;
        }
    }
}