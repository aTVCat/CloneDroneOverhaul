using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulCrashPreventionController
    {
        public static bool dataIsNUllError;
        public static bool couldntAllocateError;

        public static bool TryPreventCrash(string crashString)
        {
            if (string.IsNullOrEmpty(crashString))
                return false;

            if (crashString.Contains("Screen position out of view frustum"))
            {
                FixScreenPosition();
                return true;
            }
            if (crashString.Contains("JoinSessionRoutine"))
            {
                ShutdownBoltIfRunning();
                return true;
            }
            return false;
        }

        public static void FixScreenPosition()
        {
            if (GameModeManager.IsMultiplayer())
            {
                BoltLauncher.Shutdown();
                return;
            }
            if (GameModeManager.IsSinglePlayer())
            {
                if (PhotoManager.Instance.IsInPhotoMode())
                {
                    Camera camera = Camera.main;
                    if (!camera)
                    {
                        camera = Camera.current;
                        if (!camera)
                            return;
                    }

                    camera.transform.position = Vector3.zero;
                    return;
                }

                Character character = CharacterTracker.Instance.GetPlayer();
                if (!character)
                {
                    /*
                    LevelEditorCameraController cameraController = LevelEditorCameraManager.Instance.GetPrivateField<LevelEditorCameraController>("_cameraController");
                    if (!cameraController)
                        return;

                    cameraController.transform.position = Vector3.zero;
                    cameraController.transform.eulerAngles = cameraController.StartRotation;*/
                    return;
                }

                character.transform.position = new Vector3(0, 1, 0);
                return;
            }
        }

        public static void ShutdownBoltIfRunning()
        {
            if (BoltNetwork.IsRunning)
                BoltLauncher.Shutdown();
        }
    }
}
