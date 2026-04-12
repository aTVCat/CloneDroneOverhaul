using OverhaulMod.Content.Personalization;
using OverhaulMod.Visuals;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    /// <summary>
    /// For performing heavy operations like skins refreshing on multiple robots not in a single frame
    /// </summary>
    public class CharacterUpdateScheduler : Singleton<CharacterUpdateScheduler>
    {
        public const float MINIMAL_DELAY = 0.1f;

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
                _timeLeftForNextUpdate = MINIMAL_DELAY;
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
                int index = getLastIndexOfScheduledUpdateWithImportance(importance);
                _scheduledUpdates.Insert(index, characterUpdate);
            }
            else
            {
                characterUpdate.Request.Append(request);
            }
        }

        private void performUpdate()
        {
            if (_scheduledUpdates.Count == 0) return;

            CharacterUpdateInfo update = _scheduledUpdates[0];
            if (!update.UpdateWeaponSkins())
            {
                if (!update.UpdateWeaponBag())
                {
                    if (!update.UpdateAccessories())
                    {
                        if (!update.UpdatePets())
                        {
                            _scheduledUpdates.RemoveAt(0);
                        }
                    }
                }
            }
        }

        private CharacterUpdateImportance getImportanceOfUpdate(Character character)
        {
            if (character.IsMainPlayer())
            {
                return CharacterUpdateImportance.Player;
            }
            else if (character.IsPlayer() || character.IsClone())
            {
                return CharacterUpdateImportance.PlayerClone;
            }
            else
            {
                return CharacterUpdateImportance.Enemy;
            }
        }

        private int getLastIndexOfScheduledUpdateWithImportance(CharacterUpdateImportance characterUpdateImportance)
        {
            if (_scheduledUpdates.Count == 0 || characterUpdateImportance == CharacterUpdateImportance.Player) return 0;

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
                PersonalizationController = character.GetComponent<PersonalizationController>(),
                WeaponBag = character.GetComponent<RobotWeaponBag>(),
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