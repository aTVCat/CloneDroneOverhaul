using ModLibrary;
using OverhaulAPI;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class RobotAccessoriesWearer : FirstPersonMoverExtention
    {
        public bool OwnerHasModel => base.Owner.GetCharacterModel() != null;

        private readonly List<GameObject> _accessories = new List<GameObject>();

        public bool AccessoriesAreVisible = true;

        protected override void Initialize(FirstPersonMover owner)
        {
            if (!OwnerHasModel)
            {
                OverhaulDebugController.Print("No character model!", Color.red);
                return;
            }
            OverhaulEventManager.AddEventListener<Character>("CharacterKilled", onKill, true);
        }

        public void RegisterAccessory(in GameObject gameObject)
        {
            if (_accessories.Contains(gameObject)) return;
            _accessories.Add(gameObject);
            SetUpAccessory(gameObject);
            gameObject.SetActive(AccessoriesAreVisible);
        }

        public void UnregisterAccessory(in GameObject gameObject, in bool destroy = false)
        {
            if (!_accessories.Contains(gameObject)) return;
            _accessories.Remove(gameObject);
            if (!destroy) return;
            Destroy(gameObject);
        }

        public void UnregisterAllAccessories(in bool destroy = false)
        {
            if (_accessories.Count == 0) return;
            for (int i = _accessories.Count - 1; i > -1; i--)
            {
                if (_accessories[i] != null)
                {
                    UnregisterAccessory(_accessories[i], destroy);
                }
            }
        }

        public void DetachAccessory(in GameObject gameObject)
        {
            if (gameObject == null) return;
            SetUpAccessory(gameObject);
            gameObject.transform.SetParent(LevelSpecificWorldRoot.Instance.transform, true);
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            gameObject.GetComponent<GarbageTarget>().MarkReadyToCollect();
        }

        public void DetachAllAccessories()
        {
            SetAllAccessoriesVisibility(true);
            if (_accessories.Count == 0) return;
            for (int i = _accessories.Count - 1; i > -1; i--)
            {
                if (_accessories[i] != null)
                {
                    DetachAccessory(_accessories[i]);
                    UnregisterAccessory(_accessories[i]);
                }
            }
        }

        public void SetAllAccessoriesVisibility(in bool value)
        {
            if (_accessories.Count == 0) return;
            for (int i = _accessories.Count - 1; i > -1; i--)
            {
                if (_accessories[i] != null)
                {
                    _accessories[i].SetActive(value);
                }
            }
        }

        public void SwitchAllAccessoriesVisibility()
        {
            if (Owner.IsAlive())
            {
                AccessoriesAreVisible = !AccessoriesAreVisible;
                SetAllAccessoriesVisibility(AccessoriesAreVisible);
            }
        }

        public void SetUpAccessory(in GameObject gameObject)
        {
            RobotAccessoryBehaviour b = gameObject.GetComponent<RobotAccessoryBehaviour>();
            if (b == null)
            {
                b = gameObject.AddComponent<RobotAccessoryBehaviour>();
            }
            b.Owner = Owner;
            b.TargetTransform = SerializeTransform.SerializeTheTransform(gameObject.transform);

            gameObject.layer = Layers.BodyPart;
            if (gameObject.GetComponent<BoxCollider>() == null) gameObject.AddComponent<BoxCollider>();
            if (gameObject.GetComponent<Rigidbody>() == null)
            {
                Rigidbody r = gameObject.AddComponent<Rigidbody>();
                r.drag = -1;
                r.isKinematic = true;
            }
            if (gameObject.GetComponent<GarbageTarget>() == null)
            {
                GarbageTarget t = gameObject.AddComponent<GarbageTarget>();
                t.State = GarbageState.NotGarbage;
            }
        }

        private void onKill(Character c)
        {
            if (c.GetInstanceID() == Owner.GetInstanceID())
            {
                if (!OwnerHasModel) return;
                DetachAllAccessories();
            }
        }

        private void LateUpdate()
        {
            if (Owner != null && Owner.IsAlive() && Owner.IsMainPlayer())
            {
                if (Input.GetKeyDown(KeyCode.T)) SwitchAllAccessoriesVisibility();
            }
            if (Time.frameCount % 2 == 0)
            {
                foreach (GameObject g in _accessories)
                {
                    SerializeTransform.ApplyOnTransform(g.GetComponent<RobotAccessoryBehaviour>().TargetTransform, g.transform);
                }
            }
        }
    }
}