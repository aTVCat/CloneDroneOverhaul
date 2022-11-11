using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static CloneDroneOverhaul.UI.Components.OverhaulContextMenu;

namespace CloneDroneOverhaul.UI.Components
{
    public class DoOnMouseActions : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        Action ActOnClick;
        Action<bool> ActOnMouseHover;
        UnityEngine.Events.UnityEvent Event;

        bool _mouseIsOverUIElement;

        public static void AddComponent(GameObject obj, Action evnt)
        {
            obj.AddComponent<DoOnMouseActions>().ActOnClick = evnt;
        }
        public static void AddComponentWithActionWithMouse(GameObject obj, Action<bool> onMouseHover)
        {
            obj.AddComponent<DoOnMouseActions>().ActOnMouseHover = onMouseHover;
        }
        public static void AddComponentWithOnClickEvent(GameObject obj, UnityEngine.Events.UnityEvent evnt)
        {
            obj.AddComponent<DoOnMouseActions>().Event = evnt;
        }
        public static void AddComponentWithEventWithMouse(GameObject obj, UnityEngine.Events.UnityEvent evnt, Action<bool> onMouseHover)
        {
            DoOnMouseActions comp = obj.AddComponent<DoOnMouseActions>();
            comp.Event = evnt;
            comp.ActOnMouseHover = onMouseHover;
        }

        void IPointerClickHandler.OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(Event != null)
            {
                Event.Invoke();
                return;
            }
            if(ActOnClick != null)
            ActOnClick();
        }

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (_mouseIsOverUIElement == false && ActOnMouseHover != null)
            {
                ActOnMouseHover(true);
            }
            _mouseIsOverUIElement = true;
        }

        void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(_mouseIsOverUIElement == true && ActOnMouseHover != null)
            {
                ActOnMouseHover(false);
            }
            _mouseIsOverUIElement = false;
        }

        void IPointerUpHandler.OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (_mouseIsOverUIElement == true && ActOnMouseHover != null)
            {
                ActOnMouseHover(false);
            }
            _mouseIsOverUIElement = false;
        }
    }
}