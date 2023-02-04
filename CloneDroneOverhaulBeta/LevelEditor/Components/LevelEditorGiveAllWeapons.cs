using CDOverhaul.Gameplay;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    [RequireComponent(typeof(Collider), typeof(Renderer))]
    public class LevelEditorGiveAllWeapons : MonoBehaviour
    {
        private void Start()
        {
            Collider collider = base.GetComponent<Collider>();
            collider.enabled = GameModeManager.IsInLevelEditor();
            Renderer renderer = base.GetComponent<Renderer>();
            renderer.enabled = GameModeManager.IsInLevelEditor();

            OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.PlayerSetAsFirstPersonMover, giveWeapons);
        }

        private void giveWeapons(FirstPersonMover mover)
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                mover.GetComponent<UpgradeCollection>().AddUpgradeIfMissing(UpgradeType.Hammer, 3);
                mover.GetComponent<UpgradeCollection>().AddUpgradeIfMissing(UpgradeType.BowUnlock, 1);
                mover.GetComponent<UpgradeCollection>().AddUpgradeIfMissing(UpgradeType.SpearUnlock, 1);
                mover.RefreshUpgrades();

            }, 0.2f);
        }

        private void OnDestroy()
        {
            OverhaulEventManager.RemoveEventListener<FirstPersonMover>(MainGameplayController.PlayerSetAsFirstPersonMover, giveWeapons);
        }
    }
}