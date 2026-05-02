using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIImageExplorer : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElement("ImageDisplayPrefab", false)]
        private readonly ModdedObject _imageDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform _imageDisplayContainer;

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_imageDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_imageDisplayContainer);
        }

        public void Populate(List<string> imageLinks, bool customLinks = false)
        {
            if (_imageDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_imageDisplayContainer);

            if (imageLinks.IsNullOrEmpty())
                return;

            foreach (string p in imageLinks)
            {
                ModdedObject moddedObject = Instantiate(_imageDisplayPrefab, _imageDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                UIElementImageDisplay imageDisplay = moddedObject.gameObject.AddComponent<UIElementImageDisplay>();
                imageDisplay.InitializeAsElement();
                imageDisplay.Populate(p, customLinks);
                imageDisplay.imageViewerParentTransform = base.transform;
            }
        }
    }
}
