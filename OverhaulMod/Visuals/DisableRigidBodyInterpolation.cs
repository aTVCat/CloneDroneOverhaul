using UnityEngine;

namespace OverhaulMod.Visuals
{
    public class DisableRigidBodyInterpolation : MonoBehaviour
    {
        private float _timer;

        private Rigidbody _rigidBody;

        public void Initialize(Rigidbody rigidbody, float timer)
        {
            _rigidBody = rigidbody;
            _timer = timer;
        }

        private void Update()
        {
            _timer = Mathf.Max(0f, _timer - Time.deltaTime);
            if (_timer == 0f)
            {
                if (_rigidBody) _rigidBody.interpolation = RigidbodyInterpolation.None;
                Destroy(this);
            }
        }
    }
}