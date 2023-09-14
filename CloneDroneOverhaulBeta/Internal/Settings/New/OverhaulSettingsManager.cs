using System;
using System.Collections.Generic;
using System.Reflection;

namespace CDOverhaul
{
    public class OverhaulSettingsManager : OverhaulManager<OverhaulSettingsManager>
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public List<OverhaulSettingInfo> Settings;

        public override void Initialize()
        {
            base.Initialize();
            Settings = GetNewListOfSettings();
        }

        public List<OverhaulSettingInfo> GetNewListOfSettings()
        {
            List<OverhaulSettingInfo> result = new List<OverhaulSettingInfo>();

            Type[] allTypes = OverhaulMod.GetAllTypes();
            if (allTypes.IsNullOrEmpty())
                return result;

            int typeIndex = 0;
            do
            {
                Type currentType = allTypes[typeIndex];
                FieldInfo[] allFields = currentType.GetFields(BINDING_FLAGS);
                if (allFields.IsNullOrEmpty())
                {
                    typeIndex++;
                    continue;
                }

                int fieldIndex = 0;
                do
                {
                    FieldInfo currentField = allFields[fieldIndex];
                    OverhaulSettingInfo settingInfo = CreateSettingInfo(currentField);
                    if (settingInfo == null)
                    {
                        fieldIndex++;
                        continue;
                    }

                    result.Add(settingInfo);

                    fieldIndex++;
                } while (fieldIndex < allFields.Length);

                typeIndex++;
            } while (typeIndex < allTypes.Length);

            return result;
        }

        public OverhaulSettingInfo CreateSettingInfo(FieldInfo fieldInfo)
        {
            OverhaulSettingAttribute settingAttribute = fieldInfo.GetCustomAttribute<OverhaulSettingAttribute>();
            if (settingAttribute == null)
                return null;

            OverhaulSettingInfo result = new OverhaulSettingInfo()
            {
                path = settingAttribute.Path,
                fieldReference = fieldInfo,
                defaultValue = fieldInfo.GetValue(null),
                isHidden = settingAttribute.IsHidden
            };
            result.fieldReference.SetValue(null, result.GetValue<object>());

            result.AddAttribute(settingAttribute);
            foreach (OverhaulSettingBaseAttribute attribute in fieldInfo.GetCustomAttributes<OverhaulSettingBaseAttribute>())
            {
                result.AddAttribute(attribute);
            }
            return result;
        }

        public OverhaulSettingInfo GetSettingInfo(string path)
        {
            foreach (OverhaulSettingInfo info in Settings)
            {
                if (info.path == path)
                    return info;
            }
            return null;
        }

        public List<OverhaulSettingInfo> GetSettingInfos(string category)
        {
            List<OverhaulSettingInfo> result = new List<OverhaulSettingInfo>();
            foreach (OverhaulSettingInfo info in Settings)
            {
                if (info.category == category)
                    result.Add(info);
            }
            return result;
        }

        public List<OverhaulSettingInfo> GetSettingInfos(string category, string section)
        {
            List<OverhaulSettingInfo> result = new List<OverhaulSettingInfo>();
            foreach (OverhaulSettingInfo info in Settings)
            {
                if (info.category == category && info.section == section)
                    result.Add(info);
            }
            return result;
        }

        public List<string> GetCategories()
        {
            List<string> list = new List<string>();
            foreach (OverhaulSettingInfo info in Settings)
            {
                if (!list.Contains(info.category))
                    list.Add(info.category);
            }
            return list;
        }

        public List<string> GetSections(string category)
        {
            List<string> list = new List<string>();
            foreach (OverhaulSettingInfo info in Settings)
            {
                if (info.category == category && !list.Contains(info.section))
                    list.Add(info.section);
            }
            return list;
        }
    }
}
