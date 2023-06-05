using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.HUD.Tooltips
{
    public class OverhaulTooltipsUI : OverhaulUI
    {
        [OverhaulSetting("Mod.Tooltips.Show tooltips", true, !OverhaulVersion.IsUpdate2Hotfix)]
        public static bool ShowTooltips;

        [SettingSliderParameters(false, 2f, 8f)]
        [OverhaulSetting("Mod.Tooltips.Show duration", 4f, !OverhaulVersion.IsUpdate2Hotfix)]
        public static float TooltipsShowDuration;

        public OverhaulTooltipsController TooltipsController;

        private GameObject m_DefaultTooltipPrefab;
        private GameObject m_Container;

        internal bool HaveToPopulateTooltips;
        private float m_TimeToHideTooltips;

        public override void Initialize()
        {
            m_DefaultTooltipPrefab = MyModdedObject.GetObject<Transform>(1).gameObject;
            m_DefaultTooltipPrefab.SetActive(false);
            m_Container = MyModdedObject.GetObject<Transform>(0).gameObject;
            HaveToPopulateTooltips = true;

            DelegateScheduler.Instance.Schedule(PopulateTooltipsContainer, 1f);
        }

        public GameObject GetDefaultPrefab() => m_DefaultTooltipPrefab;

        public void PopulateTooltipsContainer()
        {
            if (!HaveToPopulateTooltips || !TooltipsController || HadBadStart)
                return;

            foreach(OverhaulTooltip tooltip in TooltipsController.GetTooltips())
            {
                GameObject gameObject = tooltip.GetTooltipPrefab();
                if (gameObject)
                {
                    ModdedObject moddedObject = Instantiate(gameObject.GetComponent<ModdedObject>(), m_Container.transform);
                    moddedObject.gameObject.SetActive(true);

                    tooltip.SpawnedTooltipModdedObject = moddedObject;
                    tooltip.OnSpawnedTooltip();
                }
            }

            HaveToPopulateTooltips = false;
        }

        public void Show()
        {
            if (!ShowTooltips || !OverhaulFeatureAvailabilitySystem.BuildImplements.AreTooltipsEnabled)
                return;

            if (HaveToPopulateTooltips)
                PopulateTooltipsContainer();

            m_TimeToHideTooltips = Time.unscaledTime + TooltipsShowDuration;
        }

        private void Update()
        {
            m_Container.SetActive(Time.unscaledTime < m_TimeToHideTooltips);
        }
    }
}
