using CDOverhaul.Gameplay;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace CDOverhaul.HUD
{
    public class UISkinEntry : MonoBehaviour
    {
        private UISkinSelection _ui;
        private WeaponSkinModels _mySkin;

        public WeaponType WeaponType { get; set; }
        public string SkinName { get; set; }

        public void Initialize(in WeaponSkinModels skin, in UISkinSelection ui, in WeaponType weaponType)
        {
            _mySkin = skin;
            _ui = ui;

            ModdedObject m = base.GetComponent<ModdedObject>();
            m.GetObject<Text>(1).text = skin.SkinName;

            string file = WeaponSkinModels.SkinPreviewDirectory + weaponType.ToString() + "_" + skin.SkinName + ".jpg";
            if (OverhaulUtilities.FileUtils.FileExists(file))
            OverhaulUtilities.TextureAndMaterialUtils.LoadTextureAsync(file, delegate(Texture2D tex) 
            {
                m.GetObject<Image>(0).sprite = OverhaulUtilities.TextureAndMaterialUtils.FastSpriteCreate(tex);
            });

            WeaponType = weaponType;
            SkinName = skin.SkinName;

            base.GetComponent<Button>().onClick.AddListener(OnClick);
            VisualizeDeselect();
        }

        public void OnClick()
        {
            _ui.OnSelectSkin(this);
        }

        public void VisualizeSelect()
        {
        }

        public void VisualizeDeselect()
        {
        }
    }
}