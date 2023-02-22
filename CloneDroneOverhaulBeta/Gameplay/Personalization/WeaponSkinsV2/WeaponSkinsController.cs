using BestHTTP.SocketIO;
using PlayFab.ExperimentationModels;
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

            _ = OverhaulEventManager.AddEventListener<Character>(GlobalEvents.CharacterStarted, ApplySkinsOnCharacter, true);
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

        public void ApplySkinsOnCharacter(Character character)
        {
            if(character == null || !(character is FirstPersonMover))
            {
                return;
            }
            FirstPersonMover firstPersonMover = character as FirstPersonMover;
            WeaponSkinsWearer wearer = firstPersonMover.GetComponent<WeaponSkinsWearer>();
            if(wearer == null)
            {
                wearer = firstPersonMover.gameObject.AddComponent<WeaponSkinsWearer>();
            }
            wearer.Owner = firstPersonMover;
            if (!firstPersonMover.IsPlayer())
            {
                return;
            }
            wearer.SpawnSkins();
        }

        public static WeaponVariant GetVariant(bool fire, bool multiplayer)
        {
            if (!fire && !multiplayer)
            {
                return WeaponVariant.Default;
            }
            else if (!fire && multiplayer)
            {
                return WeaponVariant.DefaultMultiplayer;
            }
            else if (fire && !multiplayer)
            {
                return WeaponVariant.Fire;
            }
            return WeaponVariant.FireMultiplayer;
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
            IWeaponSkinItemDefinition result = null;
            error = WeaponSkinItemNullReason.Null;

            foreach(IWeaponSkinItemDefinition weaponSkinItem in m_WeaponSkins)
            {
                if (weaponSkinItem.IsUnlocked(false))
                {
                    if ((filter == WeaponSkinItemFilter.Everything || weaponSkinItem.GetFilter() == filter) && weaponSkinItem.GetWeaponType() == weaponType && weaponSkinItem.GetItemName() == skinName)
                    {
                        result = weaponSkinItem;
                    }
                }
                else
                {
                    // Implement is locked by not exclusivity
                    error = WeaponSkinItemNullReason.LockedExclusive;
                }
            }

            return result;
        }

        IWeaponSkinItemDefinition[] IWeaponSkinsControllerV2.GetSkinItems(WeaponType weaponType, WeaponSkinItemFilter filter)
        {
            throw new System.NotImplementedException();
        }

        IWeaponSkinItemDefinition[] IWeaponSkinsControllerV2.GetSkinItems(WeaponSkinItemFilter filter)
        {
            List<IWeaponSkinItemDefinition> result = new List<IWeaponSkinItemDefinition>();
            foreach (IWeaponSkinItemDefinition weaponSkinItem in m_WeaponSkins)
            {
                if (weaponSkinItem.IsUnlocked(false) && (filter == WeaponSkinItemFilter.Everything || weaponSkinItem.GetFilter() == filter))
                {
                    result.Add(weaponSkinItem);
                }
            }
            return result.ToArray();
        }

        IWeaponSkinItemDefinition[] IWeaponSkinsControllerV2.GetSkinItems(FirstPersonMover firstPersonMover)
        {
            return Interface.GetSkinItems(WeaponSkinItemFilter.None);
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