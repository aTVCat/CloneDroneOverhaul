using OverhaulMod.Combat;
using System.Collections.Generic;
using System.Linq;

namespace OverhaulMod.Patches.Behaviours
{
    internal class ChallengeManagerPatchBehaviour : GamePatchBehaviour
    {
        public override void Patch()
        {
            ChallengeManager challengeManager = ChallengeManager.Instance;
            foreach (ChallengeDefinition challenge in challengeManager.Challenges)
            {
                if (challenge.ChallengeID == "HammerChallenge" || challenge.ChallengeID == "SpearOnly" || challenge.ChallengeID == "BowOnlyChallenge")
                {
                    List<UpgradeTypeAndLevel> list = challenge.DisabledUpgrades.ToList();
                    list.Add(new UpgradeTypeAndLevel()
                    {
                        UpgradeType = ModUpgradesManager.SCYTHE_UNLOCK_UPGRADE,
                        Level = 1
                    });
                    list.Add(new UpgradeTypeAndLevel()
                    {
                        UpgradeType = ModUpgradesManager.SCYTHE_FIRE_UPGRADE,
                        Level = 1
                    });
                    challenge.DisabledUpgrades = list.ToArray();
                }
            }
        }

        public override void UnPatch()
        {
            ChallengeManager challengeManager = ChallengeManager.Instance;
            foreach (ChallengeDefinition challenge in challengeManager.Challenges)
            {
                if (challenge.ChallengeID == "HammerChallenge" || challenge.ChallengeID == "SpearOnly" || challenge.ChallengeID == "BowOnlyChallenge")
                {
                    List<int> indexesToRemove = new List<int>();
                    List<UpgradeTypeAndLevel> list = challenge.DisabledUpgrades.ToList();

                    int index = 0;
                    foreach (UpgradeTypeAndLevel upgrade in list)
                    {
                        if (upgrade.UpgradeType == ModUpgradesManager.SCYTHE_UNLOCK_UPGRADE || upgrade.UpgradeType == ModUpgradesManager.SCYTHE_FIRE_UPGRADE)
                        {
                            indexesToRemove.Add(index);
                        }
                        index++;
                    }

                    indexesToRemove.Reverse();
                    foreach (int i in indexesToRemove)
                        list.RemoveAt(i);

                    challenge.DisabledUpgrades = list.ToArray();
                }
            }
        }
    }
}
