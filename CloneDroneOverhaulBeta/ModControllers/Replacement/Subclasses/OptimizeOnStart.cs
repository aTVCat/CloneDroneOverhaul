using UnityEngine;

namespace CDOverhaul.Patches
{
    public class OptimizeOnStart : ReplacementBase
    {
        public override void Replace()
        {
            base.Replace();

            // Optimise cameras
            Camera[] cameras = Camera.allCameras;
            foreach (Camera cam in cameras)
            {
                if (cam.name == "ArenaCamera")
                {
                    cam.pixelRect = new Rect(new Vector2(0f, 0f), new Vector2(460f, 240f));
                }
            }

            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
