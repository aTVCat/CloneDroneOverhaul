using OverhaulMod.Content.Personalization.Objects;
using OverhaulMod.Engine;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationAccessoryBehaviour : MonoBehaviour
    {
        private bool _isBodyPartActive;

        private MechBodyPart _bodyPart;

        private PersonalizationEditorPlacedObject _itemObject;

        private PersonalizationAccessoryReferences _references;

        private void OnDestroy()
        {
            Unregister();
        }

        private void Update()
        {
            bool isBodyPartActive = _bodyPart ? _bodyPart.gameObject.activeSelf : false;
            if (_isBodyPartActive != isBodyPartActive)
            {
                _isBodyPartActive = isBodyPartActive;
                RefreshVisibility();
            } // todo: optimize?
        }

        public void SetBodyPart(MechBodyPart bodyPart)
        {
            _bodyPart = bodyPart;
            _isBodyPartActive = bodyPart && bodyPart.gameObject.activeSelf;
        }

        public void SetItemObject(PersonalizationEditorPlacedObject objectBehaviour)
        {
            _itemObject = objectBehaviour;
        }

        public void Register()
        {
            if (!_bodyPart)
                return;

            PersonalizationAccessoryReferences references = _references ? _references : PersonalizationAccessoryReferences.AddReferencesComponent(_bodyPart.gameObject);
            references.AddAccessory(this);
        }

        public void Unregister()
        {
            if (_references)
                _references.RemoveAccessory(this);
        }

        public void RefreshVisibility()
        {
            if (!_itemObject)
                return;

            MechBodyPart mechBodyPart = _bodyPart;
            if (!mechBodyPart)
            {
                _itemObject.SetChildrenActive(false);
                return;
            }

            Character owner = _bodyPart.GetOwner();
            if (owner)
            {
                if (!owner.IsAlive())
                {
                    _itemObject.SetChildrenActive(false);
                    return;
                }
                else if (owner.IsMainPlayer())
                {
                    PersonalizationItemInfo itemInfo = _itemObject.SpawnInfo?.ItemInfo;
                    if (itemInfo != null && itemInfo.BodyPartName == "Head" && CameraManager.EnableFirstPersonMode && !CameraManager.Instance.IsCameraControlledByCutscene && !PhotoManager.Instance.IsInPhotoMode())
                    {
                        _itemObject.SetChildrenActive(false);
                        return;
                    }
                }
            }
            _itemObject.SetChildrenActive(_isBodyPartActive && mechBodyPart.GetNumDestroyedVoxels() == 0 && !mechBodyPart.HasParentConnectionBeenSevered());
        }
    }
}
