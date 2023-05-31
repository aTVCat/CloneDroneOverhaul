using UnityEngine;

namespace CDOverhaul.Patches
{
    public class PlayerCameraPrefabReplacement : ReplacementBase
    {
        private PlayerCameraMover _mover;

        public override void Replace()
        {
            base.Replace();

            Transform transform = PlayerCameraManager.Instance.MechCameraTransformPrefab;
            _mover = transform.GetComponentInChildren<PlayerCameraMover>();
            if (_mover == null)
            {
                SuccessfullyPatched = false;
                return;
            }

            _mover.MinDistanceFromCenter = 1f;
            _mover.PositionYChangePerDistance = -0.1f;
            _mover.ShortenedDistanceAddition = 3f;

            SuccessfullyPatched = true;
        }
    }
}
