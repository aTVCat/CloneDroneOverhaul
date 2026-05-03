using OverhaulMod.Content.Personalization;
using OverhaulMod.Content.Personalization.Objects;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPEVerificationMenu : OverhaulUIBehaviour
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

        [UIElement("WeaponSkinTaskList", false)]
        private readonly GameObject _weaponSkinTaskList;

        [UIElement("WeaponSkinTaskList")]
        private readonly Image _weaponSkinTaskListBG;

        [UIElement("AccessoriesTaskList", false)]
        private readonly GameObject _accessoriesTaskList;

        [UIElement("WeaponVariantDisplay", false)]
        private readonly ModdedObject _weaponVariantDisplay;

        [UIElement("Content")]
        private readonly Transform _container;

        private bool _currentItemIsNotFullyCompleted;

        private bool _currentItemIsFullyIncomplete;

        private PersonalizationItemInfo _itemInfo;

        private PersonalizationEditorPlacedObject _itemRootObject;

        public override void Show()
        {
            base.Show();

            _itemInfo = PersonalizationEditorManager.Instance.EditingItemInfo;
            _itemRootObject = PersonalizationEditorManager.Instance.EditingRoot;

            RefreshCompletion();
            RefreshButtonAndStatusText();
        }

        public void RefreshCompletion()
        {
            if (_container.childCount != 0)
                TransformUtils.DestroyAllChildren(_container);

            _currentItemIsFullyIncomplete = true;
            _currentItemIsNotFullyCompleted = false;

            if (_itemInfo == null || !_itemRootObject) return;

            PersonalizationCategory category = _itemInfo.Category;
            _weaponSkinTaskList.SetActive(category == PersonalizationCategory.WeaponSkins);
            _accessoriesTaskList.SetActive(category == PersonalizationCategory.Accessories);

            if (category == PersonalizationCategory.WeaponSkins)
            {
                updateWeaponSkinCompletion(_itemRootObject);
            }
            else if (category == PersonalizationCategory.Accessories)
            {
                updateAccessoryCompletion(_itemRootObject);
            }
        }

        private void updateWeaponSkinCompletion(PersonalizationEditorPlacedObject rootObject)
        {
            string bgColor = ALL_WEAPON_VARIANTS_PRESENT_COLOR;

            Dictionary<WeaponVariant2, bool> supportedVariants = new Dictionary<WeaponVariant2, bool>();

            PersonalizationItemVerificationManager personalizationItemVerificationManager = PersonalizationItemVerificationManager.Instance;
            foreach (WeaponVariant2 weaponVariant in typeof(WeaponVariant2).GetEnumValues())
            {
                if (weaponVariant == WeaponVariant2.None)
                    continue;

                bool value = personalizationItemVerificationManager.DoesWeaponSkinSupportWeaponVariant(rootObject, weaponVariant, out bool weaponDoesHaveThisVariant);
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

            _weaponSkinTaskListBG.color = ModParseUtils.TryParseColor(bgColor, Color.gray);
        }

        private void updateAccessoryCompletion(PersonalizationEditorPlacedObject rootObject)
        {
            _currentItemIsFullyIncomplete = false;
            _currentItemIsNotFullyCompleted = false;
        }

        public void RefreshButtonAndStatusText()
        {
            PersonalizationItemInfo itemInfo = PersonalizationEditorManager.Instance.EditingItemInfo;
            if (itemInfo == null) return;

            if (!itemInfo.IsSentForVerification && !itemInfo.IsVerified)
            {
                _statusText.text = "You haven't uploaded this item yet.";
            }
            else if (itemInfo.IsSentForVerification && !itemInfo.IsVerified)
            {
                _statusText.text = "This item is being verified...";
            }
            else if (itemInfo.IsSentForVerification && itemInfo.IsVerified)
            {
                _statusText.text = "This item's update is being verified...";
            }
            else
            {
                _statusText.text = "This item is verified!\nYou can update it if you have made changes.";
            }

            if (itemInfo.IsVerified)
            {
                if (itemInfo.IsSentForVerification)
                {
                    _sendButtonText.text = "Reupload update";
                }
                else
                {
                    _sendButtonText.text = "Update item";
                }
            }
            else if (itemInfo.IsSentForVerification)
            {
                _sendButtonText.text = "Reupload item";
            }
            else
            {
                _sendButtonText.text = "Upload item";
            }

            _sendButton.interactable = !_currentItemIsFullyIncomplete && itemInfo != null && !itemInfo.ReuploadedTheItem;
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
            PersonalizationItemInfo itemInfo = _itemInfo;
            if (itemInfo == null) return;

            if (itemInfo.IsVerified) itemInfo.Version++;

            PersonalizationItemSaveResult saveResult = personalizationEditorManager.SaveItem(true);
            if (saveResult.HasFailed())
            {
                string error = saveResult.Error;
                if (error.Length > 512) error = error.Remove(512);

                UIPE.Instance.ShowSaveErrorMessage(error);
                return;
            }

            _exitButton.interactable = false;
            _sendButton.interactable = false;
            _loadingIndicator.SetActive(true);

            PersonalizationItemVerificationManager.Instance.SendItemToVerification(itemInfo, delegate
            {
                if (itemInfo.IsSentForVerification) itemInfo.ReuploadedTheItem = true;
                itemInfo.IsSentForVerification = true;

                saveResult = personalizationEditorManager.SaveItem(true);

                _exitButton.interactable = true;
                _loadingIndicator.SetActive(false);

                RefreshButtonAndStatusText();

                if (saveResult.HasFailed())
                {
                    ModUIUtils.MessagePopupOK("Success and fail", "Your item has been sent to verification, but some data wasn't saved correctly.\nYour item is not corrupted and you don't have to send the item to verification again.\n\nIt can take few hours or days to verify items.", true);
                }
                else
                {
                    ModUIUtils.MessagePopupOK("Success", "It can take few hours or days to verify items.", true);
                }
            }, delegate
            {
                _exitButton.interactable = true;
                _sendButton.interactable = true;
                _loadingIndicator.SetActive(true);
                ModUIUtils.MessagePopupOK("Could not send item to verification", $"Try again later.\n\nIf the error doesn't get fixed, it'll probably get fixed in the next mod update.", 200f, true);
            });
        }
    }
}
