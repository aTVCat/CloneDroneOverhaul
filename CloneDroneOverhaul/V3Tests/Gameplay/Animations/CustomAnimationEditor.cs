using System;

namespace CloneDroneOverhaul.V3Tests.Gameplay.Animations
{
    public class CustomAnimationEditor : Interfaces.IEditor
    {
        public void EnterEditor()
        {
            GameFlowManager.Instance.SetGameMode(GetGamemode());
        }

        public GameMode GetGamemode()
        {
            return (GameMode)29299;
        }
    }
}
}
