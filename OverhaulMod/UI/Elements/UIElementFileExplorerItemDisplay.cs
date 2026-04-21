using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementFileExplorerItemDisplay : OverhaulUIBehaviour, IPointerClickHandler
    {
        private float _timeForDoubleClick;

        private Button _button;

        public string fullName
        {
            get;
            set;
        }

        public string displayName
        {
            get;
            set;
        }

        public bool isFolder
        {
            get;
            set;
        }

        public Action<UIElementFileExplorerItemDisplay> clickAction
        {
            get;
            set;
        }

        public Action<UIElementFileExplorerItemDisplay> doubleClickAction
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            _button = base.GetComponent<Button>();
        }

        public override void Start()
        {
            UIElementShowTooltipOnHightLight showTooltipOnHightLight = base.gameObject.AddComponent<UIElementShowTooltipOnHightLight>();
            showTooltipOnHightLight.InitializeElement();
            showTooltipOnHightLight.TooltipText = displayName;
            showTooltipOnHightLight.TooltipShowDuration = 2f;
        }

        public bool IsDoubleClicked() => Time.unscaledTime < _timeForDoubleClick;

        private void onClicked()
        {
            if (IsDoubleClicked())
            {
                doubleClickAction?.Invoke(this);
                return;
            }
            _timeForDoubleClick = Time.unscaledTime + 0.25f;
            clickAction?.Invoke(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClicked();
        }
    }
}
