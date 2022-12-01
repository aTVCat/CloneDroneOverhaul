using System;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI.Components
{
    public class ReferenceOnClick : MonoBehaviour
    {
        Action<MonoBehaviour> ActOnClick;

        public static void CallOnClickWithReference(GameObject obj, Action<MonoBehaviour> evnt)
        {
            obj.AddComponent<ReferenceOnClick>().ActOnClick = evnt;
        }

        void Awake()
        {
            base.GetComponent<Button>().onClick.AddListener(onButtonClick);
        }

        void onButtonClick()
        {
            ActOnClick(this);
        }
    }
}