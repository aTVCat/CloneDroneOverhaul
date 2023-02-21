using UnityEngine;

namespace CDOverhaul.ArenaRemaster
{
    public class ArenaRemasterArrowBlinker : MonoBehaviour
    {
        private float _timeToUpdate = -1;

        private GameObject Off;
        private GameObject On;

        private ArenaRemasterArrowBlinker _nextArrowBlinker;

        public void Initialize(in ModdedObject moddedObject, in ArenaRemasterArrowBlinker nextArrow)
        {
            _nextArrowBlinker = nextArrow;
            Off = moddedObject.GetObject<Transform>(0).gameObject;
            On = moddedObject.GetObject<Transform>(1).gameObject;
            ChangeState(false);
        }

        public void ChangeState(in bool value)
        {
            if (value)
            {
                _timeToUpdate = Time.time + 0.5f;
            }

            Off.SetActive(!value);
            On.SetActive(value);
        }

        private void Update()
        {
            if (_timeToUpdate != -1 && Time.time >= _timeToUpdate)
            {
                ChangeState(false);
                _nextArrowBlinker.ChangeState(true);
                _timeToUpdate = -1;
            }
        }
    }
}
