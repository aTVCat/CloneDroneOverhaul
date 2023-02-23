using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Patches
{
    /// <summary>
    /// This doesn't really work actually...
    /// </summary>
    public class StoryChapter3AllyFix : MonoBehaviour
    {
        private FirstPersonMover _ally;
        private bool _isFixingAlly;

        private Vector3 _ogPos;

        public void StartForcingAllyToStayAtPositionForever()
        {
            DelegateScheduler.Instance.Schedule(delegate
            {
                _ally = PlayerAllyManager.Instance.GetPrivateField<FirstPersonMover>("_secondHuman");
                _isFixingAlly = true;
                if (_ally != null)
                {
                    _ogPos = _ally.transform.position;
                }
            }, 2.5f);
        }

        public void StopFixingAlly()
        {
            _ally = null;
            _isFixingAlly = false;
        }

        private void FixedUpdate()
        {
            if (_isFixingAlly)
            {
                if (_ally == null)
                {
                    _isFixingAlly = false;
                    return;
                }
                _ally.transform.position = _ogPos;
                _ally.SetVelocity(Vector3.zero);
            }
        }
    }
}