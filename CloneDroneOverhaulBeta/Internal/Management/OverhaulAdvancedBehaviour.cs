using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    public class OverhaulAdvancedBehaviour : OverhaulBehaviour
    {
        public bool HasAddedEventListeners
        {
            get;
            protected set;
        }

        public virtual void AddListeners()
        {
            HasAddedEventListeners = true;
        }

        public virtual void RemoveListeners()
        {
            HasAddedEventListeners = false;
        }
    }
}
