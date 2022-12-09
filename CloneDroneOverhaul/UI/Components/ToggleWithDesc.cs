using System;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI.Components
{
    public class ToggleWithDesc : MonoBehaviour
    {
        Action<bool> action;
        public ToggleWithDesc SetUp(string descTranslateID, Action<bool> onChangedValue)
        {
            Text txt = base.GetComponent<ModdedObject>().GetObjectFromList<Text>(0);
            base.GetComponent<Toggle>().onValueChanged.AddListener(onValueChanged);
            action = onChangedValue;

            if (!string.IsNullOrEmpty(descTranslateID))
            {
                txt.text = OverhaulMain.GetTranslatedString(descTranslateID);
            }

            return this;
        }

        public void SetValue(bool val)
        {
            base.GetComponent<Toggle>().isOn = val;
        }
        void onValueChanged(bool val)
        {
            if (action != null)
            {
                action(val);
            }
        }


        public bool ToggleValue
        {
            get
            {
                return base.GetComponent<Toggle>().isOn;
            }
        }
    }
}