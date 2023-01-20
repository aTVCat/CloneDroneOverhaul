using System;
using System.Collections.Generic;

namespace CloneDroneOverhaul.V3.Base
{
    public class DelegateTimer : V3_ModControllerBase
    {
        private List<TimedAction> actions = new List<TimedAction>();

        private void Update()
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

        public void CompleteNextFrame(Action action)
        {
            DelegateScheduler.Instance.Schedule(action, UnityEngine.Time.deltaTime);
        }

        public void CompleteInTime(Action action, float timeToWait, bool useUnscaledTime)
        {
            actions.Add(new TimedAction2
            {
                Act = action,
                UnscaledTime = useUnscaledTime,
                TimeToComplete = useUnscaledTime ? UnityEngine.Time.unscaledTime + timeToWait : UnityEngine.Time.time + timeToWait
            });
        }

        private class TimedAction
        {
            public Action Act;
            public bool CompleteNextFrame;

            public Action<object[]> ActArgs;
            public object[] Args;

            public bool HasArgs => Args != null;
        }

        private class TimedAction2 : TimedAction
        {
            public float TimeToComplete = -1;
            public bool UnscaledTime;
        }
    }

}
