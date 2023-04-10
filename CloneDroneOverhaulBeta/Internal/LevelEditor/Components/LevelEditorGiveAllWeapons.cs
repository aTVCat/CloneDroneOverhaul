using CDOverhaul.Gameplay;
using ModLibrary;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    [RequireComponent(typeof(Collider), typeof(Renderer))]
    public class LevelEditorGiveAllWeapons : MonoBehaviour
    {
        [IncludeInLevelEditor(false, false)]
        public bool UnequipAll;

        private void Start()
        {
            Collider collider = base.GetComponent<Collider>();
            collider.enabled = GameModeManager.IsInLevelEditor();
            Renderer renderer = base.GetComponent<Renderer>();
            renderer.enabled = GameModeManager.IsInLevelEditor();

            _ = OverhaulEventsController.AddEventListener<FirstPersonMover>(OverhaulGameplayCoreController.PlayerSetAsFirstPersonMover, giveWeapons);
        }

        private void giveWeapons(FirstPersonMover mover)
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                GiveAllWeapons(mover);

            }, 0.3f);
        }

        public void GiveAllWeaponsToPlayer()
        {
            GiveAllWeapons(CharacterTracker.Instance.GetPlayerRobot());
        }

        public void GiveAllWeapons(in FirstPersonMover mover)
        {
            if (mover == null)
            {
                return;
            }

            if (UnequipAll)
            {
                List<WeaponType> list = mover.GetPrivateField<List<WeaponType>>("_equippedWeapons");
                list.Clear();
                mover.SetPrivateField("_equippedWeapons", list);
                mover.SetEquippedWeaponType(WeaponType.None);
            }
            else
            {
                mover.GetComponent<UpgradeCollection>().AddUpgradeIfMissing(UpgradeType.Hammer, 3);
                mover.GetComponent<UpgradeCollection>().AddUpgradeIfMissing(UpgradeType.BowUnlock, 1);
                mover.GetComponent<UpgradeCollection>().AddUpgradeIfMissing(UpgradeType.SpearUnlock, 1);
            }
            mover.RefreshUpgrades();
        }

        private void OnDestroy()
        {
            OverhaulEventsController.RemoveEventListener<FirstPersonMover>(OverhaulGameplayCoreController.PlayerSetAsFirstPersonMover, giveWeapons);
        }
    }
}