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
        private readonly Button m_exitButton;

        [UIElement("ImageDisplayPrefab", false)]
        private readonly ModdedObject m_imageDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform m_imageDisplayContainer;

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (m_imageDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_imageDisplayContainer);
        }

        public void Populate(List<string> imageLinks, bool customLinks = false)
        {
            if (m_imageDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_imageDisplayContainer);

            if (imageLinks.IsNullOrEmpty())
                return;

            foreach (string p in imageLinks)
            {
                ModdedObject moddedObject = Instantiate(m_imageDisplayPrefab, m_imageDisplayContainer);
                moddedObject.gameObject.SetActive(true);
                UIElementImageDisplay imageDisplay = moddedObject.gameObject.AddComponent<UIElementImageDisplay>();
                imageDisplay.InitializeElement();
                imageDisplay.Populate(p, customLinks);
                imageDisplay.imageViewerParentTransform = base.transform;
            }
        }
    }
}
