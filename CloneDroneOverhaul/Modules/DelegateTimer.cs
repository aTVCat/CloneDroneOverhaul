using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            for(int i = actions.Count - 1; i > 0; i--)
            {
                if (actions[i].CompleteNextFrame && actions[i].Act != null)
                {
                    actions[i].Act();
                    actions.RemoveAt(i);
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

        private class TimedAction
        {
            public Action Act;
            public bool CompleteNextFrame;
        }
    }
}
