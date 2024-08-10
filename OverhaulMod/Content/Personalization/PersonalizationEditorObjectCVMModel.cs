using OverhaulMod.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [PersonalizationEditorObjectProperty]
        public Dictionary<WeaponVariant, CVMModelPreset> volumeSettingPresets
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue<Dictionary<WeaponVariant, CVMModelPreset>>(nameof(volumeSettingPresets), null);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(volumeSettingPresets), value);
            }
        }

        [PersonalizationEditorObjectProperty]
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

        private void Start()
        {
            if (volumeSettingPresets == null)
                volumeSettingPresets = new Dictionary<WeaponVariant, CVMModelPreset>();
        }

        public WeaponVariant GetUnusedShowCondition()
        {
            WeaponType weaponType = objectBehaviour.ControllerInfo.ItemInfo.Weapon;
            if (!volumeSettingPresets.ContainsKey(WeaponVariant.Normal))
                return WeaponVariant.Normal;
            else if (!volumeSettingPresets.ContainsKey(WeaponVariant.OnFire) && weaponType != WeaponType.Bow)
                return WeaponVariant.OnFire;
            else if (!volumeSettingPresets.ContainsKey(WeaponVariant.NormalMultiplayer) && weaponType == WeaponType.Sword)
                return WeaponVariant.NormalMultiplayer;
            else if (!volumeSettingPresets.ContainsKey(WeaponVariant.OnFireMultiplayer) && weaponType == WeaponType.Sword)
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
            Dictionary<WeaponVariant, CVMModelPreset> d = volumeSettingPresets;
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


        }
    }
}
