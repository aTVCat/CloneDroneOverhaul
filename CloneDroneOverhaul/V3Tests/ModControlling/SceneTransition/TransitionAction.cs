using System;

namespace CloneDroneOverhaul.V3Tests.Base
{
    public struct TransitionAction
    {
        public TranstionType Type;
        public Action Action;
        public string SceneName;
        public bool UseAsyncSceneLoading;

        public bool HideOnComplete;
    }
}