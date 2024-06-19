using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
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
            if (!personalizationEditorManager)
                return;

            PersonalizationItemInfo personalizationItemInfo = personalizationEditorManager.currentEditingItemInfo;
            if (personalizationItemInfo == null)
                return;

            personalizationItemInfo.Version++;
            if (!personalizationEditorManager.SaveItem(out string error2))
            {
                UIPersonalizationEditor.instance.ShowSaveErrorMessage(error2);
                return;
            }

            m_sendButton.interactable = false;
            m_loadingIndicator.SetActive(true);

            PersonalizationItemVerificationManager.Instance.SendItemToVerification(personalizationItemInfo, delegate
            {
                m_loadingIndicator.SetActive(false);
                ModUIUtils.MessagePopupOK("Success", "", true);
            }, delegate (string error)
            {
                m_sendButton.interactable = true;
                m_loadingIndicator.SetActive(true);
                ModUIUtils.MessagePopupOK("Could not send item to verification", error, true);
            });
        }
    }
}
