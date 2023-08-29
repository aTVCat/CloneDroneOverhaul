using CDOverhaul.CustomMultiplayer;
using System.Collections.Generic;

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
            GameModeData = DataRepository.Instance.TryLoad(GetGameModeName() + "_Data", out GameData data, false)
                ? data
                : GameDataManager.Instance.createNewGameData();

            GameFlowManager.Instance._gameMode = GetGameMode();
            GameFlowManager.Instance.startSingplayerGameFromTitleScreen();

            if (GetMultiplayerMode() != EOverhaulMultiplayerMode.None)
            {
                OverhaulMultiplayerManager.Instance.StartMultiplayer(GetMultiplayerMode());
            }
        }
    }
}
