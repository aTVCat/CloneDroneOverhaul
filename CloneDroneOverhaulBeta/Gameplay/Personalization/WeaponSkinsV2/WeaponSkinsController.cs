using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinsControllerV2 : ModController, IWeaponSkinsControllerV2, IConsoleCommandReceiver
    {
        public IWeaponSkinsControllerV2 Interface
        {
            get;
            private set;
        }

        private static readonly List<IWeaponSkinItemDefinition> m_WeaponSkins = new List<IWeaponSkinItemDefinition>();

        public override void Initialize()
        {
            Interface = this;
            addSkins();
        }
        private void addSkins()
        {
            bool hasAdded = OverhaulSessionController.GetKey<bool>("hasAddedSkins");
            if (!hasAdded)
            {
                OverhaulSessionController.SetKey("hasAddedSkins", true);

                IWeaponSkinItemDefinition swordDetailedSkin = Interface.NewSkinItem(WeaponType.Sword, "Detailed", WeaponSkinItemFilter.None);
                swordDetailedSkin.SetModel(AssetController.GetAsset("SwordSkinDetailedOne", Enumerators.EModAssetBundlePart.WeaponSkins),
                    new WeaponSkinModelOffset(new Vector3(0, 0, -0.7f),
                    new Vector3(0, 270, 270),
                    new Vector3(0.55f, 0.55f, 0.55f)),
                    false,
                    false);
            }
        }

        IWeaponSkinItemDefinition IWeaponSkinsControllerV2.NewSkinItem(WeaponType weapon, string skinName, WeaponSkinItemFilter filter)
        {
            IWeaponSkinItemDefinition newWeaponItem = new WeaponSkinItemDefinitionV2();
            newWeaponItem.SetItemName(skinName);
            newWeaponItem.SetWeaponType(weapon);
            newWeaponItem.SetFilter(filter);
            m_WeaponSkins.Add(newWeaponItem);
            return newWeaponItem;
        }

        IWeaponSkinItemDefinition IWeaponSkinsControllerV2.GetSkinItem(WeaponType weaponType, string skinName, WeaponSkinItemFilter filter, out WeaponSkinItemNullReason error)
        {
            throw new System.NotImplementedException();
        }

        IWeaponSkinItemDefinition[] IWeaponSkinsControllerV2.GetSkinItems(WeaponType weaponType, WeaponSkinItemFilter filter)
        {
            throw new System.NotImplementedException();
        }

        IWeaponSkinItemDefinition[] IWeaponSkinsControllerV2.GetSkinItems(WeaponSkinItemFilter filter)
        {
            throw new System.NotImplementedException();
        }

        IWeaponSkinItemDefinition[] IWeaponSkinsControllerV2.GetSkinItems(FirstPersonMover firstPersonMover)
        {
            throw new System.NotImplementedException();
        }

        string[] IConsoleCommandReceiver.Commands()
        {
            throw new System.NotImplementedException();
        }

        string IConsoleCommandReceiver.OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}