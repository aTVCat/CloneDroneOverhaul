using CloneDroneOverhaul.V3Tests.Base;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class RobotAdvancedCameraController : MonoBehaviour
    {
        AdvancedCameraController _controller;
        Character _owner;
        Transform _camera;

        Transform _initCamTransformParent;

        Transform _headTransform;

        public RobotAdvancedCameraController Initialize(AdvancedCameraController controller, Character character)
        {
            _controller = controller;
            _owner = character;

            if(_owner != null && _owner.IsAlivePlayer())
            {
                //_headTransform = TransformUtils.FindChildRecursive(_owner.transform, "Head");
                PlayerCameraMover pcm = _owner.GetCameraMover();

                if (pcm != null)
                {
                    _camera = pcm.transform;
                    if (_initCamTransformParent == null) _initCamTransformParent = _camera.parent;
                    if (_headTransform == null)
                    {
                        Transform h = character.GetBodyPartParent("Head");
                        if(h != null)
                        {
                            _headTransform = h.Find("model");
                        }
                    }
                }
            }

            return this;
        }

        public void PatchCharacter(in GameObject cameraGObj, AdvancedCameraType state)
        {
            UpdateModelsAndCamera(state);
        }

        public void UpdateModelsAndCamera(AdvancedCameraType newState)
        {
            if (this._controller == null || this._owner == null || this._camera == null)
            {
                Initialize(AdvancedCameraController.GetInstance<AdvancedCameraController>(), base.GetComponent<Character>());
                if (!this._camera)
                {
                    return;
                }
            }

            if(_headTransform != null)
            {
                bool showHead = newState == AdvancedCameraType.ThirdPerson || newState == AdvancedCameraType.Free || newState == AdvancedCameraType.Side;
                _headTransform.gameObject.SetActive(showHead);
                if (showHead)
                {
                    _camera.SetParent(_initCamTransformParent, false);
                }
                else
                {
                    _camera.SetParent(_headTransform.parent, false);
                    _camera.localPosition = Vector3.zero;
                }

            }
        }
    }
}