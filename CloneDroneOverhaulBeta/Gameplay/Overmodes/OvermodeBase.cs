using CDOverhaul.CustomMultiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay.Overmodes
{
    public abstract class OvermodeBase
    {
        public abstract GameMode GetGameMode();
        public virtual EOverhaulMultiplayerMode GetMultiplayerMode() => EOverhaulMultiplayerMode.None;

        public virtual List<LevelDescription> GetLevelDescriptions() => null;
        public virtual string GetCurrentLevelID() => null;

        public virtual void Start()
        {
            GameFlowManager.Instance._gameMode = GetGameMode();
            GameFlowManager.Instance.startSingplayerGameFromTitleScreen();
        }
    }
}
