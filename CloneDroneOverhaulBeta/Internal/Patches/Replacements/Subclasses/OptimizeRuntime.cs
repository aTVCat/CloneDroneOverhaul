namespace CDOverhaul.Patches
{
    public class OptimizeRuntime : ReplacementBase
    {
        public override void Replace()
        {
            base.Replace();

            _ = GameUIRoot.Instance.gameObject.AddComponent<GameUIRootBehaviour>();

            SuccessfullyPatched = true;
        }
    }
}
