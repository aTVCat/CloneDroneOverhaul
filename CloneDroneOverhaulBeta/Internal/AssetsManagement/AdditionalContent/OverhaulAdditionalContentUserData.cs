using System.Collections;
using UnityEngine;

namespace CDOverhaul.NetworkAssets.AdditionalContent
{
    public class OverhaulAdditionalContentUserData : OverhaulDataBase
    {
        public string EnabledContent;

        public override void RepairFields()
        {
            if(EnabledContent == null)
            {
                EnabledContent = string.Empty;
            }
        }

        public void EnableContentPack(OverhaulAdditionalContentPackInfo info)
        {
            if (!checkValues(info))
            {
                return;
            }

            if (!EnabledContent.Contains(info.PackID))
            {
                EnabledContent += info.PackID + "@";
                SaveData();
            }
        }

        public void DisableContentPack(OverhaulAdditionalContentPackInfo info)
        {
            if (!checkValues(info))
            {
                return;
            }

            if (EnabledContent.Contains(info.PackID))
            {
                EnabledContent = EnabledContent.Replace(info.PackID + "@", string.Empty);
                SaveData();
            }
        }

        public bool IsContentPackEnabled(OverhaulAdditionalContentPackInfo info)
        {
            if(!checkValues(info))
            {
                return false;
            }

            return EnabledContent.Contains(info.PackID);
        }

        private bool checkValues(OverhaulAdditionalContentPackInfo info)
        {
            return info != null && !string.IsNullOrEmpty(info.PackID);
        }
    }
}