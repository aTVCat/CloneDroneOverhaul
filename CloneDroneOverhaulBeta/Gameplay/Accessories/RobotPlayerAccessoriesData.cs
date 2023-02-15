using System.Collections.Generic;

namespace CDOverhaul.Gameplay
{
    public class RobotPlayerAccessoriesData : ModDataContainerBase
    {
        public List<string> Accessories;

        public override void RepairMissingFields()
        {
            if (Accessories == null)
            {
                Accessories = new List<string>();
            }
        }

        protected override void OnPrepareToSave()
        {
            FileName = "PlayerAccessories.json";
        }

        public void SaveAccessories()
        {
            SaveData<RobotPlayerAccessoriesData>();
        }
    }
}
