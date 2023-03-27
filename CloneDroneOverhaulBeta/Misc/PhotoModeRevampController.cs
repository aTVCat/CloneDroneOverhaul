using CDOverhaul.HUD;

namespace CDOverhaul.Misc
{
    public class PhotoModeRevampController : OverhaulController
    {
        private static PhotoManager m_Manager;

        public override void Initialize()
        {
            m_Manager = PhotoManager.Instance;

            OverhaulCanvasController c = GetController<OverhaulCanvasController>();
            if(c != null)
            {
                _ = c.AddHUD<OverhaulPhotoModeControls>(c.HUDModdedObject.GetObject<ModdedObject>(1));
            }
        }

        protected override void OnDisposed()
        {
        }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}