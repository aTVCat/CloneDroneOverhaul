using UnityEngine;
using UnityEngine.EventSystems;
using static CloneDroneOverhaul.UI.Components.OverhaulContextMenu;

namespace CloneDroneOverhaul.UI.Components
{
    public class OverhaulContextMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public enum ContextMenuElementType
        {
            Text,

            Button
        }
        public enum ContextMenuActivationMethod
        {
            LMB,

            RMB
        }
        public class ContextMenuElement
        {
            public ContextMenuElementType ElementType;
            public UnityEngine.EventSystems.BaseEventData ButtonAction;
            public string TextFieldString;

            public ContextMenuElement(ContextMenuElementType elementType, BaseEventData buttonAction, string textFieldString)
            {
                ElementType = elementType;
                ButtonAction = buttonAction;
                TextFieldString = textFieldString;
            }
        }
        public ContextMenuElement[] Elements;
        public ContextMenuActivationMethod ActivationMethod;
        private bool isPointerOverElement;
        private float timeMouseIsOverElement;

        private bool isShowingUI;
        public bool IsShowingUI
        {
            get => isShowingUI;
            private set
            {
                if (value == isShowingUI)
                {
                    return;
                }
                isShowingUI = value;
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            isPointerOverElement = false;
            timeMouseIsOverElement = 0;
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            isPointerOverElement = true;
        }

        private void Update()
        {
            if (isPointerOverElement)
            {
                timeMouseIsOverElement += Time.unscaledDeltaTime;
                if (timeMouseIsOverElement > 0.5f)
                {

                }
            }
        }
    }

    public static class ContextMenuManager
    {
        public static OverhaulContextMenu AddContextMenuListener(this GameObject gObj, ContextMenuElement[] contextMenuElements, ContextMenuActivationMethod activationMethod)
        {
            OverhaulContextMenu menu = null;
            if (gObj != null)
            {
                menu = gObj.AddComponent<OverhaulContextMenu>();
                menu.Elements = contextMenuElements;
                menu.ActivationMethod = activationMethod;
            }
            return menu;
        }
    }

}