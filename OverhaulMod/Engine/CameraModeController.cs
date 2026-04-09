using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class CameraModeController : MonoBehaviour
    {
        private CameraManager _cameraManager;

        private FirstPersonMover _owner;

        private PlayerCameraMover _cameraMover;

        private Animator _cameraAnimator;

        private Transform _targetTransform;

        private float _eyesLevel;

        private float _lerp;

        private float _spearLerp;

        public float ForwardVectorMultiplier = 0.25f;

        public Vector3 ShakePositionOffset, ShakeRotationOffset;

        private List<Renderer> _headRenderers;
        public List<Renderer> headRenderers
        {
            get
            {
                if (_headRenderers == null)
                    _headRenderers = _owner.GetRenderersOfBodyPart("Head");

                return _headRenderers;
            }
        }

        private List<Renderer> _jawRenderers;
        public List<Renderer> jawRenderers
        {
            get
            {
                if (_jawRenderers == null)
                    _jawRenderers = _owner.GetRenderersOfBodyPart("Jaw");

                return _jawRenderers;
            }
        }

        private List<Renderer> _shieldRenderers;
        public List<Renderer> shieldRenderers
        {
            get
            {
                if (_shieldRenderers == null)
                    _shieldRenderers = _owner.GetRenderersOfBodyPart("Shield");

                return _shieldRenderers;
            }
        }

        private List<Renderer> _torsoRenderers;
        public List<Renderer> torsoRenderers
        {
            get
            {
                if (_torsoRenderers == null)
                    _torsoRenderers = _owner.GetRenderersOfBodyPart("Torso");

                return _torsoRenderers;
            }
        }

        private List<Renderer> _tieRenderers;
        public List<Renderer> tieRenderers
        {
            get
            {
                if (_tieRenderers == null)
                    _tieRenderers = _owner.GetRenderersOfBodyPart("Tie");

                return _tieRenderers;
            }
        }

        private void Start()
        {
            _lerp = CameraManager.EnableFirstPersonMode ? 0f : 1f;
            _cameraManager = CameraManager.Instance;

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.ConsciousnessTransferStarted, ForceEnableHeadRenderers);
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.ConsciousnessTransferComplete, ForceEnableHeadRenderers);

            if (CameraManager.EnableFirstPersonMode)
                PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        private void OnDestroy()
        {
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.ConsciousnessTransferStarted, ForceEnableHeadRenderers);
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.ConsciousnessTransferComplete, ForceEnableHeadRenderers);
        }

        private void LateUpdate()
        {
            int frameCount = Time.frameCount;
            bool shouldRefreshVsibility = frameCount % 5 == 0;

            FirstPersonMover firstPersonMover = _owner;
            if (IsMindTransferInProgress() || !firstPersonMover || !firstPersonMover.IsAlive())
                return;

            float dt = Time.unscaledDeltaTime;
            bool shouldMoveCameraToHead = CameraManager.EnableFirstPersonMode && !_cameraManager.EnableThirdPerson && !firstPersonMover._isGrabbedForUpgrade && !MultiplayerSpectateManager.Instance.IsInMultiplayerSpectatorMode() && (!GameModeManager.UsesMultiplayerSpawnPoints() || firstPersonMover.HasConstructionFinished());

            _lerp = Mathf.Clamp01(_lerp + (shouldMoveCameraToHead ? -dt : dt) * 3f);
            _spearLerp = Mathf.Clamp01(_spearLerp + (shouldMoveCameraToHead && firstPersonMover.GetEquippedWeaponType() == WeaponType.Spear ? dt : -dt));

            bool shouldCalculateOffset = _lerp < 1f;

            if (shouldRefreshVsibility) RefreshHeadVisibility(_lerp);

            Animator animator = _cameraAnimator;
            if (!_targetTransform || !animator || !animator.enabled)
                return;

            PlayerCameraMover playerCameraMover = _cameraMover;
            if (playerCameraMover)
            {
                if (_lerp > 0f && _lerp < 1f)
                    playerCameraMover._hasCollidedWithEnvironment = true;
                else if (_lerp == 0f)
                    playerCameraMover._hasCollidedWithEnvironment = false;

                playerCameraMover.LateUpdate();
            }

            Vector3 forwardVector = Vector3.zero;
            Vector3 upVector = Vector3.zero;
            if (shouldCalculateOffset)
            {
                forwardVector = _targetTransform.forward * (firstPersonMover._isOnFloorFromKick ? 0f : ForwardVectorMultiplier);
                upVector = _targetTransform.up * _eyesLevel;
            }

            Transform transform = base.transform;
            Vector3 difference = transform.position;
            transform.position = Vector3.Lerp(_targetTransform.position + forwardVector + upVector, difference, NumberUtils.EaseInOutCubic(0f, 1f, _lerp)) + ShakePositionOffset;

            if (shouldCalculateOffset)
            {
                Vector3 localPosition = transform.localPosition;
                localPosition.x = ShakePositionOffset.x;
                localPosition.x += NumberUtils.EaseInOutQuad(0f, 1f, _spearLerp) * -0.35f;
                transform.localPosition = localPosition;
            }
        }

        public void Initialize(CameraManager cameraManager, FirstPersonMover owner, PlayerCameraMover playerCameraMover, Animator animator)
        {
            _owner = owner;
            _cameraMover = playerCameraMover;
            _cameraAnimator = animator;
            _cameraManager = cameraManager;
            RefreshOffset();
        }

        public void SetBodyRenderersActive(bool value)
        {
            setRenderersActive(headRenderers, value);
            setRenderersActive(jawRenderers, value);
            setRenderersActive(shieldRenderers, value);
            setRenderersActive(tieRenderers, value);
            setRenderersActive(torsoRenderers, (_owner ? _owner.IsMindSpaceCharacter : false) || value || _owner._isOnFloorFromKick);
        }

        public bool IsMindTransferInProgress()
        {
            PlayerCameraMover mover = _cameraMover;
            return mover && mover._isConsciousnessTransferInProgress;
        }

        public void ForceEnableHeadRenderers()
        {
            SetBodyRenderersActive(true);
        }

        public void RefreshOffset()
        {
            if (!_targetTransform)
            {
                _targetTransform = getTargetTransform();
            }

            if (!_targetTransform || (_owner && _owner.IsMindSpaceCharacter))
            {
                _eyesLevel = 0.575f;
                return;
            }

            Bounds bounds = new Bounds(_targetTransform.localPosition, Vector3.zero);
            int index = 0;
            foreach (MeshFilter meshFilter in _targetTransform.GetComponentsInChildren<MeshFilter>())
            {
                Mesh mesh = meshFilter.mesh;
                if (mesh)
                {
                    if (index == 0)
                        bounds = mesh.bounds;
                    else
                        bounds.Encapsulate(mesh.bounds);
                }
                index++;
            }

            _eyesLevel = bounds.center.y + (bounds.extents.y * 0.2f);
        }

        public void RefreshHeadVisibility(float lerpValue)
        {
            Animator animator = _cameraAnimator;
            SetBodyRenderersActive(lerpValue > 0.1f || !animator || !animator.enabled || !_owner.IsAlive() || PhotoManager.Instance.IsInPhotoMode());
        }

        private void setRenderersActive(List<Renderer> list, bool value)
        {
            if (list.Count == 0)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                Renderer renderer = list[i];
                if (renderer)
                {
                    renderer.enabled = value;
                }
            }
        }

        private Transform getTargetTransform()
        {
            FirstPersonMover firstPersonMover = _owner;

            Transform targetTransform = null;
            BaseBodyPart headPart = firstPersonMover.IsMindSpaceCharacter ? (BaseBodyPart)firstPersonMover.GetBodyPartParent("Head").GetComponentInChildren<MindSpaceBodyPart>() : firstPersonMover.GetBodyPart(MechBodyPartType.Head);
            if (headPart)
            {
                targetTransform = headPart.transform.parent;
            }

            return targetTransform;
        }
    }
}
