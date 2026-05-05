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
        private static Color s_darkerWhite = new Color(0.95f, 0.95f, 0.95f, 1f);

        [ModSetting(ModSettingsConstants.LAST_BUILD_CHANGELOG_WAS_SHOWN, null)]
        public static string LastBuildChangelogWasShownOn;

        [UIElementAction(nameof(OnCloseButtonClicked))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnFeedbackButtonClicked))]
        [UIElement("FeedbackButton")]
        private readonly Button _feedbackButton;

        [UIElement("VersionDisplay", false)]
        private readonly ModdedObject _versionDisplay;

        [UIElement("Separator", false)]
        private readonly ModdedObject _separator;

        [UIElement("Content")]
        private readonly Transform _container;

        [UIElement("Header")]
        private readonly Text _headerText;

        [UIElement("TextLine", false)]
        private readonly Text _textLine;

        [UIElement("ImageEmbed", false)]
        private readonly ModdedObject _imageEmbed;

        [UIElement("TextContent")]
        private readonly Transform _textContainer;

        [UIElement("VersionListBG")]
        private readonly GameObject _versionListBG;

        [UIElement("VersionListScrollRect")]
        private readonly GameObject _versionListScrollRectObject;

        [UIElement("VersionListScrollRect")]
        private readonly ScrollRect _versionListScrollRect;

        [UIElement("MainPart")]
        private readonly RectTransform _mainPartTransform;

        [UIElement("Panel")]
        private readonly RectTransform _panelTransform;

        [UIElement("ChangelogScrollRect")]
        private readonly ScrollRect _changelogScrollRect;

        private Image _shading;

        public override bool HideTitleScreen => true;

        public override bool CloseOnEscapeButtonPress => _allowHidingThisMenu;

        private bool _allowHidingThisMenu;

        private Button _previousButtonClicked;

        private Button _firstButton;

        protected override void OnInitialized()
        {
            _allowHidingThisMenu = true;
            _shading = base.GetComponent<Image>();
            _textLine.gameObject.AddComponent<BetterOutline>().effectColor = Color.black;

            string path = Path.Combine(ModCore.DataFolder, "changelogs");
            if (!Directory.Exists(path)) return;

            List<Version> versions = new List<Version>();
            foreach (string directory in Directory.GetDirectories(path))
            {
                string dirName = ModFileUtils.GetDirectoryName(directory);
                if (!Version.TryParse(dirName, out Version version)) version = new Version(0, 0, 0);

                if (ModBuild.Version < version) continue;

                versions.Add(version);
            }

            if (versions.IsNullOrEmpty()) return;

            Button firstButton = null;

            versions.Sort(CompareByVersion);

            int majorVersion = ModBuild.VersionMinor;
            int minorVersion = ModBuild.VersionBuild;
            foreach (Version version in versions)
            {
                string updateString;
                if ((version.Major != majorVersion || version.Minor != minorVersion) || !firstButton)
                {
                    majorVersion = version.Major;
                    minorVersion = version.Minor;
                    updateString = $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} {majorVersion}.{minorVersion}";

                    ModdedObject separator = Instantiate(_separator, _container);
                    separator.gameObject.SetActive(true);
                    separator.GetObject<Text>(0).text = updateString;
                }
                else
                {
                    updateString = $"{LocalizationManager.Instance.GetTranslatedString("changelog_update")} {majorVersion}.{minorVersion}";
                }

                ModdedObject display = Instantiate(_versionDisplay, _container);
                display.gameObject.SetActive(true);

                string verString = version.ToString();
                string detailsString = getBuildDetails(verString);
                string verHeader = detailsString.IsNullOrEmpty() ? verString : $"{detailsString} ({verString})";
                display.GetObject<Text>(0).text = verHeader;

                Button button = display.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    Button pb = _previousButtonClicked;
                    if (pb)
                        pb.interactable = true;

                    button.interactable = false;
                    _previousButtonClicked = button;

                    PopulateChangelog(updateString, verHeader, verString);
                });

                if (!firstButton)
                {
                    firstButton = button;
                    _firstButton = firstButton;
                }
            }

            if (firstButton)
                firstButton.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(null));

            ModActionUtils.DoInFrames(refreshVersionsScrollRect, 2);
        }

        public override void Hide()
        {
            base.Hide();

            ModSettingsManager.SetStringValue(ModSettingsConstants.LAST_BUILD_CHANGELOG_WAS_SHOWN, ModBuild.Version.ToString());
            ModSettingsDataManager.Instance.Save();
        }

        public void ShowVersionList()
        {
            _versionListBG.SetActive(true);
            _versionListScrollRectObject.SetActive(true);

            Vector2 offset = _mainPartTransform.offsetMin;
            offset.x = 265f;
            _mainPartTransform.offsetMin = offset;
        }

        public void HideVersionList()
        {
            _versionListBG.SetActive(false);
            _versionListScrollRectObject.SetActive(false);

            Vector2 offset = _mainPartTransform.offsetMin;
            offset.x = 10f;
            _mainPartTransform.offsetMin = offset;
        }

        public void ShrinkPanel()
        {
            Vector2 sideDelta = _panelTransform.sizeDelta;
            sideDelta.x = 475f;
            _panelTransform.sizeDelta = sideDelta;
        }

        public void ExpandPanel()
        {
            Vector2 sideDelta = _panelTransform.sizeDelta;
            sideDelta.x = 725f;
            _panelTransform.sizeDelta = sideDelta;
        }

        public void SetPanelOffset(Vector2 offset)
        {
            _panelTransform.anchoredPosition = offset;
        }

        public void SetCloseButtonActive(bool value)
        {
            _exitButton.gameObject.SetActive(value);
            _allowHidingThisMenu = value;
        }

        public void SetShadingActive(bool value)
        {
            _shading.enabled = value;
        }

        public void SetContext(ShowArguments showArguments)
        {
            if (showArguments.ShrinkPanel)
                ShrinkPanel();
            else
                ExpandPanel();

            if (showArguments.HideVersionList)
                HideVersionList();
            else
                ShowVersionList();

            SetPanelOffset(showArguments.PanelOffset);
            SetCloseButtonActive(showArguments.CloseButtonActive);
            SetShadingActive(!showArguments.DisableShading);
        }

        public void ClickOnFirstButton()
        {
            if (_firstButton)
                _firstButton.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(null));
        }

        public void PopulateChangelog(string updateString, string header, string folderName)
        {
            string path = Path.Combine(ModCore.DataFolder, "changelogs", folderName);
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

            PopulateText($"{updateString} · {header}", text);
        }

        public void Clear()
        {
            if (_textContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_textContainer);
        }

        public void PopulateText(string header, string text)
        {
            _headerText.text = header;
            Clear();

            if (text.IsNullOrEmpty() || text.IsNullOrWhiteSpace())
                return;

            ModActionUtils.DoInFrames(refreshChangelogScrollRect, 2);

            if (text.Contains(Environment.NewLine))
            {
                foreach (string line in text.Split(Environment.NewLine.ToCharArray()))
                    instantiateChangelogElement(line);

                return;
            }
            instantiateChangelogElement(text);
        }

        private void instantiateChangelogElement(string line)
        {
            if (line.IsNullOrEmpty())
                return;

            if (line.StartsWith("img="))
            {
                ModdedObject moddedObject = Instantiate(_imageEmbed, _textContainer);
                moddedObject.gameObject.SetActive(true);

                UIElementPatchNotesImageEmbed imageEmbed = moddedObject.gameObject.AddComponent<UIElementPatchNotesImageEmbed>();
                imageEmbed.URL = line.Substring(4);
                imageEmbed.PatchNotesTransform = base.transform;
                imageEmbed.InitializeElement();
            }
            else
            {
                Text textLine = Instantiate(_textLine, _textContainer);
                textLine.gameObject.SetActive(true);
                configureLine(textLine, line);
            }
        }

        private void configureLine(Text textLine, string line)
        {
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

        private string getBuildDetails(string folder)
        {
            string patch = LocalizationManager.Instance.GetTranslatedString("word_patch");

            switch (folder)
            {
                // test
                case "4.3.215":
                    return $"Preview";

                // releases
                case "3.0.345":
                case "3.1.0":
                case "4.0.227":
                case "4.1.13":
                case "4.2.32":
                    return LocalizationManager.Instance.GetTranslatedString("initial_release");

                // single patches
                case "3.1.8":
                case "4.1.14":
                    return patch;

                // series of patches
                case "4.2.46":
                    return $"{patch} 1";
                case "4.2.52":
                    return $"{patch} 2";
                case "4.2.54":
                    return $"{patch} 3";
                case "4.2.1030":
                    return $"{patch} 4";
                case "4.2.1036":
                    return $"{patch} 5";
                case "4.2.1037":
                    return $"{patch} 6";
                case "4.2.1038":
                    return $"{patch} 7";
                case "4.2.1045":
                    return $"{patch} 8";
                case "4.2.1051":
                    return $"{patch} 9";
                case "4.2.1052":
                    return $"{patch} 10";
                case "4.2.1124":
                    return $"{patch} 11";

                default:
                    return string.Empty;
            }
        }

        private void refreshVersionsScrollRect()
        {
            _versionListScrollRect.verticalNormalizedPosition = 1f; // for some reason it sometimes auto scrolls to anywhere, but the top
        }

        private void refreshChangelogScrollRect()
        {
            _changelogScrollRect.verticalNormalizedPosition = 1f;
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
            if (b > a) return 1;
            else if (b < a) return -1;

            return 0;
        }

        public struct ShowArguments
        {
            public bool ShrinkPanel;

            public bool HideVersionList;

            public Vector2 PanelOffset;

            public bool CloseButtonActive;

            public bool DisableShading;
        }
    }
}
