using OverhaulMod.Content;
using UnityEngine;

namespace OverhaulMod.UI
{
    public class UIElementTitleScreenAddonsButton : OverhaulUIBehaviour
    {
        [UIElement("LoadingIndicator", false)]
        private readonly GameObject _loadingIndicator;

        [UIElement("Image", true)]
        private readonly GameObject _icon;

        private AddonManager _addonManager;

        protected override void OnInitialized()
        {
            _addonManager = AddonManager.Instance;
        }

        public override void Update()
        {
            bool isLoadingAnyContent = _addonManager.IsLoadingAddons();
            _loadingIndicator.SetActive(isLoadingAnyContent);
            _icon.SetActive(!isLoadingAnyContent);
        }
    }
}
