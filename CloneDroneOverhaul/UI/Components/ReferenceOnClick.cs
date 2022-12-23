using System;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI.Components
{
    public class ReferenceOnClick : MonoBehaviour
    {
        private Action<MonoBehaviour> ActOnClick;

        public static void CallOnClickWithReference(GameObject obj, Action<MonoBehaviour> evnt)
        {
            obj.AddComponent<ReferenceOnClick>().ActOnClick = evnt;
        }

        private void Awake()
        {
            base.GetComponent<Button>().onClick.AddListener(onButtonClick);
        }

        private void onButtonClick()
        {
            ActOnClick(this);
        }
    }
}