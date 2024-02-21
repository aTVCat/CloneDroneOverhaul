using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UINewsDetailsPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("Header")]
        private readonly Text m_titleText;

        [UIElement("Description")]
        private readonly Text m_descriptionText;

        [UIElement("NoSurveysAvailableLabel", false)]
        private readonly GameObject m_noSurveyAvailableLabelObject;

        [UIElement("SurveysAvailableLabel", false)]
        private readonly GameObject m_surveyAvailableLabelObject;

        [UIElement("SurveysBG", false)]
        private readonly Transform m_surveyBG;

        [UIElement("AnswerVariantButton", false)]
        private readonly ModdedObject m_answerVariantButton;

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void Populate(NewsInfo newsInfo)
        {
            m_titleText.text = newsInfo.Title;
            m_descriptionText.text = newsInfo.Description;

            bool hasSurvey = !newsInfo.Survey.IsNullOrEmpty();
            m_noSurveyAvailableLabelObject.SetActive(!hasSurvey);
            m_surveyAvailableLabelObject.SetActive(hasSurvey);
            m_surveyBG.gameObject.SetActive(hasSurvey);

            if (m_surveyBG.childCount != 0)
                TransformUtils.DestroyAllChildren(m_surveyBG);

            List<Button> list = new List<Button>();

            if (hasSurvey)
            {
                bool hasAnsweredTheSurvey = NewsManager.Instance.HasAnsweredSurvey(newsInfo.Title);

                string[] split = newsInfo.Survey.Split('@');
                foreach (string answer in split)
                {
                    ModdedObject moddedObject = Instantiate(m_answerVariantButton, m_surveyBG);
                    moddedObject.gameObject.SetActive(true);
                    moddedObject.GetObject<Text>(0).text = answer;

                    Button button = moddedObject.GetComponent<Button>();
                    button.onClick.AddListener(delegate
                    {
                        if (!list.IsNullOrEmpty())
                            foreach (Button button1 in list)
                                if (button1)
                                    button1.interactable = false;

                        ModWebhookManager.Instance.ExecuteSurveysWebhook(answer, newsInfo.Title, delegate
                        {
                            NewsManager.Instance.SetHasAnsweredSurvey(newsInfo.Title);

                            ModUIUtils.MessagePopupOK("Answer given!", "", 100f, true);
                        }, delegate (string error)
                        {
                            if (!list.IsNullOrEmpty())
                                foreach (Button button1 in list)
                                    if (button1)
                                        button1.interactable = true;

                            ModUIUtils.MessagePopupOK("News survey error", "Could not execute webhook. Details:\n" + error);
                        });
                    });
                    button.interactable = !hasAnsweredTheSurvey;
                    list.Add(button);
                }
            }
        }
    }
}
