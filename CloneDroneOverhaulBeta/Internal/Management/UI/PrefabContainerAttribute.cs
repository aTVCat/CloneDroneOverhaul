using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    public class PrefabContainerAttribute : Attribute
    {
        public int PrefabIndex, ContainerIndex;

        public PrefabContainerAttribute(int prefabIndex, int containerIndex)
        {
            PrefabIndex = prefabIndex;
            ContainerIndex = containerIndex;
        }
    }
}
