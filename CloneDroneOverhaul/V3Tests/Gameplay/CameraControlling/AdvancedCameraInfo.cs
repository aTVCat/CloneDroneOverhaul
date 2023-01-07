using UnityEngine;
using ModLibrary;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public class RobotAdvancedCameraController : MonoBehaviour
    {
        public static readonly Vector3 CameraTargetPosition = new Vector3(0, 0.4f, 0);

        private AdvancedCameraController _controller;
        private Character _owner;
        private Transform _camera;
        private Transform _initCamTransformParent;
        private Transform _headTransform;

        private bool _hasAddedListeners;

        public RobotAdvancedCameraController Initialize(AdvancedCameraController controller, Character character)
        {
            _controller = controller;
            _owner = character;

            if (_owner != null)
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

                if (!_hasAddedListeners)
                {
                    _hasAddedListeners = true;
                    Singleton<GlobalEventManager>.Instance.AddEventListener("EnteredPhotoMode", JustShowModels);
                    Singleton<GlobalEventManager>.Instance.AddEventListener("ExitedPhotoMode", JustHideModels);
                }
            }

            return this;
        }

        void OnDestroy()
        {
            Singleton<GlobalEventManager>.Instance.RemoveEventListener("EnteredPhotoMode", JustShowModels);
            Singleton<GlobalEventManager>.Instance.RemoveEventListener("ExitedPhotoMode", JustHideModels);
        }

        void LateUpdate()
        {
            if (_camera != null && _headTransform != null && _camera.parent == _headTransform.parent)
            {
                _camera.localPosition = CameraTargetPosition;
            }
        }

        public bool FoundHead()
        {
            return _headTransform != null;
        }

        /// <summary>
        /// Use for event listeners
        /// </summary>
        public void JustShowModels()
        {
            ShowModels(null, true);
        }

        /// <summary>
        /// Use for event listeners
        /// </summary>
        public void JustHideModels()
        {
            HideModels(null);
        }

        /// <summary>
        /// There's no need to attach cinematic camera instance to <paramref name="c"/>, just leave it null
        /// </summary>
        public void ShowModels(LevelEditorCinematicCamera c, bool dontManipulateWithCamera)
        {
            UpdateModelsAndCamera(true, !dontManipulateWithCamera);
        }

        /// <summary>
        /// There's no need to attach cinematic camera instance to <paramref name="c"/>, just leave it null
        /// </summary>
        public void HideModels(LevelEditorCinematicCamera c)
        {
            if(_owner == null || _owner.GetCameraMover() == null)
            {
                return;
            }
            PatchCharacter(_owner.GetCameraMover().gameObject, _controller.CurrentCameraMode);
        }

        public void PatchCharacter(in GameObject cameraGObj, EAdvancedCameraType state)
        {
            UpdateModelsAndCamera(state == EAdvancedCameraType.ThirdPerson || state == EAdvancedCameraType.Side);
        }

        public void UpdateModelsAndCamera(bool showHead, bool updateCamera = true)
        {
            if (!OverhaulDescription.TEST_FEATURES_ENABLED)
            {
                return;
            }

            // Fix missing fields if ones are null
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
                Initialize(_controller, _owner);
            }

            if (_headTransform != null)
            {
                _headTransform.gameObject.SetActive(showHead);
                if (showHead)
                {
                    if (updateCamera)
                    {
                        _camera.SetParent(_initCamTransformParent, false);
                    }
                    _owner.SetPrivateField<Transform>("_playerCameraParent", _initCamTransformParent);
                }
                else
                {
                    if (updateCamera)
                    {
                        _camera.SetParent(_headTransform.parent, false);
                    }
                    _owner.SetPrivateField<Transform>("_playerCameraParent", _headTransform.parent);
                }
            }
        }
    }
}