using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorGuideWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        [UIElement("Header")]
        private readonly Text _header;

        [UIElement("Description")]
        private readonly Text _description;

        [UIElement("GuideWindow")]
        private readonly CanvasGroup _canvasGroup;

        [UIElement("Button", false)]
        private readonly ModdedObject _buttonPrefab;

        [UIElement("ButtonContainer")]
        private readonly Transform _buttonContainer;

        private PersonalizationEditorGuide _currentGuide;

        private int _currentGuideStageIndex;

        protected override void OnInitialized()
        {
            _currentGuideStageIndex = -1;
        }

        public override void Show()
        {
            base.Show();
            _canvasGroup.alpha = 0f;
        }

        public override void Update()
        {
            float d = Time.unscaledDeltaTime;
            float dMultiplied = d * 12.5f;

            _canvasGroup.alpha += Mathf.Lerp(_canvasGroup.alpha, 1f, dMultiplied);
        }

        public void StartGuide(PersonalizationEditorGuide guide)
        {
            Show();

            Clear();

            _currentGuide = guide;
            _currentGuideStageIndex = 0;
            if (guide.Stages.IsNullOrEmpty())
            {
                SetTexts("Error", "This guide doesn't have any stages");
                return;
            }

            PopulateGuideStage(guide.Stages[0]);
        }

        public void FinishGuide()
        {
            _currentGuide = null;
            _currentGuideStageIndex = -1;
            Hide();
        }

        public void NextGuideStage()
        {
            System.Collections.Generic.List<PersonalizationEditorGuideStage> list = _currentGuide.Stages;
            int index = _currentGuideStageIndex + 1;
            if (index >= list.Count)
                return;

            _currentGuideStageIndex = index;
            PopulateGuideStage(list[index]);
        }

        public void PopulateGuideStage(PersonalizationEditorGuideStage guideStage)
        {
            Clear();

            if (guideStage == null)
            {
                SetTexts("Error", "This stage doesn't have any content (this is strange)");
                return;
            }

            if (_currentGuide.IsTranslated)
            {
                string header = LocalizationManager.Instance.GetTranslatedString($"ceditor_small_tutorial_header_{guideStage.Header.ToLower().Replace(' ', '_')}");
                string description = LocalizationManager.Instance.GetTranslatedString($"ceditor_small_tutorial_text_page_{_currentGuideStageIndex + 1}");
                SetTexts(header, description);
            }
            else
            {
                SetTexts(guideStage.Header, guideStage.Description);
            }

            System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, UnityAction>> list = guideStage.Buttons;
            if (!list.IsNullOrEmpty())
            {
                foreach (System.Collections.Generic.KeyValuePair<string, UnityAction> button in list)
                {
                    InstantiateButton(button.Key, button.Value);
                }
            }

            if (_currentGuideStageIndex >= _currentGuide.Stages.Count - 1)
            {
                InstantiateButton("Finish", FinishGuide);
            }
            else
            {
                InstantiateButton("Next", NextGuideStage);
            }

        }

        public void Clear()
        {
            if (_buttonContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_buttonContainer);
        }

        public void SetTexts(string header, string description)
        {
            _header.text = header;
            _description.text = description;
        }

        public void InstantiateButton(string text, UnityAction unityAction)
        {
            ModdedObject moddedObject = Instantiate(_buttonPrefab, _buttonContainer);
            moddedObject.gameObject.SetActive(true);
            moddedObject.GetObject<Text>(0).text = text;

            if (unityAction != null)
            {
                Button button = moddedObject.GetComponent<Button>();
                button.onClick.AddListener(unityAction);
            }
        }
    }
}
