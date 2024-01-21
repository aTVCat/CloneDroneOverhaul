using UnityEngine;

namespace CDOverhaul.HUD.Gamemodes
{
    public class OverhaulGamemodeUIBase : OverhaulBehaviour
    {
        protected OverhaulGamemodesUI GamemodesUI;

        public T Initialize<T>(OverhaulGamemodesUI overhaulGamemodesUI) where T : OverhaulGamemodeUIBase
        {
            GamemodesUI = overhaulGamemodesUI;
            OnInitialize();
            return (T)this;
        }

        protected virtual void OnInitialize() { }

        public void Show()
        {
            if (!GamemodesUI.gameObject.activeSelf)
                GamemodesUI.Show();

            base.gameObject.SetActive(true);
            OnShow();
        }

        protected virtual void OnShow() { }

        public void Hide()
        {
            if (GamemodesUI.gameObject.activeSelf)
                GamemodesUI.Hide();

            base.gameObject.SetActive(false);
            OnHide();
        }

        protected virtual void OnHide() { }

        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
                GamemodesUI.Hide();
            }
        }
    }
}
