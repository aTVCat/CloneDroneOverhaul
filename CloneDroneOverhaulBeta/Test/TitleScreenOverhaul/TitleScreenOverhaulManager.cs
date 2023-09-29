using CDOverhaul.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Patches
{
    public class TitleScreenOverhaulManager : OverhaulManager<TitleScreenOverhaulManager>
    {
        public TitleScreenCustomizationSystem customizationSystem
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            base.Initialize();
            if (!customizationSystem)
                customizationSystem = base.gameObject.AddComponent<TitleScreenCustomizationSystem>();
        }

        protected override void OnAssetsLoaded()
        {
            base.OnAssetsLoaded();
        }

        public override void OnSceneReloaded()
        {
            base.OnSceneReloaded();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            if (customizationSystem)
                customizationSystem.Dispose(true);
        }

        public void DoTitleScreenOverhaul()
        {
            UIConstants.ShowNewTitleScreen();
            if (customizationSystem)
                customizationSystem.SpawnLevel(out _);
        }

        private void LateUpdate()
        {
            /*
            if (!GameModeManager.IsOnTitleScreen())
                return;

            Camera camera = titleScreenLogoCamera;
            if (camera)
            {
                camera.pixelRect = m_TargetLogoCameraRect;
            }*/
        }
    }
}
