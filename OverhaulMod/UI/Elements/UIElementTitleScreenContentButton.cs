using OverhaulMod.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.UI
{
    public class UIElementTitleScreenContentButton : OverhaulUIBehaviour
    {
        [UIElement("LoadingIndicator", false)]
        private readonly GameObject m_loadingIndicator;

        private ContentManager m_contentManager;

        protected override void OnInitialized()
        {
            m_contentManager = ContentManager.Instance;
        }

        public override void Update()
        {
            m_loadingIndicator.SetActive(m_contentManager.isLoadingAnyContent);
        }
    }
}
