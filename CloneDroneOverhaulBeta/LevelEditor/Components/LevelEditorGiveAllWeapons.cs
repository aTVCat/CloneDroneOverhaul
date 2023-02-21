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

            _ = OverhaulEventManager.AddEventListener<FirstPersonMover>(MainGameplayController.PlayerSetAsFirstPersonMover, giveWeapons);
        }

        private void giveWeapons(FirstPersonMover mover)
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                GiveAllWeapons(mover);

            }, 0.2f);
        }

        public static void GiveAllWeaponsToPlayer()
        {
            GiveAllWeapons(CharacterTracker.Instance.GetPlayerRobot());
        }

        public static void GiveAllWeapons(in FirstPersonMover mover)
        {
            if (mover == null)
            {
                return;
            }

            mover.GetComponent<UpgradeCollection>().AddUpgradeIfMissing(UpgradeType.Hammer, 3);
            mover.GetComponent<UpgradeCollection>().AddUpgradeIfMissing(UpgradeType.BowUnlock, 1);
            mover.GetComponent<UpgradeCollection>().AddUpgradeIfMissing(UpgradeType.SpearUnlock, 1);
            mover.RefreshUpgrades();
        }

        private void OnDestroy()
        {
            OverhaulEventManager.RemoveEventListener<FirstPersonMover>(MainGameplayController.PlayerSetAsFirstPersonMover, giveWeapons);
        }
    }
}