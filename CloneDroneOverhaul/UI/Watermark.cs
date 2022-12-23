using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class Watermark : ModGUIBase
    {
        private TextMeshProUGUI _version;
        private Button _changelog;
        private Button _aboutModButton;
        private Button _localizationEditor;
        private RectTransform _buttonContainer;

        private bool _hasInitializedAboutWindow;
        private bool _showVersion;

        public override void OnInstanceStart()
        {
            base.MyModdedObject = base.GetComponent<ModdedObject>();

            _version = MyModdedObject.GetObjectFromList<TextMeshProUGUI>(0);
            _buttonContainer = MyModdedObject.GetObjectFromList<RectTransform>(1);
            _changelog = MyModdedObject.GetObjectFromList<Button>(3);
            _changelog.onClick.AddListener(openChangelog);
            _localizationEditor = MyModdedObject.GetObjectFromList<Button>(4);
            _localizationEditor.onClick.AddListener(openLocalizationEditor);
            _aboutModButton = MyModdedObject.GetObjectFromList<Button>(5);
            _aboutModButton.onClick.AddListener(openAboutWindow);

            refreshVersion();

            _localizationEditor.gameObject.SetActive(OverhaulDescription.TEST_FEATURES_ENABLED);
        }
        public override void OnNewFrame()
        {
            _version.gameObject.SetActive(_showVersion && !PhotoManager.Instance.IsInPhotoMode() && !CutSceneManager.Instance.IsInCutscene() && !Modules.MiscEffectsManager.IsUIHidden);
        }
        public override void RunFunction<T>(string name, T obj)
        {
            if (name == "onGameModeUpdated")
            {
                refreshVersion();
                _buttonContainer.gameObject.SetActive(GameModeManager.IsOnTitleScreen());
            }
        }
        public override void RunFunction(string name, object[] arguments)
        {
            if (name == "onLanguageChanged")
            {
                MyModdedObject.GetObjectFromList<Text>(6).text = OverhaulMain.GetTranslatedString("Watermark_AboutOverhaul");
            }
        }
        public override void OnSettingRefreshed(string ID, object value, bool isRefreshedOnStart = false)
        {
            if (ID == "Misc.Mod.Show version")
            {
                _showVersion = (bool)value;
            }
        }

        private void refreshText()
        {

        }

        private void refreshVersion()
        {
            _version.text = OverhaulDescription.GetModName(true, !GameModeManager.IsOnTitleScreen());
        }

        private void openLocalizationEditor()
        {
            GUIModule.GetGUI<Localization.OverhaulLocalizationEditor>().TryShow();
        }

        private void openAboutWindow()
        {
            MyModdedObject.GetObjectFromList<RectTransform>(2).gameObject.SetActive(true);
            ModdedObject mObj = MyModdedObject.GetObjectFromList<ModdedObject>(2);
            if (!_hasInitializedAboutWindow)
            {
                _hasInitializedAboutWindow = true;
                mObj.GetObjectFromList<Button>(1).onClick.AddListener(closeAboutWindow);
                mObj.GetObjectFromList<Button>(6).onClick.AddListener(openModbot);
                mObj.GetObjectFromList<Button>(8).onClick.AddListener(openGithub);
                mObj.GetObjectFromList<Button>(10).onClick.AddListener(openGoogleFormsBugReport);
            }

            mObj.GetObjectFromList<Text>(0).text = OverhaulMain.GetTranslatedString("About_AboutCDO");
            mObj.GetObjectFromList<Text>(2).text = OverhaulMain.GetTranslatedString("About_MadeBy");
            mObj.GetObjectFromList<Text>(3).text = OverhaulMain.GetTranslatedString("About_Version") + OverhaulDescription.GetModVersion(true) + "\n" + OverhaulMain.GetTranslatedString("About_DevStatus");
            mObj.GetObjectFromList<Text>(5).text = OverhaulMain.GetTranslatedString("About_Desc");
            mObj.GetObjectFromList<Text>(7).text = OverhaulMain.GetTranslatedString("About_ModBobPage");
            mObj.GetObjectFromList<Text>(9).text = OverhaulMain.GetTranslatedString("About_GitHub");
            mObj.GetObjectFromList<Text>(11).text = OverhaulMain.GetTranslatedString("About_ReportABugGoogle");
            mObj.GetObjectFromList<Text>(14).text = OverhaulMain.GetTranslatedString("About_Feedback");
            mObj.GetObjectFromList<Text>(15).text = OverhaulMain.GetTranslatedString("About_InDiscord");
        }
        private void closeAboutWindow()
        {
            MyModdedObject.GetObjectFromList<RectTransform>(2).gameObject.SetActive(false);
        }
        private void openGoogleFormsBugReport()
        {
            BaseUtils.OpenURL("https://forms.gle/ZXdQyTrYrbZDX4LH7");
        }
        private void openGithub()
        {
            BaseUtils.OpenURL("https://github.com/aTVCat/CloneDroneOverhaul");
        }
        private void openModbot()
        {
            BaseUtils.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
        }

        private void openChangelog()
        {
            BaseUtils.OpenURL("https://github.com/aTVCat/CloneDroneOverhaul/releases/tag/" + OverhaulDescription.GetModVersion(false));
            //BaseUtils.OpenURL("https://github.com/aTVCat/CloneDroneOverhaul/releases/tag/a0.2.0.13");
        }
    }
}
