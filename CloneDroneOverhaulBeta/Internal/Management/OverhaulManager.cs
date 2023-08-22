using CDOverhaul.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    public class OverhaulManager<T> : OverhaulController where T : OverhaulController
    {
        private static T s_Reference;
        public static T reference
        {
            get
            {
                if (!s_Reference)
                    s_Reference = GetController<T>();

                return s_Reference;
            }
        }

        public override void Initialize()
        {
            OverhaulCore.OnAssetsLoadDone += OnAssetsLoaded;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            s_Reference = null;
            OverhaulCore.OnAssetsLoadDone -= OnAssetsLoaded;
        }

        protected virtual void OnAssetsLoaded()
        {

        }
    }
}
