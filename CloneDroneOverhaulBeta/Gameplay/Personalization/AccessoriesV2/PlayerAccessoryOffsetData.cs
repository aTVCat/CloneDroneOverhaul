using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class PlayerAccessoryOffsetData : OverhaulDataBase
    {
        public const string FolderName = "AccessoriesOffsets";

        public string AccessoryItemName;
        public Dictionary<string, ModelOffset> Offsets;

        public override void RepairFields()
        {
            if (!Offsets.IsNullOrEmpty())
            {
                foreach(ModelOffset off in Offsets.Values)
                {
                    if(off.OffsetLocalScale == Vector3.zero)
                    {
                        off.OffsetLocalScale = Vector3.one;
                    }
                }
            }
        }

        protected override void OnPreSave()
        {
            FileName = AccessoryItemName + "Offsets";
        }
    }
}
