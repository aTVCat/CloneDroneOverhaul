using OverhaulMod.Engine;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationAccessoryReferences : MonoBehaviour
    {
        private List<PersonalizationAccessoryBehaviour> _accessories;

        private bool _isDestroyed;
        private void Start()
        {
            GlobalEventManager.Instance.AddEventListener(CameraManager.FIRST_PERSON_MODE_SWITCHED_EVENT, RefreshVisibility);
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.CinematicCameraTurnedOn, RefreshVisibility);
            GlobalEventManager.Instance.AddEventListener(CameraManager.CINEMATIC_CAMERA_TURNED_OFF_EVENT, RefreshVisibility);
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.EnteredPhotoMode, RefreshVisibility);
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.ExitedPhotoMode, RefreshVisibility);
        }

        private void OnDestroy()
        {
            GlobalEventManager.Instance.RemoveEventListener(CameraManager.FIRST_PERSON_MODE_SWITCHED_EVENT, RefreshVisibility);
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.CinematicCameraTurnedOn, RefreshVisibility);
            GlobalEventManager.Instance.RemoveEventListener(CameraManager.CINEMATIC_CAMERA_TURNED_OFF_EVENT, RefreshVisibility);
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.EnteredPhotoMode, RefreshVisibility);
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.ExitedPhotoMode, RefreshVisibility);
            _isDestroyed = true;
        }

        public void AddAccessory(PersonalizationAccessoryBehaviour accessory)
        {
            if (_isDestroyed)
                return;

            if (_accessories == null)
                _accessories = new List<PersonalizationAccessoryBehaviour>() { accessory };
            else if (!_accessories.Contains(accessory))
                _accessories.Add(accessory);
        }

        public void RemoveAccessory(PersonalizationAccessoryBehaviour accessory)
        {
            if (_accessories == null)
                return;

            _accessories.Remove(accessory);

            if (!_isDestroyed && ShouldDestroy())
                Destroy(this);
        }

        public void RefreshVisibility()
        {
            if (_isDestroyed || _accessories == null || _accessories.Count == 0)
                return;

            for (int i = 0; i < _accessories.Count; i++)
            {
                PersonalizationAccessoryBehaviour accessory = _accessories[i];
                if (accessory)
                    accessory.RefreshVisibility();
            }
        }

        public bool ShouldDestroy()
        {
            return _accessories == null || _accessories.Count == 0;
        }

        public static PersonalizationAccessoryReferences AddReferencesComponent(GameObject obj)
        {
            PersonalizationAccessoryReferences references = obj.GetComponent<PersonalizationAccessoryReferences>();
            if (!references)
            {
                references = obj.AddComponent<PersonalizationAccessoryReferences>();
            }
            return references;
        }
    }
}
