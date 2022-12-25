using System.Collections;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Optimisation
{
    public static class OptimiseOnStartup
    {
        public static Camera ArenaCamera;
        public static Camera TitleScreenLevelCamera;
        public static Camera TitleScreenLogoCamera;

        public static void Initialize()
        {
            Camera[] cameras = Modules.GameInformationManager.UnoptimizedThings.GetFPSLoweringStuff().AllCameras;
            foreach(Camera cam in cameras)
            {
                if(cam.name == "TitleScreenLogoCamera")
                {
                    TitleScreenLogoCamera = cam;
                }
                else if (cam.name == "ArenaCamera")
                {
                    ArenaCamera = cam;
                    if(ArenaCamera != null)
                    {
                        ArenaCamera.enabled = false;
                        ArenaCamera.pixelRect = new Rect(new Vector2(0f, 0f), new Vector2(460f, 240f)); //ArenaCamera.pixelRect = new Rect(new Vector2(0f, 0f), new Vector2(640f, 360f));
                    }
                }
                else if (cam.name == "TitleScreenLevelCamera")
                {
                    TitleScreenLevelCamera = cam;
                }
            }
        }

        public static void SetArenaCameraEnabled()
        {
            if (ArenaCamera != null) ArenaCamera.enabled = true;
        }
    }
}