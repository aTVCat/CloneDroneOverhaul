using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class ModelOffset
    {
        public static readonly ModelOffset Default = new ModelOffset(Vector3.zero, Vector3.zero, Vector3.one); 

        public Vector3 OffsetPosition
        {
            get;
            set;
        }

        public Vector3 OffsetEulerAngles
        {
            get;
            set;
        }

        public Vector3 OffsetLocalScale
        {
            get;
            set;
        }

        public ModelOffset()
        {
        }

        public ModelOffset(Vector3 position)
        {
            OffsetPosition = position;
            OffsetEulerAngles = Vector3.zero;
            OffsetLocalScale = Vector3.one;
        }

        public ModelOffset(Vector3 position, Vector3 eulerAngles)
        {
            OffsetPosition = position;
            OffsetEulerAngles = eulerAngles;
            OffsetLocalScale = Vector3.one;
        }

        public ModelOffset(Vector3 position, Vector3 eulerAngles, Vector3 localScale)
        {
            OffsetPosition = position;
            OffsetEulerAngles = eulerAngles;
            OffsetLocalScale = localScale;
        }
    }
}
