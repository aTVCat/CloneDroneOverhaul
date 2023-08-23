using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected override void AddListeners()
        {
            base.AddListeners();
            OverhaulEventsController.AddEventListener(GlobalEvents.LevelEditorStarted, AddComponents, true);
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            OverhaulEventsController.RemoveEventListener(GlobalEvents.LevelEditorStarted, AddComponents, true);
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

            OverhaulCanvasController.SetCanvasPixelPerfect(false);
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
