using UnityEngine;

namespace OverhaulMod.Engine
{
    public class ArenaAudienceLinePoint : MonoBehaviour
    {
        [IncludeInLevelEditor]
        public int ID;

        [IncludeInLevelEditor]
        public bool IsEnd;

        [IncludeInLevelEditor]
        public bool IsLower;

        [IncludeInLevelEditor]
        public bool IsMiddle;

        [IncludeInLevelEditor]
        public bool IsTop;

        [IncludeInLevelEditor]
        public bool IsVIP;

        private void Start()
        {
            ArenaAudienceManager.Instance.AddLinePoint(this);
        }

        private void OnDestroy()
        {
            ArenaAudienceManager.Instance.RemoveLinePoint(this);
        }
    }
}
