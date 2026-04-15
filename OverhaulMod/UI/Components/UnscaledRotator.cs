using UnityEngine;

namespace OverhaulMod.UI
{
    /// <summary>
    /// <see cref="Rotator"/>, but uses <see cref="Time.unscaledDeltaTime"/>
    /// </summary>
    public class UnscaledRotator : MonoBehaviour
    {
        public Vector3 RandomStartRotation;

        public Vector3 RotationSpeed;

        public bool IsRandom;

        private Vector3 _rotationSpeed;

        public void CopySettings(Rotator rotator)
        {
            RandomStartRotation = rotator.RandomStartRotation;
            RotationSpeed = rotator.RotationSpeed;
            IsRandom = rotator.IsRandom;
        }

        public void SetRotationSpeed(Vector3 newSpeed)
        {
            _rotationSpeed = newSpeed;
        }

        public void Awake()
        {
            if (IsRandom)
            {
                _rotationSpeed = new Vector3(Random.Range(-RotationSpeed.x, RotationSpeed.x), Random.Range(-RotationSpeed.y, RotationSpeed.y), Random.Range(-RotationSpeed.z, RotationSpeed.z));
            }
            else
            {
                _rotationSpeed = RotationSpeed;
            }

            if (RandomStartRotation.magnitude > 1f)
            {
                base.transform.localEulerAngles = new Vector3(Random.Range(-RandomStartRotation.x, RandomStartRotation.x), Random.Range(-RandomStartRotation.y, RandomStartRotation.y), Random.Range(-RandomStartRotation.z, RandomStartRotation.z));
            }
        }

        public void Update()
        {
            transform.Rotate(_rotationSpeed * Time.unscaledDeltaTime);
        }
    }
}