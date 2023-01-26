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

        private static Dictionary<int, List<FirstPersonMoverExtention>> _spawnedExtentions = new Dictionary<int, List<FirstPersonMoverExtention>>();

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
                eList.Remove(extention);
            }
        }

        internal static void UnregisterFirstPersonMover(FirstPersonMover mover)
        {
            if (_spawnedExtentions.ContainsKey(mover.GetInstanceID()))
            {
                _spawnedExtentions.Remove(mover.GetInstanceID());
            }
        }

        public static T GetExtention<T>(in FirstPersonMover mover) where T : FirstPersonMoverExtention
        {
            if (_spawnedExtentions.TryGetValue(mover.GetInstanceID(), out List<FirstPersonMoverExtention> list))
            {
                foreach (FirstPersonMoverExtention extention in list)
                {
                    if (extention.GetType() == typeof(T))
                    {
                        return (T)extention;
                    }
                }
            }
            return null;
        }

        public static List<FirstPersonMoverExtention> GetExtentions(in FirstPersonMover mover)
        {
            _spawnedExtentions.TryGetValue(mover.GetInstanceID(), out List<FirstPersonMoverExtention> list);
            return list;
        }

        #endregion

        protected bool IsInitialized { get; private set; }
        protected FirstPersonMover Owner { get; private set; }

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

        public virtual void OnAIUpdate(in AIController controller)
        {

        }
    }
}