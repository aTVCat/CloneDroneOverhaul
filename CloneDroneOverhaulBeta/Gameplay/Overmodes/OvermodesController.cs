using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void test_StartGame()
        {
            CurrentOvermode = new TestOvermode();
            CurrentOvermode.Start();
        }

        public bool IsOvermode() => CurrentOvermode != null;
    }
}
