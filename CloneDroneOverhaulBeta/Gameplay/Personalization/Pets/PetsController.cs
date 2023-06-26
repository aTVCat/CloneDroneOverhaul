using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay.Pets
{
    public class PetsController : OverhaulGameplayController
    {
        public static readonly List<PetItem> Pets = new List<PetItem>();

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
