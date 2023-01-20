using CloneDroneOverhaul.V3.Utilities;
using UnityEngine;

namespace CloneDroneOverhaul.V3.HUD
{
    public class UIMindTransfersLeftForEdgeCases : V3_ModHUDBase
    {
        private RectTransform Prefab;
        private RectTransform Container;

        private void Start()
        {
            Prefab = MyModdedObject.GetObjectFromList<RectTransform>(0);
            Container = MyModdedObject.GetObjectFromList<RectTransform>(1);
        }

        public void RefreshMindTransfers()
        {
            TransformUtils.DestroyAllChildren(Container);

            Prefab.gameObject.Instantiate(Prefab.gameObject, Container, GameDataManager.Instance.GetNumConsciousnessTransfersLeft(), true);

            Container.localScale = Vector3.zero;
            iTween.ScaleTo(Container.gameObject, Vector3.one, 1f);

            DelegateScheduler.Instance.Schedule(delegate
            {
                iTween.ScaleTo(Container.gameObject, Vector3.zero, 0.5f);
            }, 5f);
        }
    }
}
