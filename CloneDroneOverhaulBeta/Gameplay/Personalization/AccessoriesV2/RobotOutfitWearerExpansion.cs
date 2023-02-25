using ModLibrary;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class RobotOutfitWearerExpansion : FirstPersonMoverExpansionBase
    {
        private PlayerOutfitController m_OutfitController;
        private Dictionary<IPlayerAccessoryItemDefinition, GameObject> m_SpawnedModels = new Dictionary<IPlayerAccessoryItemDefinition, GameObject>();

        public override void Start()
        {
            base.Start();

            m_OutfitController = OverhaulGameplayCoreController.Instance.Outfit;
            ReApplyOutfit();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            m_SpawnedModels = null;
            m_OutfitController = null;
        }

        public void ReApplyOutfit()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            if (!OverhaulGamemodeManager.SupportsOutfits())
            {
                return;
            }

            m_SpawnedModels.Clear();
            IPlayerAccessoryItemDefinition[] items = m_OutfitController.Interface.GetAccessoryItems(FirstPersonMover);
            foreach (IPlayerAccessoryItemDefinition item in items)
            {
                SpawnAccessory(item);
            }
        }

        protected override void OnDeath()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            foreach(GameObject gm in m_SpawnedModels.Values)
            {
                gm.transform.SetParent(LevelSpecificWorldRoot.Instance.transform, true);
                Rigidbody b = gm.GetComponent<Rigidbody>();
                b.isKinematic = false;
            }
            m_SpawnedModels.Clear();
            DestroyBehaviour();
        }

        public void SpawnAccessory(IPlayerAccessoryItemDefinition item)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            if(item == null || !(item is PlayerAccessoryItemDefinition))
            {
                return;
            }
            if (!(item as PlayerAccessoryItemDefinition).IsFirstPersonMoverSupported(FirstPersonMover))
            {
                return;
            }
            MechBodyPart mechBodyPart = FirstPersonMover.GetBodyPart(item.GetBodypartType());
            if(mechBodyPart == null)
            {
                return;
            }
            Transform transformParent = mechBodyPart.transform.parent;
            if (transformParent == null)
            {
                return;
            }

            IPlayerAccessoryModel model = item.GetModel();
            if(model == null)
            {
                OverhaulDebugController.PrintError("No accessory model and offset! " + item.GetItemName());
                return;
            }
            GameObject modelGameObject = model.GetModel();
            if (model == null)
            {
                OverhaulDebugController.PrintError("No accessory model! " + item.GetItemName());
                return;
            }
            ModelOffset offset = (item as PlayerAccessoryItemDefinition).GetFirstPersonMoverOffset(FirstPersonMover);
            if(offset == null)
            {
                return;
            }

            GameObject instantiatedModel = Instantiate(modelGameObject, transformParent);
            instantiatedModel.transform.localPosition = offset.OffsetPosition;
            instantiatedModel.transform.localEulerAngles = offset.OffsetEulerAngles;
            instantiatedModel.transform.localScale = offset.OffsetLocalScale;
            instantiatedModel.AddComponent<RobotAccessoryBehaviour>().Initialize(FirstPersonMover, item);
            SetUpAccessory(instantiatedModel);
            m_SpawnedModels.Add(item, instantiatedModel);
        }

        public void SetUpAccessory(GameObject gameObject)
        {
            gameObject.layer = Layers.BodyPart;
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if(renderer == null)
            {
                renderer = gameObject.GetComponentInChildren<Renderer>();
            }
            if(renderer != null)
            {
                if (renderer.GetComponent<BoxCollider>() == null)
                {
                    _ = renderer.gameObject.AddComponent<BoxCollider>();
                }
            }

            if (gameObject.GetComponent<Rigidbody>() == null)
            {
                Rigidbody r = gameObject.AddComponent<Rigidbody>();
                r.isKinematic = true;
            }
            if (gameObject.GetComponent<GarbageTarget>() == null)
            {
                GarbageTarget t = gameObject.AddComponent<GarbageTarget>();
                t.State = GarbageState.NotGarbage;
            }
        }

        public void RemoveAccessory(IPlayerAccessoryItemDefinition item)
        {
            _ = m_SpawnedModels.Remove(item);
        }
    }
}