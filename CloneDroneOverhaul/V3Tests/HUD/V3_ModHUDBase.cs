using CloneDroneOverhaul.V3Tests.Base;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.HUD
{
    public class V3_ModHUDBase : V3_ModControllerBase
    {
        public ModdedObject MyModdedObject { get; private set; }

        public static T AddHUD<T>(in ModdedObject moddedObject) where T : V3_ModHUDBase
        {
            T result = null;

            result = V3_MainModController.AddManager<T>(null, moddedObject.transform);
            result.MyModdedObject = moddedObject;
            result.gameObject.SetActive(true);

            return result;
        }

        public static T AddHUDPatch<T>(in string name) where T : V3_ModHUDBase
        {
            T result;

            result = V3_MainModController.AddManager<T>(name, GameUIRoot.Instance.transform);

            return result;
        }


        public static void ParentUIToGameUIRoot(in RectTransform transform)
        {
            transform.SetParent(Singleton<GameUIRoot>.Instance.transform);
            transform.SetAsFirstSibling();
            transform.sizeDelta = Vector2.zero;
            transform.localScale = Vector3.one;
            transform.localPosition = Vector2.zero;
        }
    }
}
