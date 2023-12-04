namespace CDOverhaul
{
    public class AddonBase : OverhaulBehaviour
    {
        /// <summary>
        /// Addon folder
        /// </summary>
        public string folder
        {
            get;
            internal set;
        }

        public override void Start()
        {
            AddListeners();
        }

        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
            RemoveListeners();
        }

        public virtual void AddListeners()
        {

        }

        public virtual void RemoveListeners()
        {

        }
    }
}
