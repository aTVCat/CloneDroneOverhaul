using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class RobotAdvancedCameraController : MonoBehaviour
    {
        private AdvancedCameraController _controller;
        private Character _owner;
        private Transform _camera;
        private Transform _initCamTransformParent;
        private Transform _headTransform;

        public RobotAdvancedCameraController Initialize(AdvancedCameraController controller, Character character)
        {
            _controller = controller;
            _owner = character;

            if (_owner != null && _owner.IsAlivePlayer())
            {
                //_headTransform = TransformUtils.FindChildRecursive(_owner.transform, "Head");
                PlayerCameraMover pcm = _owner.GetCameraMover();

                if (pcm != null)
                {
                    _camera = pcm.transform;
                    if (_initCamTransformParent == null)
                    {
                        _initCamTransformParent = _camera.parent;
                    }

                    if (_headTransform == null)
                    {
                        Transform h = character.GetBodyPartParent("Head");
                        if (h != null)
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
            if (!OverhaulDescription.TEST_FEATURES_ENABLED)
            {
                return;
            }

            if (_controller == null || _owner == null || _camera == null)
            {
                Initialize(AdvancedCameraController.GetInstance<AdvancedCameraController>(), base.GetComponent<Character>());
                if (!_camera)
                {
                    return;
                }
            }

            if (_headTransform != null)
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