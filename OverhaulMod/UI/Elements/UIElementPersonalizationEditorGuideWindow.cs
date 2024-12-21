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
        private readonly Button m_closeButton;

        [UIElement("Header")]
        private readonly Text m_header;

        [UIElement("Description")]
        private readonly Text m_description;

        [UIElement("GuideWindow")]
        private readonly CanvasGroup m_canvasGroup;

        [UIElement("Button", false)]
        private readonly ModdedObject m_buttonPrefab;

        [UIElement("ButtonContainer")]
        private readonly Transform m_buttonContainer;

        private PersonalizationEditorGuide m_currentGuide;

        private int m_currentGuideStageIndex;

        protected override void OnInitialized()
        {
            m_currentGuideStageIndex = -1;
        }

        public override void Show()
        {
            base.Show();
            m_canvasGroup.alpha = 0f;
        }

        public override void Update()
        {
            float d = Time.unscaledDeltaTime;
            float dMultiplied = d * 12.5f;

            m_canvasGroup.alpha += Mathf.Lerp(m_canvasGroup.alpha, 1f, dMultiplied);
        }

        public void StartGuide(PersonalizationEditorGuide guide)
        {
            Show();

            Clear();

            m_currentGuide = guide;
            m_currentGuideStageIndex = 0;
            if (guide.Stages.IsNullOrEmpty())
            {
                SetTexts("Error", "This guide doesn't have any stages");
                return;
            }

            PopulateGuideStage(guide.Stages[0]);
        }

        public void FinishGuide()
        {
            m_currentGuide = null;
            m_currentGuideStageIndex = -1;
            Hide();
        }

        public void NextGuideStage()
        {
            System.Collections.Generic.List<PersonalizationEditorGuideStage> list = m_currentGuide.Stages;
            int index = m_currentGuideStageIndex + 1;
            if (index >= list.Count)
                return;

            m_currentGuideStageIndex = index;
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

            if (m_currentGuide.IsTranslated)
            {
                string header = LocalizationManager.Instance.GetTranslatedString($"ceditor_small_tutorial_header_{guideStage.Header.ToLower().Replace(' ', '_')}");
                string description = LocalizationManager.Instance.GetTranslatedString($"ceditor_small_tutorial_text_page_{m_currentGuideStageIndex + 1}");
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

            if (m_currentGuideStageIndex >= m_currentGuide.Stages.Count - 1)
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
            if (m_buttonContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_buttonContainer);
        }

        public void SetTexts(string header, string description)
        {
            m_header.text = header;
            m_description.text = description;
        }

        public void InstantiateButton(string text, UnityAction unityAction)
        {
            ModdedObject moddedObject = Instantiate(m_buttonPrefab, m_buttonContainer);
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
