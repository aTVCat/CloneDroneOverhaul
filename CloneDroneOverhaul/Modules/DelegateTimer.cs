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

            List<int> indexes = new List<int>();
            int index = 0;

            foreach (TimedAction action in actions)
            {
                if (action.CompleteNextFrame && action.Act != null)
                {
                    action.Act();
                    indexes.Add(index);
                }
                index++;
            }

            foreach (int i in indexes)
            {
                actions.RemoveAt(i);
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
