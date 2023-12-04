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
