using CDOverhaul.CustomMultiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay.Overmodes
{
    public class TestOvermode : OvermodeBase
    {
        private static readonly List<LevelDescription> s_Levels = new List<LevelDescription>()
        {
            new LevelDescription()
            {
                LevelID = "test-level",
                LevelTags = new List<LevelTags>() { LevelTags.LevelEditor },
                GeneratedUniqueID = Guid.NewGuid().ToString(),
            }
        };

        public override GameMode GetGameMode() => (GameMode)20;
        public override EOverhaulMultiplayerMode GetMultiplayerMode() => EOverhaulMultiplayerMode.SandBox;

        public override List<LevelDescription> GetLevelDescriptions()
        {
            s_Levels[0].PrefabName = OverhaulMod.Core.ModDirectory + "Assets/Overmodes/CustomAnimationsRoom.json";
            return s_Levels;
        }
        public override string GetCurrentLevelID() => "test-level";
    }
}
