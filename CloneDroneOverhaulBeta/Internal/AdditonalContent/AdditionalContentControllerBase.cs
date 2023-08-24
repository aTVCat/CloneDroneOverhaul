namespace CDOverhaul
{
    public class AdditionalContentControllerBase : OverhaulBehaviour
    {
        /// <summary>
        /// Full path to content folder
        /// </summary>
        public string ContentFolderFullPath
        {
            get;
            internal set;
        }

        /// <summary>
        /// Path to content folder, doesn't include Overhaul mod folder path
        /// </summary>
        public string ContentFolderPath
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
