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

        [UIElement("SendButtonText")]
        private readonly Text m_sendButtonText;

        [UIElement("StatusText")]
        private readonly Text m_statusText;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicator;

        public override void Show()
        {
            base.Show();
            PersonalizationEditorManager personalizationEditorManager = PersonalizationEditorManager.Instance;
            if (!personalizationEditorManager)
                return;

            PersonalizationItemInfo personalizationItemInfo = personalizationEditorManager.currentEditingItemInfo;
            if (personalizationItemInfo == null)
                return;

            RefreshButtonAndStatusText(personalizationItemInfo);
        }

        public void RefreshButtonAndStatusText(PersonalizationItemInfo personalizationItemInfo)
        {
            if (!personalizationItemInfo.IsSentForVerification && !personalizationItemInfo.IsVerified)
            {
                m_statusText.text = "You haven't uploaded this item yet.";
            }
            else if (personalizationItemInfo.IsSentForVerification && !personalizationItemInfo.IsVerified)
            {
                m_statusText.text = "This item is being verified...";
            }
            else if (personalizationItemInfo.IsSentForVerification && personalizationItemInfo.IsVerified)
            {
                m_statusText.text = "This item's update is being verified...";
            }
            else
            {
                m_statusText.text = "This item is verified!\nYou can update it if you have made changes to it.";
            }

            if (personalizationItemInfo.IsVerified)
            {
                m_sendButtonText.text = "Update item";
            }
            else
            {
                m_sendButtonText.text = "Upload item";
            }

            m_sendButton.interactable = personalizationItemInfo != null && !personalizationItemInfo.IsSentForVerification;
        }

        public bool CanExit()
        {
            return m_exitButton.interactable;
        }

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

            m_exitButton.interactable = false;
            m_sendButton.interactable = false;
            m_loadingIndicator.SetActive(true);

            PersonalizationItemVerificationManager.Instance.SendItemToVerification(personalizationItemInfo, delegate
            {
                personalizationItemInfo.IsSentForVerification = true;
                _ = personalizationEditorManager.SaveItem(out _);

                m_exitButton.interactable = true;
                m_loadingIndicator.SetActive(false);

                RefreshButtonAndStatusText(personalizationItemInfo);
                ModUIUtils.MessagePopupOK("Success", "", true);
            }, delegate (string error)
            {
                m_exitButton.interactable = true;
                m_sendButton.interactable = true;
                m_loadingIndicator.SetActive(true);
                ModUIUtils.MessagePopupOK("Could not send item to verification", error, true);
            });
        }
    }
}
