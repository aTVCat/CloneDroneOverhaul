using CDOverhaul.Gameplay.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay
{
    public abstract class PersonalizationItemsController : OverhaulGameplayController
    {
        public override void Initialize()
        {
            base.Initialize();
            AddItems();
        }

        public abstract void AddItems();
    }
}
