namespace CDOverhaul.Gameplay
{
    public abstract class PersonalizationItemsController : OverhaulGameplayController
    {
        public override void Initialize()
        {
            base.Initialize();
            AddItems();
        }

        public abstract void AddItems();
    }
}
