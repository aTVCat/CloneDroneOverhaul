using CDOverhaul.Shared;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    /// <summary>
    /// A class focused on patching <see cref="FirstPersonMover"/>
    /// </summary>
    [RequireComponent(typeof(FirstPersonMover))]
    public class FirstPersonMoverExtention : MonoBehaviour
    {
        #region Static

        private static readonly Dictionary<int, List<FirstPersonMoverExtention>> _spawnedExtentions = new Dictionary<int, List<FirstPersonMoverExtention>>();

        internal static void InitializeStatic()
        {
            _spawnedExtentions.Clear();
        }

        internal static void RegisterExtentionInstance(FirstPersonMover mover, in FirstPersonMoverExtention extention)
        {
            if (_spawnedExtentions.TryGetValue(mover.GetInstanceID(), out List<FirstPersonMoverExtention> eList))
            {
                if (eList.Contains(extention))
                {
                    return;
                }
                eList.Add(extention);
            }
            else
            {
                eList = new List<FirstPersonMoverExtention>
                {
                    extention
                };
                _spawnedExtentions.Add(mover.GetInstanceID(), eList);
                ObjectStateListener s = ObjectStateListener.AddStateListener(mover.gameObject);
                s.AddOnDestroyTrigger(delegate
                {
                    UnregisterFirstPersonMover(mover);
                });
            }
        }

        internal static void UnregisterExtentionInstance(FirstPersonMover mover, in FirstPersonMoverExtention extention)
        {
            if (_spawnedExtentions.TryGetValue(mover.GetInstanceID(), out List<FirstPersonMoverExtention> eList))
            {
                if (!eList.Contains(extention))
                {
                    return;
                }
                _ = eList.Remove(extention);
            }
        }

        internal static void UnregisterFirstPersonMover(FirstPersonMover mover)
        {
            if (_spawnedExtentions.ContainsKey(mover.GetInstanceID()))
            {
                _ = _spawnedExtentions.Remove(mover.GetInstanceID());
            }
        }

        public static T GetExtention<T>(in FirstPersonMover mover) where T : FirstPersonMoverExtention
        {
            if (!_spawnedExtentions.ContainsKey(mover.GetInstanceID()))
            {
                return null;
            }

            foreach (FirstPersonMoverExtention extention in _spawnedExtentions[mover.GetInstanceID()])
            {
                if (extention.GetType() == typeof(T))
                {
                    return (T)extention;
                }
            }
            return null;
        }

        public static List<FirstPersonMoverExtention> GetExtentions(in FirstPersonMover mover)
        {
            if (!_spawnedExtentions.ContainsKey(mover.GetInstanceID()))
            {
                return null;
            }
            return _spawnedExtentions[mover.GetInstanceID()];
        }

        #endregion

        protected bool IsInitialized { get; private set; }
        public FirstPersonMover Owner { get; set; }

        private void Start()
        {
            FirstPersonMover mover = base.GetComponent<FirstPersonMover>();
            Owner = mover;
            RegisterExtentionInstance(mover, this);
            Initialize(mover);

            IsInitialized = true;
        }

        protected virtual void Initialize(FirstPersonMover owner)
        {

        }

        public virtual void OnExecuteCommand(FPMoveCommand command, bool resetState)
        {

        }

        public virtual void OnAIUpdate(in AISwordsmanController controller)
        {

        }

        public virtual void OnUpgradesRefreshed(UpgradeCollection upgrades)
        {

        }
    }
}