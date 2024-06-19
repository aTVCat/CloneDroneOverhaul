using OverhaulMod.Engine;
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
        public Dictionary<PersonalizationEditorObjectShowConditions, VolumeSettingsPreset> volumeSettingPresets
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue<Dictionary<PersonalizationEditorObjectShowConditions, VolumeSettingsPreset>>(nameof(volumeSettingPresets), null);
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
                volumeSettingPresets = new Dictionary<PersonalizationEditorObjectShowConditions, VolumeSettingsPreset>();

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

        public PersonalizationEditorObjectShowConditions GetUnusedShowCondition()
        {
            if (!volumeSettingPresets.ContainsKey(PersonalizationEditorObjectShowConditions.IsNormal))
                return PersonalizationEditorObjectShowConditions.IsNormal;
            else if (!volumeSettingPresets.ContainsKey(PersonalizationEditorObjectShowConditions.IsOnFire))
                return PersonalizationEditorObjectShowConditions.IsOnFire;
            else if (!volumeSettingPresets.ContainsKey(PersonalizationEditorObjectShowConditions.IsNormalMultiplayer))
                return PersonalizationEditorObjectShowConditions.IsNormalMultiplayer;
            else if (!volumeSettingPresets.ContainsKey(PersonalizationEditorObjectShowConditions.IsOnFireMultiplayer))
                return PersonalizationEditorObjectShowConditions.IsOnFireMultiplayer;

            return PersonalizationEditorObjectShowConditions.None;
        }

        public PersonalizationEditorObjectShowConditions GetCurrentShowCondition()
        {
            if (PersonalizationEditorManager.IsInEditor())
            {
                return PersonalizationEditorManager.Instance.previewPresetKey;
            }
            visibilityController.GetWeaponProperties(out PersonalizationEditorObjectShowConditions showConditions);
            return showConditions;
        }

        public List<Dropdown.OptionData> GetUnusedConditionOptions()
        {
            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
            if (!volumeSettingPresets.ContainsKey(PersonalizationEditorObjectShowConditions.IsNormal))
                list.Add(new DropdownShowConditionOptionData(PersonalizationEditorObjectShowConditions.IsNormal));
            if (!volumeSettingPresets.ContainsKey(PersonalizationEditorObjectShowConditions.IsNormalMultiplayer))
                list.Add(new DropdownShowConditionOptionData(PersonalizationEditorObjectShowConditions.IsNormalMultiplayer));
            if (!volumeSettingPresets.ContainsKey(PersonalizationEditorObjectShowConditions.IsOnFire))
                list.Add(new DropdownShowConditionOptionData(PersonalizationEditorObjectShowConditions.IsOnFire));
            if (!volumeSettingPresets.ContainsKey(PersonalizationEditorObjectShowConditions.IsOnFireMultiplayer))
                list.Add(new DropdownShowConditionOptionData(PersonalizationEditorObjectShowConditions.IsOnFireMultiplayer));
            return list;
        }

        public VolumeSettingsPreset GetCurrentPreset()
        {
            PersonalizationEditorObjectShowConditions condition = GetCurrentShowCondition();
            Dictionary<PersonalizationEditorObjectShowConditions, VolumeSettingsPreset> d = volumeSettingPresets;
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

                string path = $"{ModCore.customizationFolder}{preset.VoxFilePath}";
                if (!File.Exists(path))
                {
                    volumeComponent.GenerateBasic(FillMode.None);
                }
                else
                {
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
                                ReplaceVoxelColor.ReplaceColors(volumeComponent, cp.ColorA, cp.ColorB, false);
                        }
                    }
                }
                base.transform.localScale = vector;
            }
        }
    }
}
