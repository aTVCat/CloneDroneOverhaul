using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class TwoButtonsToggle : OverhaulDisposable
    {
        public const string NonSelectedColor = "#808080";
        public const string OnColor = "#75FF93";
        public const string OffColor = "#FF6363";

        public Action StateSetOnAction;
        public Button OnButton;
        private Graphic m_OnButtonGraphic;

        public Action StateSetOffAction;
        public Button OffButton;
        private Graphic m_OffButtonGraphic;

        public bool State
        {
            get;
            private set;
        }

        public TwoButtonsToggle(ModdedObject moddedObject, int off, int on, Action offAction, Action onAction, Func<bool> setStateFunc)
        {
            StateSetOnAction = onAction;
            OnButton = moddedObject.GetObject<Button>(on);
            OnButton.onClick.AddListener(onButtonClicked);
            m_OnButtonGraphic = OnButton.targetGraphic;

            StateSetOffAction = offAction;
            OffButton = moddedObject.GetObject<Button>(off);
            OffButton.onClick.AddListener(offButtonClicked);
            m_OffButtonGraphic = OffButton.targetGraphic;

            if (setStateFunc != null)
            {
                SetState(setStateFunc(), false);
            }
        }

        protected override void OnDisposed()
        {
            AssignNullToAllVars(this);
        }

        public void SetState(bool state, bool invokeActions)
        {
            State = state;

            if (state)
            {
                m_OnButtonGraphic.color = OnColor.ToColor();
                m_OffButtonGraphic.color = NonSelectedColor.ToColor();

                if (invokeActions)
                {
                    StateSetOnAction?.Invoke();
                }
            }
            else
            {
                m_OnButtonGraphic.color = NonSelectedColor.ToColor();
                m_OffButtonGraphic.color = OffColor.ToColor();

                if (invokeActions)
                {
                    StateSetOffAction?.Invoke();
                }
            }
        }

        private void onButtonClicked()
        {
            SetState(true, true);
        }

        private void offButtonClicked()
        {
            SetState(false, true);
        }
    }
}
