using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class RobotAccessoryTransformData : ModDataContainerBase
    {
        public SerializeTransform Data;

        public int CharacterModelIndex;
        public string AccessoryName;

        public string GetFileName => CharacterModelIndex + "_" + AccessoryName + ".json";

        public override void RepairMissingFields()
        {
            if (Data == null)
            {
                Data = new SerializeTransform
                {
                    LocalScale = Vector3.one
                };
            }
        }

        protected override void OnPrepareToSave()
        {
            FileName = GetFileName;
        }

        public void SaveTransforms()
        {
            SaveData<RobotAccessoryTransformData>(true, "Accessories/");
        }
    }
}
