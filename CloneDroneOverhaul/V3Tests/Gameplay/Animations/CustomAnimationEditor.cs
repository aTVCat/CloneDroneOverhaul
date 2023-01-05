using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay.Animations
{
    public class CustomAnimationEditor : Interfaces.IEditor
    {
        public void EnterEditor()
        {
            GameFlowManager.Instance.SetGameMode(GetGamemode());
            ArenaController.SetArenaVisible(false);
            ArenaController.SetRootAndLogoVisible(false);
            LevelManager.Instance.CleanUpLevelThisFrame();

            GameObject obj = new GameObject("CustomAnimationEditor");

            Transform fpm = GameObject.Instantiate<Transform>(EnemyFactory.Instance.GetEnemyPrefab(EnemyType.PlayerDummy).transform);
            DelegateScheduler.Instance.Schedule(delegate
            {
                foreach (MonoBehaviour b in fpm.GetComponentsInChildren<MonoBehaviour>())
                {
                    b.enabled = false;
                }
                fpm.transform.position = Vector3.zero;
                fpm.transform.SetParent(obj.transform);
            }, 0.1f);

            GameObject cam = new GameObject("Camera");
            cam.AddComponent<CustomAnimationEditorCamera>();
            cam.AddComponent<Camera>();
            cam.transform.SetParent(obj.transform);
        }

        public GameMode GetGamemode()
        {
            return (GameMode)29299;
        }
    }
}
