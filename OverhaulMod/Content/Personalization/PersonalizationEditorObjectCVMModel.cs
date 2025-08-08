using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectCVMModel : PersonalizationEditorObjectComponentBase
    {
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

        public Dictionary<WeaponVariant, CVMModelPreset> presets
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue<Dictionary<WeaponVariant, CVMModelPreset>>(nameof(presets), null);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(presets), value);
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

        private CVMImporter.SaveClass m_loadedModel;

        private bool m_hasAddedEvent;

        private void Start()
        {
            if (presets == null)
                presets = new Dictionary<WeaponVariant, CVMModelPreset>();

            if (PersonalizationEditorManager.IsInEditor())
            {
                GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, RefreshModel);
                GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshModel);
                m_hasAddedEvent = true;
            }
            RefreshModel();
        }

        private void OnDestroy()
        {
            if (m_hasAddedEvent)
            {
                GlobalEventManager.Instance.RemoveEventListener(PersonalizationEditorManager.PRESET_PREVIEW_CHANGED_EVENT, RefreshModel);
                GlobalEventManager.Instance.RemoveEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshModel);
                m_hasAddedEvent = false;
            }
        }

        public WeaponVariant GetUnusedShowCondition()
        {
            WeaponType weaponType = objectBehaviour.ControllerInfo.ItemInfo.Weapon;
            if (!presets.ContainsKey(WeaponVariant.Normal))
                return WeaponVariant.Normal;
            else if (!presets.ContainsKey(WeaponVariant.OnFire) && weaponType != WeaponType.Bow)
                return WeaponVariant.OnFire;
            else if (!presets.ContainsKey(WeaponVariant.NormalMultiplayer) && weaponType == WeaponType.Sword)
                return WeaponVariant.NormalMultiplayer;
            else if (!presets.ContainsKey(WeaponVariant.OnFireMultiplayer) && weaponType == WeaponType.Sword)
                return WeaponVariant.OnFireMultiplayer;

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

        public CVMModelPreset GetCurrentPreset()
        {
            WeaponVariant condition = GetCurrentShowCondition();
            Dictionary<WeaponVariant, CVMModelPreset> d = presets;
            if (d == null || d.Count == 0)
                return null;

            if (!d.ContainsKey(condition))
            {
                if (hideIfNoPreset)
                {
                    return null;
                }

                if (condition == WeaponVariant.OnFireMultiplayer && d.ContainsKey(WeaponVariant.OnFire))
                    return d[WeaponVariant.OnFire];
                else if (condition == WeaponVariant.NormalMultiplayer && d.ContainsKey(WeaponVariant.Normal))
                    return d[WeaponVariant.Normal];

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
            if (PersonalizationEditorManager.IsInEditor())
            {
                itemInfo = PersonalizationEditorManager.Instance.currentEditingItemInfo;
            }
            else
            {
                itemInfo = objectBehaviour.ControllerInfo.ItemInfo;
            }

            string path = Path.Combine(itemInfo.RootFolderPath, preset.CvmFilePath);
            if (m_loadedModel == null)
            {
                m_loadedModel = CVMImporter.LoadModel(path);
                if (m_loadedModel == null)
                    return;
            }

            _ = CVMImporter.InstantiateModel(m_loadedModel, preset.Weapon, preset.Variant, preset.ReplaceColors, preset.ShowFireParticles, t, out string error);
            if (PersonalizationEditorManager.IsInEditor() && !error.IsNullOrEmpty())
            {
                UIPersonalizationEditor.instance.ShowErrorNotification("CVM Error", error, 15f);
            }
        }
    }
}
