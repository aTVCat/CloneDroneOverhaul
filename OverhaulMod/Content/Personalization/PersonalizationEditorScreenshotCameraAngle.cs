using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorScreenshotCameraAngle
    {
        public string Name;

        public float[] PositionArray, EulerAnglesArray;

        public void SetAngleFromTransform(Transform transform)
        {
            PositionArray = new float[] { transform.localPosition.x, transform.localPosition.y, transform.localPosition.z };
            EulerAnglesArray = new float[] { transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z };
        }

        public void ApplyToTransform(Transform transform)
        {
            transform.localPosition = new Vector3(PositionArray[0], PositionArray[1], PositionArray[2]);
            transform.localEulerAngles = new Vector3(EulerAnglesArray[0], EulerAnglesArray[1], EulerAnglesArray[2]);
        }
    }
}
