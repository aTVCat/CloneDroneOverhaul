using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static CloneDroneOverhaul.UI.Components.OverhaulContextMenu;

namespace CloneDroneOverhaul.UI.Components
{
    public class DoOnClick : MonoBehaviour, IPointerClickHandler
    {
        Action ActOnClick;
        UnityEngine.Events.UnityEvent Event;

        public static void AddComponent(GameObject obj, Action evnt)
        {
            obj.AddComponent<DoOnClick>().ActOnClick = evnt;
        }
        public static void AddComponent(GameObject obj, UnityEngine.Events.UnityEvent evnt)
        {
            obj.AddComponent<DoOnClick>().Event = evnt;
        }

        void IPointerClickHandler.OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(Event != null)
            {
                Event.Invoke();
                return;
            }
            ActOnClick();
        }
    }
}