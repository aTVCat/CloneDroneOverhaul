using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    public class OverhaulManager : OverhaulController
    {
        public override void Initialize()
        {
            OverhaulCore.OnAssetsLoadDone += OnAssetsLoaded;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            OverhaulCore.OnAssetsLoadDone -= OnAssetsLoaded;
        }

        protected virtual void OnAssetsLoaded()
        {

        }
    }
}
