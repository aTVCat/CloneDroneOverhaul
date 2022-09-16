using System;
using System.Collections.Generic;

namespace CloneDroneOverhaul.Modules
{
    public class DelegateTimer : ModuleBase
    {
        private List<TimedAction> actions = new List<TimedAction>();

        public override bool ShouldWork()
        {
            return true;
        }
        public override void OnNewFrame()
        {
            if (actions.Count < 1)
            {
                return;
            }

            for (int i = actions.Count - 1; i > 0; i--)
            {
                if (actions[i].HasArgs)
                {
                    if (actions[i].CompleteNextFrame && actions[i].Act != null)
                    {
                        actions[i].Act();
                        actions.RemoveAt(i);
                    }
                }
                else
                {
                    if (actions[i].CompleteNextFrame && actions[i].ActArgs != null)
                    {
                        actions[i].ActArgs(actions[i].Args);
                        actions.RemoveAt(i);
                    }
                }
            }
        }

        public void AddNoArgActionToCompleteNextFrame(Action action)
        {
            actions.Add(new TimedAction
            {
                Act = action,
                CompleteNextFrame = true
            });
        }

        public void AddActionToCompleteNextFrame(Action<object[]> action, object[] argument)
        {
            actions.Add(new TimedAction
            {
                ActArgs = action,
                CompleteNextFrame = true,
                Args = argument
            });
        }

        private class TimedAction
        {
            public Action Act;
            public bool CompleteNextFrame;

            public Action<object[]> ActArgs;
            public object[] Args;

            public bool HasArgs
            {
                get
                {
                    return Args != null;
                }
            }
        }
    }
}
