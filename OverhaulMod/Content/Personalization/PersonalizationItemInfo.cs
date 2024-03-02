﻿using OverhaulMod.Utils;
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

        public List<string> ImportedFiles;

        [NonSerialized]
        public string FolderPath;

        public string importedFilesFolder
        {
            get
            {
                return FolderPath + "files/";
            }
        }

        public string folderName
        {
            get
            {
                return Path.GetDirectoryName(FolderPath);
            }
        }

        public string folderNameWithSlash
        {
            get
            {
                return $"{Path.GetDirectoryName(FolderPath)}/";
            }
        }

        public void FixValues()
        {
            if (Authors == null)
                Authors = new List<string>();

            if (ExclusiveFor == null)
                ExclusiveFor = new List<string>();

            if (ImportedFiles == null)
                ImportedFiles = new List<string>();

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
                RootObject.SetPosition(Vector3.zero);
                RootObject.SetEulerAngles(Vector3.zero);
                RootObject.SetScale(Vector3.one);
            }

            if (!PersonalizationManager.IsWeaponCustomizationSupported(Weapon))
                Weapon = WeaponType.Sword;
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
            return string.IsNullOrEmpty(EditorID) || EditorID == SteamUser.GetSteamID().ToString();
        }
    }
}
