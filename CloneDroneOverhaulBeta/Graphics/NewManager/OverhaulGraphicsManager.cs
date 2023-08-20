using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CDOverhaul.HUD;

namespace CDOverhaul.Graphics
{
    public class OverhaulGraphicsManager : OverhaulManager
    {
        public AmplifyOclusionImageEffect amplifyOclusion
        {
            get;
            private set;
        }

        public BloomOverhaulImageEffect bloomOverhaul
        {
            get;
            private set;
        }

        public AmplifyColorOverhaulImageEffect amplifyColorOverhaul
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            base.Initialize();
            BloomOverhaulImageEffect.SetBloomVanilla.EventAction = delegate
            {
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Post effects.Enable bloom", true), true);
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Post effects.Bloom iterations", true), 2);
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Post effects.Bloom intensity", true), 0.5f);
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Post effects.Bloom Threshold", true), 0.9f);

                ParametersMenu menu = OverhaulController.GetController<ParametersMenu>();
                if (menu && menu.gameObject.activeSelf)
                    menu.PopulateCategory(menu.SelectedCategory, true);
            };

            AmplifyColorOverhaulImageEffect.ApplyAmplifyColorPreset1.EventAction = delegate
            {
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Amplify color.Amplify color presetV2", true), 5);
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Amplify color.TonemapperV2", true), 2);

                ParametersMenu menu = OverhaulController.GetController<ParametersMenu>();
                if (menu && menu.gameObject.activeSelf)
                    menu.PopulateCategory(menu.SelectedCategory, true);
            };

            AmplifyColorOverhaulImageEffect.ApplyAmplifyColorPreset2.EventAction = delegate
            {
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Amplify color.Amplify color presetV2", true), 2);
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Amplify color.TonemapperV2", true), 1);

                ParametersMenu menu = OverhaulController.GetController<ParametersMenu>();
                if (menu && menu.gameObject.activeSelf)
                    menu.PopulateCategory(menu.SelectedCategory, true);
            };

            AmplifyColorOverhaulImageEffect.ApplyAmplifyColorPresetDefault.EventAction = delegate
            {
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Amplify color.Amplify color presetV2", true), 0);
                SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Amplify color.TonemapperV2", true), 0);

                ParametersMenu menu = OverhaulController.GetController<ParametersMenu>();
                if (menu && menu.gameObject.activeSelf)
                    menu.PopulateCategory(menu.SelectedCategory, true);
            };
        }

        protected override void OnDisposed()
        {
            if (amplifyOclusion)
                amplifyOclusion.Dispose();
            if (bloomOverhaul)
                bloomOverhaul.Dispose();
            if (amplifyColorOverhaul)
                amplifyColorOverhaul.Dispose();

            base.OnDisposed();
        }

        public override void OnSceneReloaded()
        {
            DelegateScheduler.Instance.Schedule(reloadEffects, 0.1f);
        }

        protected override void OnAssetsLoaded()
        {
            instantiateEffects();
        }

        private void instantiateEffects()
        {
            if (IsDisposedOrDestroyed())
                return;

            amplifyOclusion = base.gameObject.AddComponent<AmplifyOclusionImageEffect>();
            bloomOverhaul = base.gameObject.AddComponent<BloomOverhaulImageEffect>();
            amplifyColorOverhaul = base.gameObject.AddComponent<AmplifyColorOverhaulImageEffect>();
        }

        private void reloadEffects()
        {
            if (IsDisposedOrDestroyed())
                return;

            amplifyOclusion.Reload();
            bloomOverhaul.Reload();
            amplifyColorOverhaul.Reload();
        }
    }
}
