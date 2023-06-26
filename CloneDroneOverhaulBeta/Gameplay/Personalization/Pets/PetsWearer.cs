using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay.Pets
{
    public class PetsWearer : OverhaulCharacterExpansion
    {
        public override void Start()
        {
            base.Start();

            PetInstanceBehaviour.CreateInstance(PetsController.Pets[1], Owner);
        }
    }
}
