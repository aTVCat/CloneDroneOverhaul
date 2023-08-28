namespace CDOverhaul.Gameplay
{
    public class OverhaulGameplaySystem : OverhaulAdvancedBehaviour
    {
        public override void Start()
        {
            AddListeners();
        }

        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
            RemoveListeners();
        }

        public virtual void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool initializedModel)
        {

        }
    }
}
