namespace CDOverhaul.Gameplay.Overmodes
{
    public class OvermodesController : OverhaulGameplayController
    {
        public static OvermodesController Instance;

        public OvermodeBase CurrentOvermode
        {
            get;
            set;
        }

        public override void Initialize()
        {
            base.Initialize();
            Instance = this;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            Instance = null;
        }

        private void Update()
        {
            if (!IsOvermode())
                return;

            if (CurrentOvermode.GameModeData != null && CurrentOvermode.GameModeData.IsDirty())
            {
                DataRepository.Instance.Save(CurrentOvermode.GameModeData, CurrentOvermode.GetGameModeName() + "_Data", false, true);
                CurrentOvermode.GameModeData.SetDirty(false);
            }
        }

        public void StartTestMode()
        {
            CurrentOvermode = new TestOvermode();
            CurrentOvermode.Start();
        }

        public static bool IsOvermode() => Instance && Instance.CurrentOvermode != null;
        public static OvermodeBase GetOvermode() => IsOvermode() ? Instance.CurrentOvermode : null;
    }
}
