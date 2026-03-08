using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationEditorVerificationMenu : OverhaulUIBehaviour
    {
        public const string ALL_WEAPON_VARIANTS_PRESENT_COLOR = "#262626";
        public const string ALL_WEAPON_VARIANTS_NOT_PRESENT_COLOR = "#802020";
        public const string WEAPON_VARIANT_NOT_PRESENT_COLOR = "#998523";

        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnSendButtonClicked))]
        [UIElement("SendButton")]
        private readonly Button _sendButton;

        [UIElement("SendButtonText")]
        private readonly Text _sendButtonText;

        [UIElement("StatusText")]
        private readonly Text _statusText;

        [UIElement("LoadingIndicator", false)]
        private readonly GameObject _loadingIndicator;

        [UIElement("ScrollRect")]
        private readonly Image _scrollRectBG;

        [UIElement("WeaponVariantDisplay", false)]
        private readonly ModdedObject _weaponVariantDisplay;

        [UIElement("Content")]
        private readonly Transform _container;

        private bool _currentItemIsNotFullyCompleted;

        private bool _currentItemIsFullyIncomplete;

        public override void Show()
        {
            base.Show();
            PersonalizationEditorManager personalizationEditorManager = PersonalizationEditorManager.Instance;
            if (!personalizationEditorManager)
                return;

            PersonalizationItemInfo personalizationItemInfo = personalizationEditorManager.currentEditingItemInfo;
            if (personalizationItemInfo == null)
                return;

            RefreshItemCompletion(personalizationEditorManager.currentEditingRoot);
            RefreshButtonAndStatusText(personalizationItemInfo);
        }

        public void RefreshItemCompletion(PersonalizationEditorObjectBehaviour objectBehaviour)
        {
            if (_container.childCount != 0)
                TransformUtils.DestroyAllChildren(_container);

            _currentItemIsFullyIncomplete = true;
            _currentItemIsNotFullyCompleted = false;
            string bgColor = ALL_WEAPON_VARIANTS_PRESENT_COLOR;

            Dictionary<WeaponVariant2, bool> supportedVariants = new Dictionary<WeaponVariant2, bool>();

            PersonalizationItemVerificationManager personalizationItemVerificationManager = PersonalizationItemVerificationManager.Instance;
            foreach (WeaponVariant2 weaponVariant in typeof(WeaponVariant2).GetEnumValues())
            {
                if (weaponVariant == WeaponVariant2.None)
                    continue;

                bool value = personalizationItemVerificationManager.DoesWeaponSkinSupportWeaponVariant(objectBehaviour, weaponVariant, out bool weaponDoesHaveThisVariant);
                if (weaponDoesHaveThisVariant)
                {
                    supportedVariants.Add(weaponVariant, value);

                    ModdedObject moddedObject = Instantiate(_weaponVariantDisplay, _container);
                    moddedObject.gameObject.SetActive(true);
                    moddedObject.GetObject<Text>(0).text = WeaponVariantManager.GetWeaponVariantString(weaponVariant);
                    moddedObject.GetObject<GameObject>(1).SetActive(value);
                    moddedObject.GetObject<GameObject>(2).SetActive(!value);
                    if (!value)
                    {
                        _currentItemIsNotFullyCompleted = true;
                        moddedObject.transform.SetAsFirstSibling();
                        bgColor = WEAPON_VARIANT_NOT_PRESENT_COLOR;
                    }
                    else
                    {
                        _currentItemIsFullyIncomplete = false;
                    }
                }
            }

            if (_currentItemIsFullyIncomplete)
            {
                bgColor = ALL_WEAPON_VARIANTS_NOT_PRESENT_COLOR;
            }

            _scrollRectBG.color = ModParseUtils.TryParseToColor(bgColor, Color.gray);
        }

        public void RefreshButtonAndStatusText(PersonalizationItemInfo personalizationItemInfo)
        {
            if (!personalizationItemInfo.IsSentForVerification && !personalizationItemInfo.IsVerified)
            {
                _statusText.text = "You haven't uploaded this item yet.";
            }
            else if (personalizationItemInfo.IsSentForVerification && !personalizationItemInfo.IsVerified)
            {
                _statusText.text = "This item is being verified...";
            }
            else if (personalizationItemInfo.IsSentForVerification && personalizationItemInfo.IsVerified)
            {
                _statusText.text = "This item's update is being verified...";
            }
            else
            {
                _statusText.text = "This item is verified!\nYou can update it if you have made changes.";
            }

            if (personalizationItemInfo.IsVerified)
            {
                if (personalizationItemInfo.IsSentForVerification)
                {
                    _sendButtonText.text = "Reupload update";
                }
                else
                {
                    _sendButtonText.text = "Update item";
                }
            }
            else if (personalizationItemInfo.IsSentForVerification)
            {
                _sendButtonText.text = "Reupload item";
            }
            else
            {
                _sendButtonText.text = "Upload item";
            }

            _sendButton.interactable = !_currentItemIsFullyIncomplete && personalizationItemInfo != null && !personalizationItemInfo.ReuploadedTheItem;
        }

        public bool CanExit()
        {
            return _exitButton.interactable;
        }

        public void OnSendButtonClicked()
        {
            if (_currentItemIsNotFullyCompleted)
            {
                ModUIUtils.MessagePopup(true, "Item not fully completed", "Your weapon skin doesn't support some of possible weapon variants.\nDo you still want to send the item to verification?", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, null, "Yes", "No", null, sendItemToVerification);
                return;
            }
            sendItemToVerification();
        }

        private void sendItemToVerification()
        {
            PersonalizationEditorManager personalizationEditorManager = PersonalizationEditorManager.Instance;
            if (!personalizationEditorManager)
                return;

            PersonalizationItemInfo personalizationItemInfo = personalizationEditorManager.currentEditingItemInfo;
            if (personalizationItemInfo == null)
                return;

            if (personalizationItemInfo.IsVerified)
                personalizationItemInfo.Version++;

            if (!personalizationEditorManager.SaveItem(out string error2, true))
            {
                if (error2.Length > 512)
                    error2 = error2.Remove(512);

                UIPersonalizationEditor.instance.ShowSaveErrorMessage(error2);
                return;
            }

            _exitButton.interactable = false;
            _sendButton.interactable = false;
            _loadingIndicator.SetActive(true);

            PersonalizationItemVerificationManager.Instance.SendItemToVerification(personalizationItemInfo, delegate
            {
                if (personalizationItemInfo.IsSentForVerification) personalizationItemInfo.ReuploadedTheItem = true;
                personalizationItemInfo.IsSentForVerification = true;

                _ = personalizationEditorManager.SaveItem(out _, true);

                _exitButton.interactable = true;
                _loadingIndicator.SetActive(false);

                RefreshButtonAndStatusText(personalizationItemInfo);
                ModUIUtils.MessagePopupOK("Success", "It can take few hours or days to verify items.", true);
            }, delegate (string error)
            {
                _exitButton.interactable = true;
                _sendButton.interactable = true;
                _loadingIndicator.SetActive(true);
                ModUIUtils.MessagePopupOK("Could not send item to verification", $"Try again later.\n\nIf the error doesn't get fixed, it'll probably get fixed in the next mod update.", 200f, true);
            });
        }
    }
}
