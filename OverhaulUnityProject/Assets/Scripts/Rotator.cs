using System;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 RandomStartRotation;

    public Vector3 RotationSpeed;

    public bool IsRandom;

    private Vector3 _rotationSpeed;

    public void SetRotationSpeed(Vector3 newSpeed)
    {
        this._rotationSpeed = newSpeed;
    }

    public void Awake()
    {
        if (this.IsRandom)
        {
            this._rotationSpeed = new Vector3(UnityEngine.Random.Range(-this.RotationSpeed.x, this.RotationSpeed.x), UnityEngine.Random.Range(-this.RotationSpeed.y, this.RotationSpeed.y), UnityEngine.Random.Range(-this.RotationSpeed.z, this.RotationSpeed.z));
        }
        else
        {
            this._rotationSpeed = this.RotationSpeed;
        }
        if (this.RandomStartRotation.magnitude > 1f)
        {
            base.transform.localEulerAngles = new Vector3(UnityEngine.Random.Range(-this.RandomStartRotation.x, this.RandomStartRotation.x), UnityEngine.Random.Range(-this.RandomStartRotation.y, this.RandomStartRotation.y), UnityEngine.Random.Range(-this.RandomStartRotation.z, this.RandomStartRotation.z));
        }
    }

    public void Update()
    {
        base.transform.Rotate(this._rotationSpeed * Time.deltaTime);
    }
}
