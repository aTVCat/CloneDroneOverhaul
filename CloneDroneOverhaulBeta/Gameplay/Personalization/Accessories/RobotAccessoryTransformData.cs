using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class RobotAccessoryTransformData : ModDataContainerBase
    {
        public SerializeTransform Data;

        public byte CharacterModelIndex;
        public string AccessoryName;

        public string GetFileName() { return CharacterModelIndex + "_" + AccessoryName + ".json"; }

        public override void RepairFields()
        {
            if (Data == null)
            {
                Data = new SerializeTransform
                {
                    LocalScale = Vector3.one
                };
            }
        }

        protected override void OnPreSave()
        {
            FileName = GetFileName();
        }

        public void SaveTransforms()
        {
            SaveData(true, "Accessories");
        }
    }
}
