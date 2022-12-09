using CloneDroneOverhaul.Modules;
using VoxReader.Interfaces;

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

    internal class RawSkinFile : IVoxFile
    {

        public int VersionNumber { get; }

        public IModel[] Models { get; }

        public IPalette Palette { get; }

        public IChunk[] Chunks { get; }

        internal RawSkinFile(int versionNumber, IModel[] models, IPalette palette, IChunk[] chunks)
        {
            this.VersionNumber = versionNumber;
            this.Models = models;
            this.Palette = palette;
            this.Chunks = chunks;
        }
    }
}
