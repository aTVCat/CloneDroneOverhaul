using ModLibrary;
using OverhaulAPI;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    /// <summary>
    /// Extention that controls accessories
    /// </summary>
    public class RobotAccessoriesWearer : FirstPersonMoverExtention
    {
        /// <summary>
        /// Check if robot has own model
        /// </summary>
        public bool OwnerHasModel => base.Owner.GetCharacterModel() != null;

        /// <summary>
        /// Currently wearing accessories
        /// </summary>
        private readonly List<GameObject> _accessories = new List<GameObject>();

        /// <summary>
        /// Are they visible (put accessories here)
        /// </summary>
        public bool AccessoriesAreVisible = true;

        /// <summary>
        /// TBA?
        /// </summary>
        /// <param name="owner"></param>
        protected override void Initialize(FirstPersonMover owner)
        {
            if (!OwnerHasModel)
            {
                OverhaulDebugController.Print("No character model!", Color.red);
                return;
            }
            _ = OverhaulEventManager.AddEventListener<Character>("CharacterKilled", onKill, true);
        }

        /// <summary>
        /// Get accessory in list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RobotAccessoryBehaviour GetEquipedAccessory(int index)
        {
            if (index >= _accessories.Count)
            {
                return null;
            }
            return _accessories[index].GetComponent<RobotAccessoryBehaviour>();
        }

        /// <summary>
        /// Make us own the accessory
        /// </summary>
        /// <param name="def"></param>
        /// <param name="mover"></param>
        public void RegisterAccessory(in RobotAccessoryItemDefinition def, in FirstPersonMover mover)
        {
            GameObject gameObject = RobotAccessoriesController.SpawnAccessory(def, mover);
            if (_accessories.Contains(gameObject))
            {
                return;
            }

            _accessories.Add(gameObject);
            SetUpAccessory(gameObject);
            gameObject.SetActive(AccessoriesAreVisible);
        }

        /// <summary>
        /// Make us NOT own the accessory and throw it into garbage room (no)
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="destroy"></param>
        public void UnregisterAccessory(in GameObject gameObject, in bool destroy = false)
        {
            _ = _accessories.Remove(gameObject);
            if (!destroy)
            {
                return;
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// TBA
        /// </summary>
        /// <param name="destroy"></param>
        public void UnregisterAllAccessories(in bool destroy = false)
        {
            if (_accessories.Count == 0)
            {
                return;
            }

            for (int i = _accessories.Count - 1; i > -1; i--)
            {
                if (_accessories[i] != null)
                {
                    UnregisterAccessory(_accessories[i], destroy);
                }
            }
        }

        /// <summary>
        /// Detach accessory from the robot and enable physics on it
        /// </summary>
        /// <param name="gameObject"></param>
        public void DetachAccessory(in GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            SetUpAccessory(gameObject);
            gameObject.transform.SetParent(LevelSpecificWorldRoot.Instance.transform, true);
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            gameObject.GetComponent<GarbageTarget>().MarkReadyToCollect();
            if (gameObject.GetComponent<Animator>() != null)
            {
                gameObject.GetComponent<Animator>().enabled = false;
            }
        }

        /// <summary>
        /// Detach all
        /// </summary>
        public void DetachAllAccessories()
        {
            SetAllAccessoriesVisibility(true);
            if (_accessories.Count == 0)
            {
                return;
            }

            for (int i = _accessories.Count - 1; i > -1; i--)
            {
                if (_accessories[i] != null)
                {
                    DetachAccessory(_accessories[i]);
                    UnregisterAccessory(_accessories[i]);
                }
            }
        }

        /// <summary>
        /// Make all accessories visible or not
        /// </summary>
        /// <param name="value"></param>
        public void SetAllAccessoriesVisibility(in bool value)
        {
            if (_accessories.Count == 0)
            {
                return;
            }

            for (int i = _accessories.Count - 1; i > -1; i--)
            {
                if (_accessories[i] != null)
                {
                    _accessories[i].SetActive(value);
                }
            }
        }

        /// <summary>
        /// TBA
        /// </summary>
        public void SwitchAllAccessoriesVisibility()
        {
            if (Owner.IsAlive())
            {
                AccessoriesAreVisible = !AccessoriesAreVisible;
                SetAllAccessoriesVisibility(AccessoriesAreVisible);
            }
        }

        /// <summary>
        /// Prepare accessory
        /// </summary>
        /// <param name="gameObject"></param>
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
            if (gameObject.GetComponent<BoxCollider>() == null)
            {
                _ = gameObject.AddComponent<BoxCollider>();
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

        private void onKill(Character c)
        {
            if (c.GetInstanceID() == Owner.GetInstanceID())
            {
                if (!OwnerHasModel)
                {
                    return;
                }

                DetachAllAccessories();
            }
        }

        private void LateUpdate()
        {
            if (Owner != null && Owner.IsAlive() && Owner.IsMainPlayer())
            {
                if (!Cursor.visible && Input.GetKeyDown(KeyCode.T))
                {
                    SwitchAllAccessoriesVisibility();
                }
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