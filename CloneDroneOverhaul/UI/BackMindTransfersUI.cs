using UnityEngine.UI;
using UnityEngine;
using CloneDroneOverhaul;

namespace CloneDroneOverhaul.UI
{
    public class BackupMindTransfersUI : ModGUIBase
    {
        RectTransform Prefab;
        RectTransform Container;
        float timeToHide;

        public override void OnInstanceStart()
        {
            base.MyModdedObject = base.GetComponent<ModdedObject>();
            Prefab = MyModdedObject.GetObjectFromList<RectTransform>(0);
            Container = MyModdedObject.GetObjectFromList<RectTransform>(1);
        }

        public void RefreshMindTransfers()
        {
            TransformUtils.DestroyAllChildren(Container);
            for (int i = 0; i < GameDataManager.Instance.GetNumConsciousnessTransfersLeft(); i++)
            {
                Transform trans = Instantiate(Prefab, Container);
                trans.gameObject.SetActive(true);
            }
            Container.localScale = Vector3.zero;
            iTween.ScaleTo(Container.gameObject, Vector3.one, 1.5f);
            timeToHide = Time.unscaledTime + 5f;
        }

        public override void OnNewFrame()
        {           
            if(timeToHide != -1 && Time.unscaledTime >= timeToHide)
            {
                timeToHide = -1;
                iTween.ScaleTo(Container.gameObject, Vector3.zero, 1.5f);
            }
        }
    }
}
