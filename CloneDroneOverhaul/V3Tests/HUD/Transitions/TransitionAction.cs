using System;

namespace CloneDroneOverhaul.V3Tests.HUD
{
    public struct TransitionAction
    {
        public ETranstionType Type;
        public Action Action;
        public string SceneName;
        public bool UseAsyncSceneLoading;

        public bool HideOnComplete;
    }
}