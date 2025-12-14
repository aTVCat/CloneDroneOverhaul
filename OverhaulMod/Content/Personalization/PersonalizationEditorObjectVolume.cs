using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using PicaVoxel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectVolume : PersonalizationEditorObjectComponentBase
    {
        public static readonly MeshingMode VolumeMeshingMode = MeshingMode.Culled;

        private static readonly Dictionary<WeaponVariant2, VolumeSettingsPreset> s_emptySettingsDictionary = new Dictionary<WeaponVariant2, VolumeSettingsPreset>();

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

        public Dictionary<WeaponVariant2, VolumeSettingsPreset> volumeSettingPresets
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue<Dictionary<WeaponVariant2, VolumeSettingsPreset>>(nameof(volumeSettingPresets), null);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(volumeSettingPresets), value);
            }
        }

        public bool hideIfNoPreset
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(hideIfNoPreset), false);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(hideIfNoPreset), value);
            }
        }

        private bool m_hasAddedEventListeners;

        private bool m_hasStarted, m_isDestroyed;

        private bool m_aboutToRefreshVolume;

        private void Start()
        {
            m_hasStarted = true;

            if (volumeSettingPresets == null)
                volumeSettingPresets = new Dictionary<WeaponVariant2, VolumeSettingsPreset>();
            else
            {
                foreach (VolumeSettingsPreset value in volumeSettingPresets.Values)
                    if (value.ReplaceWithFavoriteColors == null)
                        value.ReplaceWithFavoriteColors = new Dictionary<string, FavoriteColorSettings>();
            }

            RefreshVolume();
            if (PersonalizationEditorManager.IsInEditor())
            {
                GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, RefreshVolume);
                GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshVolume);
                m_hasAddedEventListeners = true;
            }
        }

        private void OnDestroy()
        {
            m_isDestroyed = true;
            if (m_hasAddedEventListeners)
            {
                m_hasAddedEventListeners = false;
                GlobalEventManager.Instance.RemoveEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, RefreshVolume);
                GlobalEventManager.Instance.RemoveEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshVolume);
            }
        }

        public WeaponVariant2 GetUnusedShowCondition()
        {
            WeaponType weaponType = objectBehaviour.ControllerInfo.ItemInfo.Weapon;
            if (!volumeSettingPresets.ContainsKey(WeaponVariant2.Normal))
                return WeaponVariant2.Normal;
            else if (!volumeSettingPresets.ContainsKey(WeaponVariant2.OnFire) && weaponType != WeaponType.Bow)
                return WeaponVariant2.OnFire;
            else if (!volumeSettingPresets.ContainsKey(WeaponVariant2.NormalMultiplayer) && weaponType == WeaponType.Sword)
                return WeaponVariant2.NormalMultiplayer;
            else if (!volumeSettingPresets.ContainsKey(WeaponVariant2.OnFireMultiplayer) && weaponType == WeaponType.Sword)
                return WeaponVariant2.OnFireMultiplayer;

            return WeaponVariant2.None;
        }

        public WeaponVariant2 GetCurrentShowCondition()
        {
            if (PersonalizationEditorManager.IsInEditor())
            {
                return PersonalizationEditorManager.Instance.previewPresetKey;
            }
            visibilityController.GetWeaponVariant(out WeaponVariant2 showConditions);
            return showConditions;
        }

        public VolumeSettingsPreset GetCurrentPreset()
        {
            WeaponVariant2 condition = GetCurrentShowCondition();
            Dictionary<WeaponVariant2, VolumeSettingsPreset> d = volumeSettingPresets;
            if (d == null || d.Count == 0)
                return null;

            if (!d.ContainsKey(condition))
            {
                if (hideIfNoPreset)
                {
                    return null;
                }

                if (condition == WeaponVariant2.OnFireMultiplayer && d.ContainsKey(WeaponVariant2.OnFire))
                    return d[WeaponVariant2.OnFire];
                else if (condition == WeaponVariant2.NormalMultiplayer && d.ContainsKey(WeaponVariant2.Normal))
                    return d[WeaponVariant2.Normal];

                foreach (VolumeSettingsPreset value in d.Values)
                    return value;
            }
            return d[condition];
        }

        public void RefreshVolume()
        {
            if (m_aboutToRefreshVolume || m_isDestroyed)
                return;

            m_aboutToRefreshVolume = true;
            _ = refreshVolumeCoroutine().Run();
        }

        private IEnumerator refreshVolumeCoroutine() // this fixes weird crash
        {
            while (!m_isDestroyed && !PersonalizationEditorManager.IsInEditor() && (!objectBehaviour || objectBehaviour.ControllerInfo == null))
                yield return null;

            if (m_isDestroyed)
                yield break;

            m_aboutToRefreshVolume = false;
            refreshVolume();
            yield break;
        }

        private void refreshVolume()
        {
            if (!m_hasStarted)
                return;

            VolumeSettingsPreset preset = GetCurrentPreset();

            Volume volumeComponent = volume;
            if (volumeComponent)
            {
                volumeComponent.MeshingMode = VolumeMeshingMode;
                //volumeComponent.CollisionMode = CollisionMode.None;

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
                    base.transform.localScale = objectBehaviour.SerializedScale;
                    return;
                }

                string voxFilePath = preset.VoxFilePath;
                if (voxFilePath == null)
                    voxFilePath = string.Empty;

                bool inEditor = PersonalizationEditorManager.IsInEditor();

                PersonalizationItemInfo itemInfo;
                if (inEditor)
                {
                    itemInfo = PersonalizationEditorManager.Instance.currentEditingItemInfo;
                }
                else
                {
                    itemInfo = objectBehaviour.ControllerInfo.ItemInfo;
                }

                string path = Path.Combine(itemInfo.RootFolderPath, voxFilePath);

                volumeComponent.AddFrame(0);
                if (inEditor || !PersonalizationCacheManager.Instance.TryGet(path, out byte[] array))
                {
                    if (!File.Exists(path))
                    {
                        volumeComponent.GenerateBasic(FillMode.None);
                        base.transform.localScale = objectBehaviour.SerializedScale;
                        return;
                    }
                    else
                    {
                        MagicaVoxelImporter.ImportModel(base.gameObject, path, "Import", preset.VoxelSize, preset.CenterPivot);
                        if (inEditor)
                        {
                            Frame frame = volumeComponent.GetCurrentFrame();
                            int allDimensions = frame.XSize + frame.YSize + frame.ZSize;
                            if (allDimensions > 150)
                            {
                                UIPersonalizationEditor.instance.ShowNotification("Performance warning", "This model is very huge and it can cause lags.\nIt's recommended to simplify this model.", ModParseUtils.TryParseToColor("#4C3D00", Color.yellow), 30f);
                            }
                        }
                    }
                }
                else
                {
                    ModDebug.Log("Loaded the model from memory");

                    MagicaVoxelImporter.ImportModelFromMemory(base.gameObject, array, "Import", preset.VoxelSize, preset.CenterPivot);
                }

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
                    Color favoriteColor = objectBehaviour.ControllerInfo.GetFavoriteColor();

                    List<ColorPairFloat> list = PersonalizationEditorManager.Instance.GetColorPairsFromString(cr);
                    if (!list.IsNullOrEmpty())
                    {
                        foreach (ColorPairFloat cp in list)
                        {
                            Color colorB;
                            if (preset.ReplaceWithFavoriteColors != null && preset.ReplaceWithFavoriteColors.TryGetValue(ColorUtility.ToHtmlStringRGBA(cp.ColorA), out FavoriteColorSettings favoriteColorSettings))
                            {
                                HSBColor hsbcolor = new HSBColor(favoriteColor)
                                {
                                    s = favoriteColorSettings.SaturationMultiplier,
                                    b = favoriteColorSettings.BrightnessMultiplier
                                };
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

                base.transform.localScale = objectBehaviour.SerializedScale;
            }
        }
    }
}
