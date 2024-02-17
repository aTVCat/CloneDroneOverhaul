﻿using OverhaulMod.Utils;
using Steamworks;
using System.IO;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationManager : Singleton<PersonalizationManager>
    {
        public const string ITEM_INFO_FILE = "itemInfo.json";

        public PersonalizationItemList itemList
        {
            get;
            private set;
        }

        public override void Awake()
        {
            base.Awake();

            PersonalizationItemList personalizationItemList = new PersonalizationItemList();
            personalizationItemList.Load();
            itemList = personalizationItemList;
        }

        public void ConfigureFirstPersonMover(FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover || !firstPersonMover.IsAlive())
                return;

            _ = firstPersonMover.gameObject.AddComponent<PersonalizationController>();
        }

        public bool CreateItem(string name, out PersonalizationItemInfo personalizationItem)
        {
            personalizationItem = null;
            if (name.IsNullOrEmpty())
                return false;

            string directoryName = name.Replace(" ", string.Empty);
            string directoryPath = ModCore.customizationFolder + directoryName + "/";

            if (Directory.Exists(directoryPath))
                return false;

            _ = Directory.CreateDirectory(directoryPath);
            personalizationItem = new PersonalizationItemInfo()
            {
                Name = "New item",
                Author = SteamFriends.GetPersonaName(),
                Description = "No description provided.",
                IsVerified = PersonalizationEditorManager.Instance.canVerifyItems,
                ExclusiveFor = new System.Collections.Generic.List<string>(),
                Category = PersonalizationCategory.WeaponSkins,
                EditorID = PersonalizationEditorManager.Instance.editorId,
                FolderPath = directoryPath
            };
            itemList.Items.Add(personalizationItem);

            ModJsonUtils.WriteStream(directoryPath + ITEM_INFO_FILE, personalizationItem);
            return true;
        }

        public static bool IsWeaponCustomizationSupported(WeaponType weaponType)
        {
            return weaponType != WeaponType.None
&& (weaponType == WeaponType.Sword
                || weaponType == WeaponType.Bow
                || weaponType == WeaponType.Hammer
                || weaponType == WeaponType.Spear
                || weaponType == WeaponType.Shield);
        }
    }
}
