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
        public GameData GameModeData
        {
            get;
            set;
        }

        public abstract GameMode GetGameMode();
        public abstract string GetGameModeName();
        public virtual EOverhaulMultiplayerMode GetMultiplayerMode() => EOverhaulMultiplayerMode.None;

        public virtual List<LevelDescription> GetLevelDescriptions() => null;
        public virtual string GetCurrentLevelID() => null;

        public virtual bool AllowUpgradeBots() => false;

        public virtual void Start()
        {
            if(DataRepository.Instance.TryLoad(GetGameModeName() + "_Data", out GameData data, false))
                GameModeData = data;
            else
                GameModeData = GameDataManager.Instance.createNewGameData();

            GameFlowManager.Instance._gameMode = GetGameMode();
            GameFlowManager.Instance.startSingplayerGameFromTitleScreen();

            if(GetMultiplayerMode() != EOverhaulMultiplayerMode.None)
            {
                OverhaulMultiplayerController.Instance.StartMultiplayer(GetMultiplayerMode());
            }
        }
    }
}
