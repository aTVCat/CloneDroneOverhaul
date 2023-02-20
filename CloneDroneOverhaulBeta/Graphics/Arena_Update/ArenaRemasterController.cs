using UnityEngine;

namespace CDOverhaul.ArenaRemaster
{
    public class ArenaRemasterController : ModController
    {
        /// <summary>
        /// Set "Arena2019" (Arena interior) gameobject visible
        /// </summary>
        /// <param name="value"></param>
        public static void SetArenaInteriorVisible(in bool value)
        {
            Transform root = WorldRoot.Instance.transform;

            Transform arena2019 = TransformUtils.FindChildRecursive(root, "Arena2019");
            if (arena2019 != null)
            {
                arena2019.gameObject.SetActive(value);
            }

            Transform liftwall1 = TransformUtils.FindChildRecursive(root, "LiftWall (1)");
            if (liftwall1 != null)
            {
                liftwall1.gameObject.SetActive(value);
            }
        }

        public Transform ArenaRemasterTransform;
        public ModdedObject ArenaRemasterModdedObject;

        public override void Initialize()
        {
            AssetController.PreloadAsset<GameObject>("P_AR_Arrow", Enumerators.EModAssetBundlePart.Arena_Update);

            //SetArenaInteriorVisible(false);

            ArenaRemasterTransform = Instantiate(AssetController.GetAsset("P_ArenaRemaster", Enumerators.EModAssetBundlePart.Arena_Update)).transform;
            ArenaRemasterTransform.SetParent(TransformUtils.FindChildRecursive(WorldRoot.Instance.transform, "ArenaFinal"));
            ArenaRemasterModdedObject = ArenaRemasterTransform.GetComponent<ModdedObject>();
            /*
            ArenaRemasterTransform.position = new Vector3(-39, 0, 0);
            ArenaRemasterTransform.eulerAngles = new Vector3(0, 270, 0);
            ArenaRemasterTransform.localScale = Vector3.one * 5;*/
            ArenaRemasterTransform.position = Vector3.zero;
            ArenaRemasterTransform.eulerAngles = Vector3.zero;

            setUpArrows();
        }

        private void setUpArrows()
        {
            ArenaRemasterArrowBlinker b1 = ArenaRemasterModdedObject.GetObject<Transform>(2).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            ArenaRemasterArrowBlinker b2 = ArenaRemasterModdedObject.GetObject<Transform>(1).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            ArenaRemasterArrowBlinker b3 = ArenaRemasterModdedObject.GetObject<Transform>(0).gameObject.AddComponent<ArenaRemasterArrowBlinker>();
            b1.Initialize(b1.GetComponent<ModdedObject>(), b2);
            b1.ChangeState(true);
            b2.Initialize(b2.GetComponent<ModdedObject>(), b3);
            b3.Initialize(b3.GetComponent<ModdedObject>(), b1);
        }
    }
}
