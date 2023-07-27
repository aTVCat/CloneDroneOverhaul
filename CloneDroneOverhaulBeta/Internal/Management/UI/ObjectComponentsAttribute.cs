using System;

namespace CDOverhaul
{
    public class ObjectComponentsAttribute : Attribute
    {
        public Type[] Components;

        public ObjectComponentsAttribute(Type[] components)
        {
            Components = components;
        }
    }
}
