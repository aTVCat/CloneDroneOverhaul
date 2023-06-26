using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay.Pets
{
    public class PetInstanceBehaviour : OverhaulBehaviour
    {
        public PetItem Item;
        public PetBehaviourSettings BehaviourSettings => Item != null ? Item.BehaviourSettings : null;

        public FirstPersonMover Owner;

        public void Update()
        {

        }

        public static void CreateInstance(PetItem pet, FirstPersonMover firstPersonMover)
        {

        }
    }
}
