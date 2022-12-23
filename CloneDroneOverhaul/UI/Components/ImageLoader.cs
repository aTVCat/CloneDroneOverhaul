using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI.Components
{
    public class ImageLoader : MonoBehaviour
    {
        private ModdedObject MyModdedObject;
        public Image ImageComponent;
        private bool _isDestroyed;

        public void LoadImage(string url)
        {
            if (_isDestroyed)
            {
                return;
            }

            MyModdedObject = base.GetComponent<ModdedObject>();
            MyModdedObject.GetObjectFromList<Transform>(1).gameObject.SetActive(true);

            ImageComponent = MyModdedObject.GetObjectFromList<Image>(0);

            Sprite loadBG = OverhaulCacheManager.GetCached<Sprite>("placeholderLoadSprite");
            ImageComponent.sprite = loadBG;

            Utilities.Coroutines.LoadWorkshopImage(url, delegate (Sprite sp)
            {
                if (_isDestroyed)
                {
                    return;
                }

                SetImage(sp);
            });
        }

        public void SetImage(Sprite sprite)
        {
            if (ImageComponent == null)
            {
                MyModdedObject = base.GetComponent<ModdedObject>();
                ImageComponent = MyModdedObject.GetObjectFromList<Image>(0);
            }

            MyModdedObject.GetObjectFromList<Transform>(1).gameObject.SetActive(false);

            if (sprite == null)
            {
                ImageComponent.sprite = OverhaulCacheManager.GetCached<Sprite>("placeholderLoadSprite");
                return;
            }

            ImageComponent.sprite = sprite;
        }

        private void OnDestroy()
        {
            _isDestroyed = true;
        }
    }
}