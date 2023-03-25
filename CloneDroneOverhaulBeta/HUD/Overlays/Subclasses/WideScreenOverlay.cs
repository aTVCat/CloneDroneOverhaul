using UnityEngine;

namespace CDOverhaul.HUD.Overlays
{
    public class WideScreenOverlay : OverhaulOverlayBase
    {
        private RectTransform m_UpperBar;
        private RectTransform m_LowerBar;

        private bool m_IsInCutscene;

        public static bool ForceSetIsInCutscene;
        public static float TargetY = 38.0f;

        public override void Awake()
        {
            base.gameObject.SetActive(true);
        }

        public override void Start()
        {
            ModdedObject m = GetComponent<ModdedObject>();
            m_UpperBar = m.GetObject<RectTransform>(0);
            m_UpperBar.anchoredPosition = new Vector3(0, 80f, 0);
            m_LowerBar = m.GetObject<RectTransform>(1);
            m_LowerBar.anchoredPosition = new Vector3(0, -80f, 0);
        }

        protected override void OnDisposed()
        {
            m_UpperBar = null;
            m_LowerBar = null;
            ForceSetIsInCutscene = false;
        }

        private void Update()
        {
            if(Time.frameCount % 10 == 0)
            {
                m_IsInCutscene = ShouldActivateOverlay();
            }

            Vector3 newVector = m_UpperBar.anchoredPosition;
            newVector.y += ((m_IsInCutscene ? TargetY : 80f) - newVector.y) * Time.unscaledDeltaTime * 2f;
            m_UpperBar.anchoredPosition = newVector;

            Vector3 newVector2 = m_LowerBar.anchoredPosition;
            newVector2.y += ((m_IsInCutscene ? -TargetY : -80f) - newVector2.y) * Time.unscaledDeltaTime * 2f;
            m_LowerBar.anchoredPosition = newVector2;
        }

        public static bool ShouldActivateOverlay() // Todo: Optimize this
        {
            return ForceSetIsInCutscene || ((CutSceneManager.Instance.IsInCutscene() && !GameUIRoot.Instance.UpgradeUI.gameObject.activeSelf) ||
                (GameModeManager.IsBattleRoyale() && BattleRoyaleManager.Instance != null && BattleRoyaleManager.Instance.state.TimeToGameStart >= 0 && BattleRoyaleManager.Instance.state.TimeToGameStart <= 16f));
        }
    }
}