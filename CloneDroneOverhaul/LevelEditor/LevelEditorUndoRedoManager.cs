namespace CloneDroneOverhaul.LevelEditor
{
    public class LevelEditorUndoRedoManager : Modules.ModuleBase
    {
        public static LevelEditorUndoRedoManager Instance;

        public override void Start()
        {
            Instance = this;
        }
    }
}
