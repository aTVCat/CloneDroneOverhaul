using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches.Behaviours
{
    internal class CustomizationButtonPatchBehaviour : GamePatchBehaviour
    {
        private GameObject _customizeButton;

        private Image _image;

        private Sprite _ogSprite;

        private Shadow _shadow;

        private Vector2 _ogSizeDelta;

        public override void Patch()
        {
            Transform buttonRoot = ModCache.TitleScreenUI.CustomizationUI.ButtonRoot.transform;
            _customizeButton = TransformUtils.FindChildRecursive(buttonRoot, "CustomizeButton").gameObject;

            RectTransform rectTransform = _customizeButton.GetComponent<RectTransform>();
            _ogSizeDelta = rectTransform.sizeDelta;
            rectTransform.sizeDelta = new Vector2(140f, 25f);

            _image = _customizeButton.GetComponent<Image>();
            _ogSprite = _image.sprite;
            ModUIUtils.ReplaceBackgroundSprite(_image, true);
            _shadow = _image.GetComponent<Shadow>();

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.LevelSpawned, onLevelSpawned);

            onLevelSpawned();
        }

        public override void Unpatch()
        {
            RectTransform rectTransform = _customizeButton.GetComponent<RectTransform>();
            rectTransform.sizeDelta = _ogSizeDelta;

            _image.sprite = _ogSprite;
            if (_shadow) Destroy(_shadow);

            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.LevelSpawned, onLevelSpawned);
        }

        private void onLevelSpawned()
        {
            if (!GameModeManager.IsOnTitleScreen()) return;

            _customizeButton.gameObject.SetActive(FindFirstObjectByType<CustomizationPlayerPreview>());
        }
    }
}
