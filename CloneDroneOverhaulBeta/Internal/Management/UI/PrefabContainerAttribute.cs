using System;

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
