using System;
using System.Collections.Generic;

namespace CloneDroneOverhaul.Modules
{
    public class DelegateTimer : ModuleBase
    {
        private List<TimedAction> actions = new List<TimedAction>();

        public override void OnNewFrame()
        {
            if (actions.Count == 0)
            {
                return;
            }

            for (int i = actions.Count - 1; i > -1; i--)
            {
                if (!(actions[i] is TimedAction2))
                {
                    if (!actions[i].HasArgs)
                    {
                        if (actions[i].CompleteNextFrame && actions[i].Act != null)
                        {
                            actions[i].Act();
                            actions.RemoveAt(i);
                        }
                    }
                    else
                    {
                        if (actions[i].CompleteNextFrame && !actions[i].HasArgs)
                        {
                            actions[i].ActArgs(actions[i].Args);
                            actions.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    TimedAction2 act2 = actions[i] as TimedAction2;
                    if (act2.TimeToComplete != -1 && !act2.HasArgs)
                    {
                        if (act2.UnscaledTime ? UnityEngine.Time.unscaledTime >= act2.TimeToComplete : UnityEngine.Time.time >= act2.TimeToComplete)
                        {
                            act2.Act();
                            actions.RemoveAt(i);
                        }
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

        public void AddNoArgAction(Action action, float timeToWait, bool useUnscaledTime)
        {
            actions.Add(new TimedAction2
            {
                Act = action,
                UnscaledTime = useUnscaledTime,
                TimeToComplete = useUnscaledTime ? UnityEngine.Time.unscaledTime + timeToWait : UnityEngine.Time.time + timeToWait
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

        private class TimedAction2 : TimedAction
        {
            public float TimeToComplete = -1;
            public bool UnscaledTime;
        }
    }
}
