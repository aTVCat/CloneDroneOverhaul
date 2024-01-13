using OverhaulMod.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class DifficultyTierManager : Singleton<DifficultyTierManager>, IModLoadListener
    {
        public void OnModLoaded()
        {
            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.NewDifficultyTiers))
                return;

            AddDifficultyTier(new EndlessTierDescription()
            {
                Tier = (DifficultyTier)9,
                TierDescription = "Nightmarium",
                TextColor = new Color(0.4f, 0.2f, 0.5f, 1f),
            }, 60000, 15);
        }

        public void AddDifficultyTier(EndlessTierDescription endlessTierDescription, int points, int levelsBefore)
        {
            endlessTierDescription.NumRounds = -1;

            EndlessModeManager endlessModeManager = EndlessModeManager.Instance;
            if (endlessModeManager && !endlessModeManager.TierDescriptions.IsNullOrEmpty())
            {
                List<EndlessTierDescription> list = endlessModeManager.TierDescriptions.ToList();
                list[list.Count - 1].NumRounds = levelsBefore;
                list.Add(endlessTierDescription);
                endlessModeManager.TierDescriptions = list.ToArray();

                LevelEditorDifficultyManager levelEditorDifficultyManager = LevelEditorDifficultyManager.Instance;
                if (levelEditorDifficultyManager)
                {
                    DifficultyPointLimit difficultyPointLimit = new DifficultyPointLimit()
                    {
                        DifficultyTier = endlessTierDescription.Tier,
                        MaxPoints = points
                    };
                    List<DifficultyPointLimit> pointLimitList = levelEditorDifficultyManager.DifficultyPointLimits.ToList();
                    pointLimitList.Add(difficultyPointLimit);
                    levelEditorDifficultyManager.DifficultyPointLimits = pointLimitList.ToArray();
                }
            }
        }
    }
}
