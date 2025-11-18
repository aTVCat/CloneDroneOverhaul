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

        public Dictionary<WeaponVariant2, CVMModelPreset> presets
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue<Dictionary<WeaponVariant2, CVMModelPreset>>(nameof(presets), null);
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
                presets = new Dictionary<WeaponVariant2, CVMModelPreset>();

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

        public WeaponVariant2 GetUnusedShowCondition()
        {
            WeaponType weaponType = objectBehaviour.ControllerInfo.ItemInfo.Weapon;
            if (!presets.ContainsKey(WeaponVariant2.Normal))
                return WeaponVariant2.Normal;
            else if (!presets.ContainsKey(WeaponVariant2.OnFire) && weaponType != WeaponType.Bow)
                return WeaponVariant2.OnFire;
            else if (!presets.ContainsKey(WeaponVariant2.NormalMultiplayer) && weaponType == WeaponType.Sword)
                return WeaponVariant2.NormalMultiplayer;
            else if (!presets.ContainsKey(WeaponVariant2.OnFireMultiplayer) && weaponType == WeaponType.Sword)
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

        public CVMModelPreset GetCurrentPreset()
        {
            WeaponVariant2 condition = GetCurrentShowCondition();
            Dictionary<WeaponVariant2, CVMModelPreset> d = presets;
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
