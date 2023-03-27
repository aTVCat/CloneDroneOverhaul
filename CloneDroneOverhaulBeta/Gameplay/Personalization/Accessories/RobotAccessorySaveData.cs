using System.Collections.Generic;

namespace CDOverhaul.Gameplay
{
    public class RobotAccessorySaveData : OverhaulDataBase
    {
        public List<string> Accessories;

        public override void RepairFields()
        {
            if (Accessories == null)
            {
                Accessories = new List<string>
                {
                    "Igrok's hat"
                };
            }
            SaveData();
        }

        protected override void OnPreSave()
        {
            FileName = "PlayerAccessories.json";
        }
    }
}
