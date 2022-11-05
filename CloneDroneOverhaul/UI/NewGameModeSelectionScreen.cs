using CloneDroneOverhaul.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class NewGameModeSelectionScreen : ModGUIBase
    {
        public static NewGameModeSelectionScreen Instance;

        public enum GameModeScreenType
        {
            Singleplayer,
            Multiplayer,
            Other
        }

        public override void OnInstanceStart()
        {
            base.MyModdedObject = base.GetComponent<ModdedObject>();
            Hide();

            MyModdedObject.GetObjectFromList<Button>(0).onClick.AddListener(delegate
            {
                GameUIRoot.Instance.TitleScreenUI.OnCloseModeSelectButtonClicked();
            });

            Instance = this;
        }

        public void Show(GameModeCardData[] data, bool onlyShow = false)
        {
            base.gameObject.SetActive(true);
            if (onlyShow)
            {
                return;
            }

            TransformUtils.DestroyAllChildren(MyModdedObject.GetObjectFromList<RectTransform>(1));

            Button leftArrowButton = Instantiate<Button>(MyModdedObject.GetObjectFromList<Button>(3), MyModdedObject.GetObjectFromList<RectTransform>(1));
            leftArrowButton.gameObject.SetActive(true);

            for (int i = 0; i < data.Length; i++)
            {
                ModdedObject mObj = Instantiate<ModdedObject>(MyModdedObject.GetObjectFromList<ModdedObject>(4), MyModdedObject.GetObjectFromList<RectTransform>(1));
                mObj.gameObject.SetActive(true);
                mObj.GetObjectFromList<Image>(0).sprite = data[i].ThumbnailSprite;
                mObj.GetObjectFromList<Text>(1).text = data[i].NameOfMode;
                SelectableUI sUI = BaseUtils.AddSelectableUIToObject(mObj.gameObject);
                DoOnClick.AddComponent(mObj.gameObject, data[i].ClickedCallback);
            }

            Button rightArrowButton = Instantiate<Button>(MyModdedObject.GetObjectFromList<Button>(2), MyModdedObject.GetObjectFromList<RectTransform>(1));
            rightArrowButton.gameObject.SetActive(true);
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
