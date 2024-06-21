﻿using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using PicaVoxel;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectVolume : PersonalizationEditorObjectComponentBase
    {
        private bool m_hasAddedEventListeners;

        private PersonalizationEditorObjectVisibilityController m_visibilityController;
        public PersonalizationEditorObjectVisibilityController visibilityController
        {
            get
            {
                if (!m_visibilityController)
                {
                    m_visibilityController = base.GetComponent<PersonalizationEditorObjectVisibilityController>();
                }
                return m_visibilityController;
            }
        }

        private Volume m_volume;
        public Volume volume
        {
            get
            {
                if (!m_volume)
                {
                    m_volume = base.GetComponent<Volume>();
                }
                return m_volume;
            }
        }

        [PersonalizationEditorObjectProperty]
        public Dictionary<WeaponVariant, VolumeSettingsPreset> volumeSettingPresets
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue<Dictionary<WeaponVariant, VolumeSettingsPreset>>(nameof(volumeSettingPresets), null);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(volumeSettingPresets), value);
            }
        }

        private void Start()
        {
            if (volumeSettingPresets == null)
                volumeSettingPresets = new Dictionary<WeaponVariant, VolumeSettingsPreset>();
            else
            {
                foreach (VolumeSettingsPreset value in volumeSettingPresets.Values)
                    if (value.ReplaceWithFavoriteColors == null)
                        value.ReplaceWithFavoriteColors = new Dictionary<string, FavoriteColorSettings>();
            }

            EventController singleEventController = base.gameObject.AddComponent<EventController>();
            singleEventController.AddEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, RefreshVolume);

            RefreshVolume();
            if (PersonalizationEditorManager.IsInEditor())
            {
                GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshVolume);
                m_hasAddedEventListeners = true;
            }
        }

        private void OnDestroy()
        {
            if (m_hasAddedEventListeners)
            {
                m_hasAddedEventListeners = false;
                GlobalEventManager.Instance.RemoveEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshVolume);
            }
        }

        public WeaponVariant GetUnusedShowCondition()
        {
            if (!volumeSettingPresets.ContainsKey(WeaponVariant.IsNormal))
                return WeaponVariant.IsNormal;
            else if (!volumeSettingPresets.ContainsKey(WeaponVariant.IsOnFire))
                return WeaponVariant.IsOnFire;
            else if (!volumeSettingPresets.ContainsKey(WeaponVariant.IsNormalMultiplayer))
                return WeaponVariant.IsNormalMultiplayer;
            else if (!volumeSettingPresets.ContainsKey(WeaponVariant.IsOnFireMultiplayer))
                return WeaponVariant.IsOnFireMultiplayer;

            return WeaponVariant.None;
        }

        public WeaponVariant GetCurrentShowCondition()
        {
            if (PersonalizationEditorManager.IsInEditor())
            {
                return PersonalizationEditorManager.Instance.previewPresetKey;
            }
            visibilityController.GetWeaponVariant(out WeaponVariant showConditions);
            return showConditions;
        }

        public List<Dropdown.OptionData> GetUnusedConditionOptions()
        {
            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
            if (!volumeSettingPresets.ContainsKey(WeaponVariant.IsNormal))
                list.Add(new DropdownWeaponVariantOptionData(WeaponVariant.IsNormal));
            if (!volumeSettingPresets.ContainsKey(WeaponVariant.IsNormalMultiplayer))
                list.Add(new DropdownWeaponVariantOptionData(WeaponVariant.IsNormalMultiplayer));
            if (!volumeSettingPresets.ContainsKey(WeaponVariant.IsOnFire))
                list.Add(new DropdownWeaponVariantOptionData(WeaponVariant.IsOnFire));
            if (!volumeSettingPresets.ContainsKey(WeaponVariant.IsOnFireMultiplayer))
                list.Add(new DropdownWeaponVariantOptionData(WeaponVariant.IsOnFireMultiplayer));
            return list;
        }

        public VolumeSettingsPreset GetCurrentPreset()
        {
            WeaponVariant condition = GetCurrentShowCondition();
            Dictionary<WeaponVariant, VolumeSettingsPreset> d = volumeSettingPresets;
            if (d == null || d.Count == 0)
                return null;

            if (!d.ContainsKey(condition))
            {
                foreach (VolumeSettingsPreset value in d.Values)
                    return value;
            }
            return d[condition];
        }

        public void RefreshVolume()
        {
            VolumeSettingsPreset preset = GetCurrentPreset();

            Volume volumeComponent = volume;
            if (volumeComponent)
            {
                Vector3 vector = base.transform.localScale;
                base.transform.localScale = Vector3.one;

                if (volumeComponent.NumFrames > 0)
                {
                    System.Collections.Generic.List<Frame> list = volumeComponent.Frames;
                    for (int i = 0; i < volumeComponent.NumFrames; i++)
                        Destroy(list[i].gameObject);

                    list.Clear();
                }

                if (preset == null)
                {
                    volumeComponent.GenerateBasic(FillMode.None);
                    return;
                }

                string voxFilePath = preset.VoxFilePath;
                if (voxFilePath == null)
                    voxFilePath = string.Empty;

                string path = Path.Combine(objectBehaviour.ControllerInfo.ItemInfo.RootFolderPath, voxFilePath);
                if (!File.Exists(path))
                {
                    volumeComponent.GenerateBasic(FillMode.None);
                }
                else
                {
                    Color favoriteColor = objectBehaviour.ControllerInfo.Reference.owner.GetCharacterModel().GetFavouriteColor();

                    volumeComponent.AddFrame(0);
                    MagicaVoxelImporter.ImportModel(base.gameObject, path, "Import", preset.VoxelSize, preset.CenterPivot);
                    string cr = preset.ColorReplacements;

                    if (cr.IsNullOrEmpty())
                    {
                        List<Color32> colors = new List<Color32>();
                        Frame frame = volumeComponent.GetCurrentFrame();
                        int i = 0;
                        do
                        {
                            Voxel voxel = frame.Voxels[i];
                            Color32 color32 = voxel.Color;
                            if (voxel.Active && !colors.Contains(color32))
                            {
                                colors.Add(color32);
                            }
                            i++;
                        } while (i < frame.Voxels.Length);

                        List<ColorPairFloat> colorPairs = new List<ColorPairFloat>();
                        foreach (Color32 color in colors)
                            colorPairs.Add(new ColorPairFloat(color, color));

                        preset.ColorReplacements = PersonalizationEditorManager.Instance.GetStringFromColorPairs(colorPairs);
                    }
                    else
                    {
                        List<ColorPairFloat> list = PersonalizationEditorManager.Instance.GetColorPairsFromString(cr);
                        if (!list.IsNullOrEmpty())
                        {
                            foreach (ColorPairFloat cp in list)
                            {
                                Color colorB;
                                if (preset.ReplaceWithFavoriteColors != null && preset.ReplaceWithFavoriteColors.TryGetValue(ColorUtility.ToHtmlStringRGBA(cp.ColorA), out FavoriteColorSettings favoriteColorSettings))
                                {
                                    HSBColor hsbcolor = new HSBColor(favoriteColor);
                                    hsbcolor.s *= favoriteColorSettings.SaturationMultiplier;
                                    hsbcolor.b *= favoriteColorSettings.BrightnessMultiplier;
                                    colorB = hsbcolor.ToColor();
                                    colorB.a = Mathf.Clamp01(1f - favoriteColorSettings.GlowPercent);
                                }
                                else
                                {
                                    colorB = cp.ColorB;
                                }

                                ReplaceVoxelColor.ReplaceColors(volumeComponent, cp.ColorA, colorB, false);
                            }
                        }
                    }
                }
                base.transform.localScale = vector;
            }
        }
    }
}
