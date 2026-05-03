using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Content.Personalization.Objects
{
    public class PersonalizationEditorCVMModel : PersonalizationEditorComponent
    {
        private PersonalizationEditorVisibilityToggler _visibilityController;
        public PersonalizationEditorVisibilityToggler VisibilityController
        {
            get
            {
                if (!_visibilityController) _visibilityController = base.GetComponent<PersonalizationEditorVisibilityToggler>();
                return _visibilityController;
            }
        }

        public Dictionary<WeaponVariant2, CVMModelPreset> Presets
        {
            get => GetPropertyValue<Dictionary<WeaponVariant2, CVMModelPreset>>(nameof(PersonalizationEditorCVMModel), nameof(Presets), null);
            set => SetPropertyValue(nameof(PersonalizationEditorCVMModel), nameof(Presets), value);
        }

        public bool HideIfNoPreset
        {
            get => GetPropertyValue(nameof(PersonalizationEditorCVMModel), nameof(HideIfNoPreset), false);
            set => SetPropertyValue(nameof(PersonalizationEditorCVMModel), nameof(HideIfNoPreset), value);
        }

        private CVMImporter.SaveClass _loadedModel;

        private bool _hasAddedEvent;

        private void Start()
        {
            if (Presets == null)
                Presets = new Dictionary<WeaponVariant2, CVMModelPreset>();

            if (PersonalizationEditorManager.IsInEditorMode())
            {
                GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, RefreshModel);
                GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshModel);
                _hasAddedEvent = true;
            }
            RefreshModel();
        }

        private void OnDestroy()
        {
            if (_hasAddedEvent)
            {
                GlobalEventManager.Instance.RemoveEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, RefreshModel);
                GlobalEventManager.Instance.RemoveEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshModel);
                _hasAddedEvent = false;
            }
        }

        public WeaponVariant2 GetUnusedWeaponVariant()
        {
            WeaponType weaponType = PlacedObject.SpawnInfo.ItemInfo.Weapon;
            if (!Presets.ContainsKey(WeaponVariant2.Normal))
                return WeaponVariant2.Normal;
            else if (!Presets.ContainsKey(WeaponVariant2.OnFire) && weaponType != WeaponType.Bow)
                return WeaponVariant2.OnFire;
            else if (!Presets.ContainsKey(WeaponVariant2.NormalMultiplayer) && weaponType == WeaponType.Sword)
                return WeaponVariant2.NormalMultiplayer;
            else if (!Presets.ContainsKey(WeaponVariant2.OnFireMultiplayer) && weaponType == WeaponType.Sword)
                return WeaponVariant2.OnFireMultiplayer;

            return WeaponVariant2.None;
        }

        public WeaponVariant2 GetActiveWeaponVariant()
        {
            if (PersonalizationEditorManager.IsInEditorMode())
            {
                return PersonalizationEditorManager.Instance.PreviewPresetKey;
            }
            VisibilityController.GetWeaponVariant(out WeaponVariant2 showConditions);
            return showConditions;
        }

        public CVMModelPreset GetCurrentPreset()
        {
            WeaponVariant2 condition = GetActiveWeaponVariant();
            Dictionary<WeaponVariant2, CVMModelPreset> d = Presets;
            if (d == null || d.Count == 0)
                return null;

            if (!d.ContainsKey(condition))
            {
                if (HideIfNoPreset)
                {
                    return null;
                }

                if (condition == WeaponVariant2.OnFireMultiplayer && d.ContainsKey(WeaponVariant2.OnFire))
                    return d[WeaponVariant2.OnFire];
                else if (condition == WeaponVariant2.NormalMultiplayer && d.ContainsKey(WeaponVariant2.Normal))
                    return d[WeaponVariant2.Normal];

                foreach (CVMModelPreset value in d.Values)
                    return value;
            }
            return d[condition];
        }

        public void RefreshModel()
        {
            refreshModel();
        }

        private void refreshModel()
        {
            Transform t = base.transform;
            if (t.childCount != 0)
                TransformUtils.DestroyAllChildren(t);

            CVMModelPreset preset = GetCurrentPreset();
            if (preset == null || preset.CvmFilePath.IsNullOrEmpty())
                return;

            PersonalizationItemInfo itemInfo;
            if (PersonalizationEditorManager.IsInEditorMode())
            {
                itemInfo = PersonalizationEditorManager.Instance.EditingItemInfo;
            }
            else
            {
                itemInfo = PlacedObject.SpawnInfo.ItemInfo;
            }

            string path = Path.Combine(itemInfo.RootFolderPath, preset.CvmFilePath);
            if (_loadedModel == null)
            {
                _loadedModel = CVMImporter.LoadModel(path);
                if (_loadedModel == null)
                    return;
            }

            _ = CVMImporter.InstantiateModel(_loadedModel, preset.Weapon, preset.Variant, preset.ReplaceColors, preset.ShowFireParticles, t, out string error);
            if (PersonalizationEditorManager.IsInEditorMode() && !error.IsNullOrEmpty())
            {
                UIPE.Instance.ShowErrorNotification("CVM Error", error, 15f);
            }
        }
    }
}
