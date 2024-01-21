using CDOverhaul.Device;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class RecommendationTooltip : OverhaulBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private GameObject m_TextPanel;

        private bool m_IsMouseIn;

        public void Initialize(RecommendationLevel level, string text)
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            m_TextPanel = moddedObject.GetObject<Transform>(3).gameObject;
            m_TextPanel.SetActive(false);

            moddedObject.GetObject<Text>(7).text = text;

            moddedObject.GetObject<Transform>(0).gameObject.SetActive(level == RecommendationLevel.Recommended);
            moddedObject.GetObject<Transform>(1).gameObject.SetActive(level == RecommendationLevel.BelowReqirements);
            moddedObject.GetObject<Transform>(2).gameObject.SetActive(level == RecommendationLevel.Unsupported);

            moddedObject.GetObject<Transform>(4).gameObject.SetActive(level == RecommendationLevel.Recommended);
            moddedObject.GetObject<Transform>(5).gameObject.SetActive(level == RecommendationLevel.BelowReqirements);
            moddedObject.GetObject<Transform>(6).gameObject.SetActive(level == RecommendationLevel.Unsupported);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_IsMouseIn = true;
            m_TextPanel.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_IsMouseIn = false;
            m_TextPanel.SetActive(false);
        }
    }
}
