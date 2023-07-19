using UnityEngine;

namespace CDOverhaul.HUD.Tooltips
{
    [RequireComponent(typeof(ModdedObject))]
    public abstract class OverhaulTooltip : OverhaulBehaviour
    {
        private OverhaulTooltipsController m_Controller;
        protected OverhaulTooltipsController Controller
        {
            get
            {
                if (!m_Controller)
                    m_Controller = OverhaulController.GetController<OverhaulTooltipsController>();

                return m_Controller;
            }
        }

        private ModdedObject m_MyModdedObject;
        protected ModdedObject MyModdedObject
        {
            get
            {
                if (!m_MyModdedObject)
                    m_MyModdedObject = base.GetComponent<ModdedObject>();

                return m_MyModdedObject;
            }
        }

        private float m_TimeToHide;

        public abstract void Initialize();
        protected override void OnDisposed() => OverhaulDisposable.AssignNullToAllVars(this);
        protected virtual float GetShowDuration() => 3f;

        public void Show()
        {
            base.gameObject.SetActive(true);
            m_TimeToHide = Time.unscaledTime + GetShowDuration() + OverhaulTooltipsUI.TooltipsAdditionalShowDuration;
        }

        private void Update()
        {
            if (Time.unscaledTime > m_TimeToHide)
                base.gameObject.SetActive(false);
        }
    }
}
