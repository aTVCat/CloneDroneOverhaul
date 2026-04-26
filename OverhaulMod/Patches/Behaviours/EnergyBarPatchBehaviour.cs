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

        private EnergyUI _playerEnergyBar;
        public EnergyUI PlayerEnergyBar
        {
            get
            {
                if (!_playerEnergyBar)
                {
                    _playerEnergyBar = ModCache.UIRoot?.EnergyUI;
                }
                return _playerEnergyBar;
            }
        }

        private EnergyUI _mountEnergyBar;
        private EnergyUI MountEnergyBar
        {
            get
            {
                if (!_mountEnergyBar)
                {
                    Transform mountEnergyBarTransform = TransformUtils.FindChildRecursive(ModCache.UIRoot.transform, "EnergyUI_Mount");
                    if (!mountEnergyBarTransform) return null;

                    _mountEnergyBar = mountEnergyBarTransform.GetComponent<EnergyUI>();
                }
                return _mountEnergyBar;
            }
        }

        public void PatchEnergyUI()
        {
            patchPlayerEnergyBar();
            patchMountEnergyBar();
        }

        private void patchPlayerEnergyBar()
        {
            EnergyUI energyBarComponent = PlayerEnergyBar;
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
            EnergyUI energyBarComponent = MountEnergyBar;
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