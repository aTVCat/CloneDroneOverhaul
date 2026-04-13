using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class AccessoryOffset
    {
        public float[] PositionArray, EulerAnglesArray, ScaleArray;

        public void InitializeTransformArrays()
        {
            if (PositionArray == null) PositionArray = new float[3];
            if (EulerAnglesArray == null) EulerAnglesArray = new float[3];
            if (ScaleArray == null) ScaleArray = new float[3];
        }

        public void SetPosition(Vector3 vector)
        {
            PositionArray[0] = vector.x;
            PositionArray[1] = vector.y;
            PositionArray[2] = vector.z;
        }

        public Vector3 GetPosition() => new Vector3(PositionArray[0], PositionArray[1], PositionArray[2]);

        public void SetEulerAngles(Vector3 vector)
        {
            EulerAnglesArray[0] = vector.x;
            EulerAnglesArray[1] = vector.y;
            EulerAnglesArray[2] = vector.z;
        }

        public Vector3 GetEulerAngles() => new Vector3(EulerAnglesArray[0], EulerAnglesArray[1], EulerAnglesArray[2]);

        public void SetScale(Vector3 vector)
        {
            ScaleArray[0] = vector.x;
            ScaleArray[1] = vector.y;
            ScaleArray[2] = vector.z;
        }

        public Vector3 GetScale() => new Vector3(ScaleArray[0], ScaleArray[1], ScaleArray[2]);
    }
}
