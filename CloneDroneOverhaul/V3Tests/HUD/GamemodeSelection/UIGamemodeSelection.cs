using CloneDroneOverhaul.UI.Components;
using CloneDroneOverhaul.V3Tests.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.V3Tests.HUD
{
    public class UIGamemodeSelection : V3_ModHUDBase
    {
        private void Start()
        {
            MyModdedObject.GetObjectFromList<Button>(0).onClick.AddListener(delegate
            {
                GameUIRoot.Instance.TitleScreenUI.OnCloseModeSelectButtonClicked();
            });
            base.MyModdedObject.GetObjectFromList<Button>(9).onClick.AddListener(delegate
            {
                StoryModeOverhaul.Instance.StartOvermode(null, false);
            });
            base.MyModdedObject.GetObjectFromList<Button>(8).onClick.AddListener(delegate
            {
                EndlessModeOverhaul.Instance.StartOvermode(null, true);
            });

            base.MyModdedObject.GetObjectFromList<Button>(8).gameObject.SetActive(OverhaulDescription.TEST_FEATURES_ENABLED);
            base.MyModdedObject.GetObjectFromList<Button>(9).gameObject.SetActive(OverhaulDescription.TEST_FEATURES_ENABLED);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameUIRoot.Instance.TitleScreenUI.OnCloseModeSelectButtonClicked();
            }
        }

        public void Show(GameModeCardData[] data, bool onlyShow = false)
        {
            base.gameObject.SetActive(true);
            if (onlyShow)
            {
                return;
            }

            MyModdedObject.GetObjectFromList<Text>(5).text = LocalizationManager.Instance.GetTranslatedString("Choose your game mode!");

            TransformUtils.DestroyAllChildren(MyModdedObject.GetObjectFromList<RectTransform>(1));

            for (int i = 0; i < data.Length; i++)
            {
                ModdedObject mObj = Instantiate<ModdedObject>(MyModdedObject.GetObjectFromList<ModdedObject>(4), MyModdedObject.GetObjectFromList<RectTransform>(1));
                mObj.gameObject.SetActive(true);
                mObj.GetObjectFromList<Image>(0).sprite = data[i].ThumbnailSprite;
                mObj.GetObjectFromList<Text>(1).text = LocalizationManager.Instance.GetTranslatedString(data[i].NameOfMode);
                SelectableUI sUI = BaseUtils.AddSelectableUIToObject(mObj.gameObject);
                mObj.GetObjectFromList<RectTransform>(2).gameObject.SetActive(false);
                DoOnMouseActions.AddComponentWithEventWithMouse(mObj.gameObject, data[i].ClickedCallback, delegate (bool isMouseOver)
                {
                    mObj.GetObjectFromList<RectTransform>(2).gameObject.SetActive(isMouseOver);
                });
            }
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
        }

        public void SetMainScreenVisible(bool val)
        {
            if (!val)
            {
                Hide();
            }
            else
            {
                Show(null, true);
            }
        }
    }
}
