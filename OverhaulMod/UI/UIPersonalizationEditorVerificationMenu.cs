using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorVerificationMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnSendButtonClicked))]
        [UIElement("SendButton")]
        private readonly Button m_sendButton;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicator;

        public void OnSendButtonClicked()
        {
            PersonalizationEditorManager personalizationEditorManager = PersonalizationEditorManager.Instance;
            if (personalizationEditorManager && Directory.Exists(personalizationEditorManager.editingFolder) && personalizationEditorManager.editingItemInfo != null)
            {
                m_sendButton.interactable = false;
                m_loadingIndicator.SetActive(true);

                personalizationEditorManager.SaveItem();
                PersonalizationItemVerificationManager.Instance.SendItemToVerification(personalizationEditorManager.editingFolder, personalizationEditorManager.editingItemInfo, delegate
                {
                    m_sendButton.interactable = true;
                    m_loadingIndicator.SetActive(false);
                    ModUIUtils.MessagePopupOK("Success", "Failure", true);
                }, delegate (string error)
                {
                    m_sendButton.interactable = false;
                    m_loadingIndicator.SetActive(true);
                    ModUIUtils.MessagePopupOK("Could not send item to verification", error, true);
                });
            }
        }
    }
}
