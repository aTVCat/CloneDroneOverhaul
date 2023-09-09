using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.HUD;

namespace CDOverhaul.LevelEditor
{
    public class OverhaulLevelEditorManager : OverhaulManager<OverhaulLevelEditorManager>
    {
        public MoveByCoordsTool moveByCoordsTool
        {
            get;
            private set;
        }

        public LevelEditorFixes fixes
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void OnSceneReloaded()
        {
            base.OnSceneReloaded();
            RemoveComponents();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            RemoveComponents();
        }

        public override void AddListeners()
        {
            base.AddListeners();
            OverhaulEvents.AddEventListener(GlobalEvents.LevelEditorStarted, AddComponents, true);
        }

        public override void RemoveListeners()
        {
            base.RemoveListeners();
            OverhaulEvents.RemoveEventListener(GlobalEvents.LevelEditorStarted, AddComponents, true);
        }

        public void AddComponents()
        {
            if (!moveByCoordsTool)
            {
                moveByCoordsTool = base.gameObject.AddComponent<MoveByCoordsTool>();
            }

            if (!fixes)
            {
                fixes = base.gameObject.AddComponent<LevelEditorFixes>();
                fixes.Init();
            }

            OverhaulUIManager.SetCanvasPixelPerfect(false);
        }

        public void RemoveComponents()
        {
            if (moveByCoordsTool)
                moveByCoordsTool.Dispose(true);

            if (fixes)
                fixes.Dispose(true);
        }
    }
}
