using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemInfo
    {
        public string Name, Description;

        public bool IsVerified;

        public string ItemID, EditorID;

        public List<string> Authors;

        public List<string> ExclusiveFor;

        public PersonalizationCategory Category;

        public WeaponType Weapon;

        public string BodyPartName;

        public int Version;

        public PersonalizationEditorObjectInfo RootObject;

        [NonSerialized]
        public string FolderPath;

        [NonSerialized]
        public List<string> ImportedFiles;

        public void LoadImportedFilePaths()
        {
            List<string> list = new List<string>();
            foreach (string f in Directory.GetFiles(GetImportedFilesFolder(this)))
                list.Add(Path.GetFileName(f));

            ImportedFiles = list;
        }

        public void FixValues()
        {
            if (Authors == null)
                Authors = new List<string>();

            if (ExclusiveFor == null)
                ExclusiveFor = new List<string>();

            if (RootObject == null)
            {
                RootObject = new PersonalizationEditorObjectInfo()
                {
                    Name = "Root",
                    Path = "Empty",
                    IsRoot = true,
                    Children = new List<PersonalizationEditorObjectInfo>(),
                    PropertyValues = new Dictionary<string, object>()
                };
            }

            RootObject.SetPosition(Vector3.zero);
            RootObject.SetEulerAngles(Vector3.zero);
            RootObject.SetScale(Vector3.one);

            if (!PersonalizationManager.IsWeaponCustomizationSupported(Weapon))
                Weapon = WeaponType.Sword;

            LoadImportedFilePaths();
        }

        public void SetAuthor(string name)
        {
            Authors.Clear();
            Authors.Add(name);
        }

        public string GetAuthorsString()
        {
            if (Authors.IsNullOrEmpty())
                return "n/a";

            string result = Authors[0];
            if (Authors.Count != 1)
            {
                for (int i = 1; i < Authors.Count; i++)
                    result += $", {Authors[i]}";
            }
            return result;
        }

        public bool IsExclusive()
        {
            return ExclusiveFor != null && ExclusiveFor.Count != 0;
        }

        public bool CanBeEdited()
        {
            return string.IsNullOrEmpty(EditorID) || EditorID.Contains(SteamUser.GetSteamID().ToString());
        }

        public bool IsUnlocked(Character character)
        {
            if (!character || !character.IsAttachedAndAlive())
                return false; 

            if (!IsExclusive())
                return true;

            bool isMainPlayer = character.IsMainPlayer();
            string playFabId = isMainPlayer ? ModUserInfo.localPlayerPlayFabID : character.GetPlayFabID();
            CSteamID steamId = isMainPlayer ? ModUserInfo.localPlayerSteamID : default;

            bool result = (!playFabId.IsNullOrEmpty() ? ExclusiveFor.Contains(playFabId) : false) || (steamId != default ? ExclusiveFor.Contains(steamId.ToString()) : false);

            return character && (result || character.IsClone() || !character.IsMainPlayer());
        }

        public bool IsUnlocked()
        {
            return IsUnlocked(CharacterTracker.Instance.GetPlayer());
        }

        public static string GetFolderName(PersonalizationItemInfo personalizationItemInfo)
        {
            return ModIOUtils.GetDirectoryName(personalizationItemInfo.FolderPath);
        }

        public static string GetImportedFilesFolder(PersonalizationItemInfo personalizationItemInfo)
        {
            return $"{personalizationItemInfo.FolderPath}files/";
        }

        public static string GetCategoryString(PersonalizationCategory personalizationCategory, bool many = false)
        {
            switch (personalizationCategory)
            {
                case PersonalizationCategory.None:
                    return "None";
                case PersonalizationCategory.WeaponSkins:
                    return many ? "Weapon skins" : "Weapon skin";
                case PersonalizationCategory.Accessories:
                    return many ? "Accessories" : "Accessory";
                case PersonalizationCategory.Pets:
                    return many ? "Pets" : "Pet";
            }
            return personalizationCategory.ToString();
        }
    }
}
