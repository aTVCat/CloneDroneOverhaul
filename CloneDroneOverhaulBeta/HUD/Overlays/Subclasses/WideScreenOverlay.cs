using System.Collections;
using UnityEngine;

namespace CDOverhaul.HUD.Overlays
{
    public class WideScreenOverlay : OverhaulOverlayBase
    {
        private RectTransform m_UpperBar;
        private RectTransform m_LowerBar;

        private bool m_IsInCutscene;

        private void Awake()
        {
            base.gameObject.SetActive(true);
        }

        private void Start()
        {
            ModdedObject m = GetComponent<ModdedObject>();
            m_UpperBar = m.GetObject<RectTransform>(0);
            m_UpperBar.anchoredPosition = new Vector3(0, 80f, 0);
            m_LowerBar = m.GetObject<RectTransform>(1);
            m_LowerBar.anchoredPosition = new Vector3(0, -80f, 0);
        }

        private void Update()
        {
            if(Time.frameCount % 10 == 0)
            {
                m_IsInCutscene = CutSceneManager.Instance.IsInCutscene() && !GameUIRoot.Instance.UpgradeUI.gameObject.activeSelf;
            }

            Vector3 newVector = m_UpperBar.anchoredPosition;
            newVector.y += ((m_IsInCutscene ? 20f : 80f) - newVector.y) * Time.unscaledDeltaTime * 2f;
            m_UpperBar.anchoredPosition = newVector;

            Vector3 newVector2 = m_LowerBar.anchoredPosition;
            newVector2.y += ((m_IsInCutscene ? -20f : -80f) - newVector2.y) * Time.unscaledDeltaTime * 2f;
            m_LowerBar.anchoredPosition = newVector2;
        }
    }
}