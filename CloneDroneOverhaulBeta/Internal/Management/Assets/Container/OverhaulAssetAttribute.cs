using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    public class OverhaulAssetAttribute : FieldReferencingAttribute
    {
        public string AssetBundle, AssetName;

        public bool AsyncLoad, FixMaterials;

        public OverhaulAssetAttribute(string assetBundle, string assetName, bool async, bool fixMaterials)
        {
            AssetBundle = assetBundle;
            AssetName = assetName;
            AsyncLoad = async;
            FixMaterials = fixMaterials;
        }
    }
}
