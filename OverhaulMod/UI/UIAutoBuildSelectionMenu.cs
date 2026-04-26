using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAutoBuildSelectionMenu : OverhaulUIBehaviour
    {
        [UIElement("BuildDisplayPrefab", false)]
        private readonly ModdedObject _buildDisplayPrefab;

        [UIElement("Holder", false)]
        private readonly GameObject _holder;

        [UIElement("Container")]
        private readonly Transform _buildDisplayContainer;

        private CanvasGroup _canvasGroup;

        private UIElementAutoBuildSelectionEntry _prevSelectedEntry, _currentSelectedEntry;

        private bool _show;

        public override bool CloseOnEscapeButtonPress => false;

        protected override void OnInitialized()
        {
            _canvasGroup = base.GetComponent<CanvasGroup>();
            PopulateBuilds();
        }

        public override void Show()
        {
            base.Show();
            if (_currentSelectedEntry)
                _currentSelectedEntry.SetBGActive(false);

            _show = true;
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = true;
            _holder.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            ModActionUtils.DoInFrame(delegate
            {
                Cursor.lockState = CursorLockMode.None;
            });
        }

        public override void Hide()
        {
            _show = false;
            _canvasGroup.blocksRaycasts = false;

            if (_currentSelectedEntry)
            {
                AutoBuildManager.Instance.ApplyBuild(_currentSelectedEntry.transform.GetSiblingIndex());
                SelectEntry(null);
            }
        }

        public override void Update()
        {
            float alpha = _canvasGroup.alpha;
            alpha += Time.unscaledDeltaTime * 10f * (_show ? 1f : -1f);
            _canvasGroup.alpha = alpha;

            if (!_show && alpha <= 0.01f)
                _holder.SetActive(false);

            if (!_show)
                return;

            int number;
            if (Input.GetKeyDown(KeyCode.Alpha1))
                number = 0;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                number = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                number = 2;
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                number = 3;
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                number = 4;
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                number = 5;
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                number = 6;
            else if (Input.GetKeyDown(KeyCode.Alpha8))
                number = 7;
            else if (Input.GetKeyDown(KeyCode.Alpha9))
                number = 8;
            else if (Input.GetKeyDown(KeyCode.Alpha0))
                number = 9;
            else
                number = -1;

            if (number >= 0 && number < _buildDisplayContainer.childCount)
            {
                Transform transform = _buildDisplayContainer.GetChild(number);
                if (transform)
                {
                    UIElementAutoBuildSelectionEntry autoBuildSelectionEntry = transform.GetComponent<UIElementAutoBuildSelectionEntry>();
                    if (autoBuildSelectionEntry)
                    {
                        SelectEntry(autoBuildSelectionEntry);
                        Hide();
                    }
                }
            }
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused)
            {
                Hide();
            }
        }

        public void PopulateBuilds()
        {
            if (_buildDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_buildDisplayContainer);

            UpgradeManager upgradeManager = UpgradeManager.Instance;
            AutoBuildManager autoBuildManager = AutoBuildManager.Instance;
            foreach (AutoBuildInfo build in autoBuildManager.BuildList.Builds)
            {
                ModdedObject moddedObject = Instantiate(_buildDisplayPrefab, _buildDisplayContainer);
                moddedObject.gameObject.SetActive(true);

                Text buildNameText = moddedObject.GetObject<Text>(0);
                buildNameText.text = AutoBuildManager.GetBuildDisplayName(build.Name);

                Image upgradeIconPrefab = moddedObject.GetObject<Image>(1);
                upgradeIconPrefab.gameObject.SetActive(true);
                Transform upgradeIconContainer = moddedObject.GetObject<Transform>(2);
                foreach (UpgradeTypeAndLevel upgrade in build.Upgrades)
                {
                    UpgradeDescription upgradeDescription = upgradeManager.GetUpgrade(upgrade.UpgradeType, upgrade.UpgradeType == UpgradeType.Armor ? 0 : upgrade.Level);
                    Sprite sprite;
                    if (upgradeDescription && upgradeDescription.Icon)
                    {
                        sprite = upgradeDescription.Icon;
                    }
                    else
                    {
                        sprite = ModResources.Sprite(AssetBundleConstants.UI, "NA-HQ-128x128");
                    }

                    Image upgradeIcon = Instantiate(upgradeIconPrefab, upgradeIconContainer);
                    upgradeIcon.sprite = sprite;
                }
                upgradeIconPrefab.gameObject.SetActive(false);

                UIElementAutoBuildSelectionEntry autoBuildSelectionEntry = moddedObject.gameObject.AddComponent<UIElementAutoBuildSelectionEntry>();
                autoBuildSelectionEntry.BuildInfo = build;
                autoBuildSelectionEntry.Menu = this;
                autoBuildSelectionEntry.InitializeElement();
            }
        }

        public void SelectEntry(UIElementAutoBuildSelectionEntry entry)
        {
            if (entry)
                entry.SetBGActive(true);

            if (_currentSelectedEntry)
                _currentSelectedEntry.SetBGActive(false);

            _prevSelectedEntry = _currentSelectedEntry;
            _currentSelectedEntry = entry;
        }
    }
}
