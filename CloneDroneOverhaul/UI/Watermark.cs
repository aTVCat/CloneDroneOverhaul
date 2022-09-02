using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class Watermark : ModGUIBase
    {
        private Text WatermarkText;
        private Button ChangelogButton;
        private Button LEButton;

        private Text ChangelogText;
        private Text LEText;

        public override void OnAdded()
        {
            base.MyModdedObject = base.GetComponent<ModdedObject>();
            WatermarkText = MyModdedObject.GetObjectFromList<Text>(0);
            ChangelogButton = MyModdedObject.GetObjectFromList<Button>(1);
            ChangelogText = MyModdedObject.GetObjectFromList<Text>(2);
            LEButton = MyModdedObject.GetObjectFromList<Button>(3);
            LEButton.onClick.AddListener(new UnityEngine.Events.UnityAction(TryShowLocalEditor));
            LEText = MyModdedObject.GetObjectFromList<Text>(4);

            OverhaulMain.Timer.AddNoArgActionToCompleteNextFrame(AddListeners);
        }

        public override void OnManagedUpdate()
        {
            ChangelogButton.gameObject.SetActive(GameModeManager.IsOnTitleScreen());
            LEButton.gameObject.SetActive(GameModeManager.IsOnTitleScreen());
            WatermarkText.text = OverhaulDescription.GetModName(true, !GameModeManager.IsOnTitleScreen());
        }

        public override void OnNewFrame()
        {
            WatermarkText.gameObject.SetActive(!PhotoManager.Instance.IsInPhotoMode());
        }

        private void OnLangChanged()
        {
            ChangelogText.text = OverhaulMain.GetTranslatedString("UpdateNotes");
        }

        private void TryShowLocalEditor()
        {
            GUIModule.GetGUI<Localization.OverhaulLocalizationEditor>().TryShow();
        }
        private void AddListeners()
        {
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.UILanguageChanged, OnLangChanged);
            OnLangChanged();
        }
    }
}
