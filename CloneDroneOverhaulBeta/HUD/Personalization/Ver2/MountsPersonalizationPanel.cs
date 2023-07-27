using System.Collections;

namespace CDOverhaul.HUD
{
    public class MountsPersonalizationPanel : OverhaulPersonalizationPanel
    {
        protected override void PopulateItems()
        {
            _ = StaticCoroutineRunner.StartStaticCoroutine(PopulateItemsCoroutine());
        }

        protected override IEnumerator PopulateItemsCoroutine()
        {
            IsPopulatingItems = true;
            yield return StaticCoroutineRunner.StartStaticCoroutine(PlayFadeAnimation(false));

            yield return StaticCoroutineRunner.StartStaticCoroutine(PlayFadeAnimation(true));
            IsPopulatingItems = false;
            yield break;
        }
    }
}
