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
                    s_Reference = Get<T>();

                return s_Reference;
            }
        }

        public bool HasAddedEventListeners
        {
            get;
            protected set;
        }

        public override void Initialize()
        {
            OverhaulCore.OnAssetsLoadDone += OnAssetsLoaded;
            AddListeners();
        }

        public override void OnSceneReloaded()
        {
            AddListeners();
        }

        protected virtual void OnAssetsLoaded()
        {

        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            RemoveListeners();
            s_Reference = null;
            OverhaulCore.OnAssetsLoadDone -= OnAssetsLoaded;
        }

        protected virtual void AddListeners()
        {
            HasAddedEventListeners = true;
        }

        protected virtual void RemoveListeners()
        {
            HasAddedEventListeners = false;
        }
    }
}
