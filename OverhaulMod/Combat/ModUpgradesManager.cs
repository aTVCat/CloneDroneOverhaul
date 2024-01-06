using ModLibrary;
using OverhaulMod.Utils;

namespace OverhaulMod.Combat
{
    public class ModUpgradesManager : Singleton<ModUpgradesManager>
    {
        public void AddUpgrade()
        {
            UpgradeManager.Instance.AddUpgrade(null, ModCache.modCoreInstance);
        }
    }
}
