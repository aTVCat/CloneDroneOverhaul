using OverhaulMod.Engine;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationAccessoryReferences : MonoBehaviour
    {
        private List<PersonalizationAccessoryBehaviour> m_accessories;

        private bool m_isDestroyed;
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
            m_isDestroyed = true;
        }

        public void AddAccessory(PersonalizationAccessoryBehaviour accessory)
        {
            if (m_isDestroyed)
                return;

            if (m_accessories == null)
                m_accessories = new List<PersonalizationAccessoryBehaviour>() { accessory };
            else if (!m_accessories.Contains(accessory))
                m_accessories.Add(accessory);
        }

        public void RemoveAccessory(PersonalizationAccessoryBehaviour accessory)
        {
            if (m_accessories == null)
                return;

            m_accessories.Remove(accessory);

            if (!m_isDestroyed && ShouldDestroy())
                Destroy(this);
        }

        public void RefreshVisibility()
        {
            if (m_isDestroyed || m_accessories == null || m_accessories.Count == 0)
                return;

            for (int i = 0; i < m_accessories.Count; i++)
            {
                PersonalizationAccessoryBehaviour accessory = m_accessories[i];
                if (accessory)
                    accessory.RefreshVisibility();
            }
        }

        public bool ShouldDestroy()
        {
            return m_accessories == null || m_accessories.Count == 0;
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
