using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPatchNotes : OverhaulUIBehaviour
    {
        private static Color s_darkerWhite = new Color(0.87f, 0.87f, 0.87f, 1f);

        [ModSetting(ModSettingsConstants.LAST_BUILD_CHANGELOG_WAS_SHOWN, null)]
        public static string LastBuildChangelogWasShownOn;

        [UIElementAction(nameof(OnCloseButtonClicked))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnFeedbackButtonClicked))]
        [UIElement("FeedbackButton")]
        private readonly Button m_feedbackButton;

        [UIElement("VersionDisplay", false)]
        private readonly ModdedObject m_versionDisplay;

        [UIElement("Content")]
        private readonly Transform m_container;

        [UIElement("Separator", false)]
        private readonly GameObject m_separator;

        [UIElement("Header")]
        private readonly Text m_headerText;

        [UIElement("TextLine", false)]
        private readonly Text m_textLine;

        [UIElement("ImageEmbed", false)]
        private readonly ModdedObject m_imageEmbed;

        [UIElement("TextContent")]
        private readonly Transform m_textContainer;

        public override bool hideTitleScreen => true;

        private Button m_previousButtonClicked;

        private Button m_firstButton;

        protected override void OnInitialized()
        {
            m_textLine.gameObject.AddComponent<BetterOutline>().effectColor = Color.black;

            string path = Path.Combine(ModCore.dataFolder, "changelogs");
            if (!Directory.Exists(path))
                return;

            List<Version> versions = new List<Version>();
            foreach (string directory in Directory.GetDirectories(path))
            {
                string dirName = ModFileUtils.GetDirectoryName(directory);

                if (!Version.TryParse(dirName, out Version version))
                    version = new Version(0, 0, 0, 0);

                if (ModBuildInfo.version < version)
                    continue;

                versions.Add(version);
            }

            if (versions.IsNullOrEmpty())
                return;

            Button firstButton = null;

            versions.Sort(CompareByVersion);

            int minorVersion = ModBuildInfo.versionMinor;
            int buildVersion = ModBuildInfo.versionBuild;
            foreach (Version version in versions)
            {
                if ((version.Minor != minorVersion || version.Build != buildVersion) && firstButton)
                {
                    GameObject separator = Instantiate(m_separator, m_container);
                    separator.SetActive(true);
                    minorVersion = version.Minor;
                    buildVersion = version.Build;
                }

                ModdedObject display = Instantiate(m_versionDisplay, m_container);
                display.gameObject.SetActive(true);

                string verString = version.ToString();
                string verHeader = getBuildDisplayVersion(verString);
                display.GetObject<Text>(0).text = verHeader;

                Button button = display.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    Button pb = m_previousButtonClicked;
                    if (pb)
                        pb.interactable = true;

                    button.interactable = false;
                    m_previousButtonClicked = button;

                    PopulateChangelog(verHeader, verString);
                });

                if (!firstButton)
                {
                    firstButton = button;
                    m_firstButton = firstButton;
                }
            }

            if (firstButton)
                firstButton.Press();
        }

        public override void Hide()
        {
            base.Hide();

            ModSettingsManager.SetStringValue(ModSettingsConstants.LAST_BUILD_CHANGELOG_WAS_SHOWN, ModBuildInfo.version.ToString());
            ModSettingsDataManager.Instance.Save();
        }

        public void ClickOnFirstButton()
        {
            if (m_firstButton)
                m_firstButton.Press();
        }

        public void PopulateChangelog(string header, string folderName)
        {
            if (m_textContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_textContainer);

            string path = Path.Combine(ModCore.dataFolder, "changelogs", folderName);
            string langCode = LocalizationManager.Instance.GetCurrentLanguageCode();
            if (langCode != "ru" && langCode != "en")
                langCode = "en";

            string file = Path.Combine(path, $"changelog_{langCode}.txt");
            string text;
            if (File.Exists(file))
            {
                text = ModFileUtils.ReadText(file);
                if (text.IsNullOrEmpty())
                {
                    text = "This update doesn't have any description.";
                }
            }
            else
            {
                if (langCode != "en")
                {
                    file = Path.Combine(path, $"changelog_en.txt");
                    if (File.Exists(file))
                        text = ModFileUtils.ReadText(file);
                    else
                        text = "Changelog file read error.";
                }
                else
                {
                    text = "Changelog file read error.";
                }
            }

            m_headerText.text = header;

            if (!text.Contains(Environment.NewLine))
            {
                Text textLine = Instantiate(m_textLine, m_textContainer);
                textLine.gameObject.SetActive(true);
                textLine.text = text;
            }
            else
            {
                string[] lines = text.Split(Environment.NewLine.ToCharArray());
                foreach (string line in lines)
                {
                    if (line.IsNullOrEmpty())
                        continue;

                    if (line.StartsWith("img="))
                    {
                        ModdedObject moddedObject = Instantiate(m_imageEmbed, m_textContainer);
                        moddedObject.gameObject.SetActive(true);

                        UIElementPatchNotesImageEmbed imageEmbed = moddedObject.gameObject.AddComponent<UIElementPatchNotesImageEmbed>();
                        imageEmbed.URL = line.Substring(4);
                        imageEmbed.PatchNotesTransform = base.transform;
                        imageEmbed.InitializeElement();
                    }
                    else
                    {
                        Text textLine = Instantiate(m_textLine, m_textContainer);
                        textLine.gameObject.SetActive(true);

                        if (line.StartsWith("# "))
                        {
                            textLine.fontSize = 19;
                            textLine.color = Color.white;
                            textLine.text = line.Substring(2);
                            textLine.transform.GetChild(0).gameObject.SetActive(true);
                        }
                        else
                        {
                            textLine.fontSize = 12;
                            textLine.color = s_darkerWhite;
                            textLine.text = line;
                            textLine.transform.GetChild(0).gameObject.SetActive(false);
                        }
                    }
                }
            }
            m_textLine.text = text;
        }

        private string getBuildDisplayVersion(string folder)
        {
            switch (folder)
            {
                case "0.2.0.13":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 2 (0.2.0.13)";
                case "0.2.0.22":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 2 {LocalizationManager.Instance.GetTranslatedString("word_patch")} (0.2.0.22)";
                case "0.2.10.22":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 2 HotFix (0.2.10.22)";
                case "0.3.0.345":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 3 (0.3.0.345)";
                case "0.3.1.0":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 3.1 (0.3.1.0)";
                case "0.3.1.8":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 3.1 HotFix (0.3.1.8)";
                case "0.4.0.200":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 4 Test (0.4.0.200)";
                case "0.4.0.227":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 4 (0.4.0.227)";
                case "0.4.1.13":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 4.1 (0.4.1.13)";
                case "0.4.2.32":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 4.2 (0.4.2.32)";
                case "0.4.2.46":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 4.2 {LocalizationManager.Instance.GetTranslatedString("word_patch")} (0.4.2.46)";
                case "0.4.2.52":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 4.2 {LocalizationManager.Instance.GetTranslatedString("word_patch")} 2 (0.4.2.52)";
                case "0.4.3.0":
                    return $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} 4.3 Preview 1 (0.4.3.0)";
                default:
                    return folder;
            }
        }

        public void OnCloseButtonClicked()
        {
            Hide();
        }

        public void OnFeedbackButtonClicked()
        {
            _ = ModUIConstants.ShowFeedbackUIRework(false);
        }

        public static int CompareByVersion(Version a, Version b)
        {
            if (b > a)
            {
                return 1;
            }
            if (b < a)
            {
                return -1;
            }
            return 0;
        }
    }
}
