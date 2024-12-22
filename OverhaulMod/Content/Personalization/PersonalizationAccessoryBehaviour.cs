using OverhaulMod.Engine;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationAccessoryBehaviour : MonoBehaviour
    {
        private bool m_isBodyPartActive;

        private MechBodyPart m_bodyPart;

        private PersonalizationEditorObjectBehaviour m_itemObject;

        private PersonalizationAccessoryReferences m_references;

        private void OnDestroy()
        {
            Unregister();
        }

        private void Update()
        {
            bool isBodyPartActive = m_bodyPart ? m_bodyPart.gameObject.activeSelf : false;
            if (m_isBodyPartActive != isBodyPartActive)
            {
                m_isBodyPartActive = isBodyPartActive;
                RefreshVisibility();
            } // todo: optimize?
        }

        public void SetBodyPart(MechBodyPart bodyPart)
        {
            m_bodyPart = bodyPart;
            m_isBodyPartActive = bodyPart && bodyPart.gameObject.activeSelf;
        }

        public void SetItemObject(PersonalizationEditorObjectBehaviour objectBehaviour)
        {
            m_itemObject = objectBehaviour;
        }

        public void Register()
        {
            if (!m_bodyPart)
                return;

            PersonalizationAccessoryReferences references = m_references ? m_references : PersonalizationAccessoryReferences.AddReferencesComponent(m_bodyPart.gameObject);
            references.AddAccessory(this);
        }

        public void Unregister()
        {
            if (m_references)
                m_references.RemoveAccessory(this);
        }

        public void RefreshVisibility()
        {
            if (!m_itemObject)
                return;

            MechBodyPart mechBodyPart = m_bodyPart;
            if (!mechBodyPart)
            {
                m_itemObject.SetChildrenActive(false);
                return;
            }

            Character owner = m_bodyPart.GetOwner();
            if (owner && !owner.IsAlive())
            {
                m_itemObject.SetChildrenActive(false);
                return;
            }

            if (owner.IsMainPlayer())
            {
                PersonalizationItemInfo itemInfo = m_itemObject.ControllerInfo?.ItemInfo;
                if (itemInfo != null && itemInfo.BodyPartName == "Head" && CameraManager.EnableFirstPersonMode && !CameraManager.Instance.isCameraControlledByCutscene && !PhotoManager.Instance.IsInPhotoMode())
                {
                    m_itemObject.SetChildrenActive(false);
                    return;
                }
            }
            m_itemObject.SetChildrenActive(m_isBodyPartActive && m_bodyPart.GetNumDestroyedVoxels() == 0 && !m_bodyPart.HasParentConnectionBeenSevered());
        }
    }
}
