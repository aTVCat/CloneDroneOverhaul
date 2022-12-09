using CloneDroneOverhaul.Modules;

namespace CloneDroneOverhaul.WeaponSkins
{
    public class WeaponSkinManager : ModuleBase
    {
        public WeaponSkinEditor Editor;

        public override void Start()
        {
        }

        public void EnterEditor()
        {
            Editor = new WeaponSkinEditor();
            Editor.Enter();
        }

        public class WeaponSkinEditor
        {
            public const GameMode SKINEDITOR_GAMEMODE = (GameMode)8258;

            public void Enter()
            {
                GameFlowManager.Instance.SetGameMode(SKINEDITOR_GAMEMODE);
                LevelManager.Instance.CleanUpLevelThisFrame();
                ArenaManager.SetArenaVisible(false);
                ArenaManager.SetLogoVisible(false);
            }
        }
    }
}
