using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections.Generic;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationItemInfo
    {
        public string Name, Description;

        public bool IsVerified; // is it custom?

        public List<string> Authors;

        public List<string> ExclusiveFor;

        public PersonalizationCategory Category;

        public string EditorID;

        public int Version;

        [NonSerialized]
        public string FolderPath;

        public void FixValues()
        {
            if (Authors == null)
                Authors = new List<string>();

            if (ExclusiveFor == null)
                ExclusiveFor = new List<string>();
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
            if(Authors.Count != 1)
            {
                for(int i = 1; i < Authors.Count; i++)
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

        public static PersonalizationItemInfo Create()
        {
            return new PersonalizationItemInfo();
        }
    }
}
