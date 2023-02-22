using System.Collections.Generic;

namespace CDOverhaul.Gameplay
{
    public class RobotAccessorySaveData : ModDataContainerBase
    {
        public List<string> Accessories;

        public override void RepairMissingFields()
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

        protected override void OnPrepareToSave()
        {
            FileName = "PlayerAccessories.json";
        }
    }
}
