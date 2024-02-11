using Steamworks;
using System;
using System.Collections.Generic;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemInfo
    {
        public string Name, Author, Description;

        public bool IsVerified; // is it custom?

        public List<string> ExclusiveFor;

        public PersonalizationCategory Category;

        public string EditorID;

        public int Version;

        [NonSerialized]
        public string FolderPath;

        public bool IsExclusive()
        {
            return ExclusiveFor != null && ExclusiveFor.Count != 0;
        }

        public bool CanBeEdited()
        {
            return string.IsNullOrEmpty(EditorID) || EditorID == SteamUser.GetSteamID().ToString();
        }

        public static PersonalizationItemInfo Create()
        {
            return new PersonalizationItemInfo();
        }
    }
}
