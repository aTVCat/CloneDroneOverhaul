using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches.Behaviours
{
    internal class EnergyBarPatchBehaviour : GamePatchBehaviour
    {
        [ModSetting(ModSettingsConstants.ENERGY_UI_REWORK, true)]
        public static bool EnablePatch;

        private EnergyUI m_playerEnergyBar;
        public EnergyUI playerEnergyBar
        {
            get
            {
                if (!m_playerEnergyBar)
                {
                    m_playerEnergyBar = ModCache.gameUIRoot?.EnergyUI;
                }
                return m_playerEnergyBar;
            }
        }

        private EnergyUI m_mountEnergyBar;
        private EnergyUI mountEnergyBar
        {
            get
            {
                if (!m_mountEnergyBar)
                {
                    Transform mountEnergyBarTransform = TransformUtils.FindChildRecursive(ModCache.gameUIRoot.transform, "EnergyUI_Mount");
                    if (!mountEnergyBarTransform) return null;

                    m_mountEnergyBar = mountEnergyBarTransform.GetComponent<EnergyUI>();
                }
                return m_mountEnergyBar;
            }
        }

        public void PatchEnergyUI()
        {
            patchPlayerEnergyBar();
            patchMountEnergyBar();
        }

        private void patchPlayerEnergyBar()
        {
            EnergyUI energyBarComponent = playerEnergyBar;
            if (!energyBarComponent) return;

            Transform barTransform = energyBarComponent.transform;
            EnergyBarBehaviour behaviour = barTransform.GetComponent<EnergyBarBehaviour>();
            if (!behaviour)
            {
                behaviour = barTransform.gameObject.AddComponent<EnergyBarBehaviour>();
                behaviour.IsMountEnergyBar = false;
                behaviour.EnergyUI = energyBarComponent;
            }

            patchGlowFill(energyBarComponent, behaviour);

            RectTransform dividerContainer = energyBarComponent.DividerContainer;
            dividerContainer.anchoredPosition = Vector3.up * 4f;

            GameObject errorText = energyBarComponent.InsufficientEnergyText?.gameObject;
            if (errorText)
            {
                SoftShadow softShadow = errorText.GetComponent<SoftShadow>();
                if (!softShadow)
                {
                    softShadow = errorText.AddComponent<SoftShadow>();
                    softShadow.effectDistance = Vector2.one * -2f;
                }
            }
        }

        private void patchMountEnergyBar()
        {
            EnergyUI energyBarComponent = mountEnergyBar;
            if (!energyBarComponent) return;

            Transform barTransform = energyBarComponent.transform;
            EnergyBarBehaviour behaviour = barTransform.GetComponent<EnergyBarBehaviour>();
            if (!behaviour)
            {
                behaviour = barTransform.gameObject.AddComponent<EnergyBarBehaviour>();
                behaviour.IsMountEnergyBar = true;
                behaviour.EnergyUI = energyBarComponent;
            }

            patchGlowFill(energyBarComponent, behaviour);
        }

        private void patchGlowFill(EnergyUI energyUI, EnergyBarBehaviour behaviour)
        {
            RectTransform glowFillTransform = energyUI.GlowFill;
            glowFillTransform.offsetMax = new Vector2(25f, 25f);
            glowFillTransform.offsetMin = new Vector2(-50f, -15f);
            Image image = glowFillTransform.GetComponent<Image>();
            image.sprite = ModResources.Sprite(AssetBundleConstants.UI, "Glow-3-256x256");

            Color color = image.color;
            color.a = 1f;
            image.color = color;

            if (behaviour)
            {
                behaviour.GlowFill = image;
                behaviour.GlowFillTransform = glowFillTransform;
            }
        }
    }
}