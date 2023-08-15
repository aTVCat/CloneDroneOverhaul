using PicaVoxel;
using System.IO;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulVoxAssetInfo
    {
        public string PathUnderModFolder = "none";
        public string VolumeName = "untitled";

        public float VoxelSize = 0.1f;
        public bool CenterPivot;

        public bool IsNone() => PathUnderModFolder == "none";

        public bool TryLoad(in Volume volume)
        {
            if (!volume)
                return false;

            string path = OverhaulMod.Core.ModDirectory + PathUnderModFolder;
            if (!File.Exists(path))
                return false;

            MagicaVoxelImporter.MagicaVoxelImport(path, VolumeName, VoxelSize, CenterPivot, volume.gameObject);
            return true;
        }

        public bool TryLoad(in GameObject gameObject)
        {
            return gameObject && TryLoad(gameObject.GetComponent<Volume>());
        }
    }
}
