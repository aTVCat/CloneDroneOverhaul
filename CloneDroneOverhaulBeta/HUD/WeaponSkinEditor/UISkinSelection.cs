using CDOverhaul.Gameplay;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UISkinSelection : UIBase
    {
        public override void Initialize()
        {
            OverhaulEventManager.AddListenerToEvent(GamemodeSubstatesController.SubstateChangedEventString, onGamemodeSubstateUpdated);

            MyModdedObject.GetObject<Button>(3).onClick.AddListener(tryWeaponOut);

            setActive(false);

            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        private void setActive(in bool value)
        {
            base.gameObject.SetActive(value);

            if (value)
            {
                EnableCursorConditionID = EnableCursorController.AddCondition();
            }
            else
            {
                EnableCursorController.RemoveCondition(EnableCursorConditionID);
            }
        }

        private void onGamemodeSubstateUpdated()
        {
            setActive(MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate == EGamemodeSubstate.WeaponSkinSelection);
        }

        private void tryWeaponOut()
        {
            MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate = EGamemodeSubstate.None;
        }
    }
}