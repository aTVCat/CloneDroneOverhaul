using UnityEngine;

namespace CDOverhaul.Patches
{
    public class PlayerCameraPrefabReplacement : ReplacementBase
    {
        public override void Replace()
        {
            base.Replace();

            Transform transform = PlayerCameraManager.Instance.MechCameraTransformPrefab;
            if (transform)
            {
                PlayerCameraMover playerCameraMover = transform.GetComponentInChildren<PlayerCameraMover>();
                if (playerCameraMover)
                {
                    playerCameraMover.MinDistanceFromCenter = 1f;
                    playerCameraMover.PositionYChangePerDistance = -0.1f;
                    playerCameraMover.ShortenedDistanceAddition = 3f;
                }
            }
            SuccessfullyPatched = true;
        }
    }
}
