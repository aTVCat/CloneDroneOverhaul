﻿using CDOverhaul.Gameplay.Editors.Personalization;
using System.Collections.Generic;
using System.Reflection;

namespace CDOverhaul.Gameplay
{
    public abstract class PersonalizationItem : OverhaulDisposable
    {
        public const string NO_DESCRIPTION_PROVIDED = "No description provided";
        public const string NO_AUTHOR_SPECIFIED = "N/A";
        public const string NO_NAME_SPECIFIED = "Unknown item";

        [PersonalizationEditorProperty("Generic Info")]
        public string Name = NO_NAME_SPECIFIED;
        [PersonalizationEditorProperty("Generic Info", true)]
        public string Description = NO_DESCRIPTION_PROVIDED;
        [PersonalizationEditorProperty("Generic Info")]
        public string Author = NO_AUTHOR_SPECIFIED;
        [PersonalizationEditorProperty("Generic Info", true)]
        public string ExclusiveFor;

        public bool IsUnlocked()
        {
            if (OverhaulVersion.IsDebugBuild || string.IsNullOrEmpty(ExclusiveFor))
                return true;

            string localPlayFabID = OverhaulPlayerIdentifier.GetLocalPlayFabID();
            string localSteamID = OverhaulPlayerIdentifier.GetLocalSteamID();

            bool isUnlocked = IsUnlockedFor(localPlayFabID, localSteamID);
            if (!isUnlocked && OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IS_DEVELOPER_ALLOWED_TO_USE_LOCKED_STUFF)
                isUnlocked = localPlayFabID.Equals("883CC7F4CA3155A3");

            return isUnlocked;
        }
        public bool IsUnlockedFor(string playFabID, string steamID)
        {
            return ExclusiveFor.Contains(playFabID) || ExclusiveFor.Contains(steamID);
        }

        public static List<PersonalizationEditorPropertyAttribute> GetAllFields<T>() where T : PersonalizationItem
        {
            List<PersonalizationEditorPropertyAttribute> result = new List<PersonalizationEditorPropertyAttribute>();
            foreach (FieldInfo info in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                PersonalizationEditorPropertyAttribute attribute = info.GetCustomAttribute<PersonalizationEditorPropertyAttribute>();
                if (attribute != null)
                {
                    attribute.FieldReference = info;
                    result.Add(attribute);
                }
            }
            return result;
        }
    }
}
