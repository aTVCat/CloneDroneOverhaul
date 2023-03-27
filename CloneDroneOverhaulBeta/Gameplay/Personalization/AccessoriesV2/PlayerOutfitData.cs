namespace CDOverhaul.Gameplay
{
    public class PlayerOutfitData : OverhaulDataBase
    {
        public const char Separator = '@';
        public const string Filename = "PlayerOutfit";

        public string EquipedAccessories;

        public override void RepairFields()
        {
            if(EquipedAccessories == null)
            {
                EquipedAccessories = string.Empty;
            }
        }

        protected override void OnPreSave()
        {
            FileName = Filename;
        }

        public void EquipAccessory(string accsName)
        {
            if (EquipedAccessories.Contains(accsName))
            {
                return;
            }
            EquipedAccessories += accsName + Separator;
            SaveData();
        }

        public void UnequipAccessory(string accsName)
        {
            if (!EquipedAccessories.Contains(accsName))
            {
                return;
            }
            EquipedAccessories = EquipedAccessories.Replace(accsName + Separator, string.Empty);
            SaveData();
        }
    }
}
