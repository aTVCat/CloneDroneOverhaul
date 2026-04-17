using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIExclusivePerkMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnEditorButtonClicked))]
        [UIElement("EditorButton")]
        private readonly Button _editorButton;

        [UIElementAction(nameof(OnRefreshButtonClicked))]
        [UIElement("RetrieveDataButton")]
        private readonly Button _refreshButton;

        [UIElement("UnlockedPerkDisplay", false)]
        private readonly ModdedObject _unlockedPerkDisplay;

        [UIElement("Content")]
        private readonly Transform _container;

        [UIElement("NothingIndicator")]
        private readonly GameObject _nothingIndicator;

        public override bool HideTitleScreen => true;

        public override void Show()
        {
            base.Show();
            Populate();

            string error = ExclusivePerkManager.Instance.GetError();
            if (error != null)
            {
                ModUIUtils.MessagePopup(true, "Could not get data. Retry?", error, 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "Ok", "Retry", "No", null, delegate
                {
                    OnRefreshButtonClicked();
                }, null);
            }

            _editorButton.gameObject.SetActive(ModUserInfo.IsDeveloper);
        }

        public void Populate()
        {
            if (_container.childCount != 0)
                TransformUtils.DestroyAllChildren(_container);

            System.Collections.Generic.List<ExclusivePerkInfo> list = ExclusivePerkManager.Instance.GetUnlockedPerks();
            if (list.IsNullOrEmpty())
            {
                _nothingIndicator.SetActive(true);
            }
            else
            {
                _nothingIndicator.SetActive(false);
                foreach (ExclusivePerkInfo perkInfo in list)
                {
                    ModdedObject moddedObject = Instantiate(_unlockedPerkDisplay, _container);
                    moddedObject.gameObject.SetActive(true);
                    moddedObject.GetObject<Text>(0).text = perkInfo.DisplayName;
                    moddedObject.GetObject<Image>(1).sprite = perkInfo.Icon.IsNullOrEmpty() ? null : ModResources.Sprite(AssetBundleConstants.PERK_ICONS, perkInfo.Icon);
                }
            }
        }

        public void OnEditorButtonClicked()
        {
            ModUIConstants.ShowExclusivePerksEditor(base.transform);
        }

        public void OnRefreshButtonClicked()
        {
            _refreshButton.interactable = false;
            ExclusivePerkManager.Instance.LoadDataFromRepository(delegate (string error)
            {
                _refreshButton.interactable = true;

                if (!error.IsNullOrEmpty())
                    ModUIUtils.MessagePopupOK("Error", error, true);

                Populate();
            });
        }
    }
}
