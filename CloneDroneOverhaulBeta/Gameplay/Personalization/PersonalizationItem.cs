using CDOverhaul.Gameplay.Editors.Personalization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CDOverhaul.Gameplay
{
    public class PersonalizationItem : OverhaulDisposable
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
        public List<string> ExclusiveFor;

        public virtual void OnDeserialized() { }

        public bool IsUnlocked()
        {
            if (OverhaulVersion.IsDebugBuild || ExclusiveFor.IsNullOrEmpty())
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
            foreach(string id in ExclusiveFor)
            {
                string justId = PersonalizationEditor.GetOnlyID(id, out byte type);
                if(justId == playFabID || justId == steamID)
                    return true;
            }
            return false;
        }

        public object GetTargetObjectOfSubClass(string name)
        {
            Type type = GetType();
            FieldInfo info = type.GetField(name, BindingFlags.Instance | BindingFlags.Public);
            object value = info.GetValue(this);

            if(value == null)
            {
                value = Activator.CreateInstance(info.FieldType);
                info.SetValue(this, value);
            }
            return value;
        }

        public static List<PersonalizationEditorPropertyAttribute> GetAllFields(Type type)
        {
            List<PersonalizationEditorPropertyAttribute> result = new List<PersonalizationEditorPropertyAttribute>();
            foreach (FieldInfo info in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                PersonalizationEditorPropertyAttribute attribute = info.GetCustomAttribute<PersonalizationEditorPropertyAttribute>();
                if (attribute != null)
                {
                    attribute.FieldReference = info;
                    result.Add(attribute);
                }

                PersonalizationEditorSubPropertyAttribute subAttribute = info.GetCustomAttribute<PersonalizationEditorSubPropertyAttribute>();
                if (subAttribute != null)
                {
                    List<PersonalizationEditorPropertyAttribute> sub = GetAllFields(info.FieldType);
                    if (!sub.IsNullOrEmpty())
                    {
                        foreach (PersonalizationEditorPropertyAttribute att in sub)
                            att.SubClassFieldName = info.Name;

                        result.AddRange(sub);
                    }
                }
            }
            return result;
        }
    }
}
