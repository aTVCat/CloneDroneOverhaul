using System.Collections.Generic;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay
{
    public class CustomWeaponSkinsData : OverhaulDataBase
    {
        public List<WeaponSkinsImportedItemDefinition> AllCustomSkins;

        public List<Dropdown.OptionData> GetOptions()
        {
            List<Dropdown.OptionData> result = new List<Dropdown.OptionData>();
            foreach (WeaponSkinsImportedItemDefinition importedItem in AllCustomSkins)
                result.Add(new Dropdown.OptionData(importedItem.Name));

            return result;
        }

        public override void RepairFields()
        {
            if (AllCustomSkins == null)
            {
                AllCustomSkins = new List<WeaponSkinsImportedItemDefinition>();
                SaveSkins();
            }
        }

        public void SaveSkins()
        {
            if (WeaponSkinsEditor.EditorEnabled)
            {
                FileName = "ImportedSkins";
                SaveData(true, "Download/Permanent");
            }
        }
    }
}
