using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectInfo
    {
        public float[] PositionArray, EulerAnglesArray, ScaleArray;

        public string Name, Path;

        public List<PersonalizationEditorObjectInfo> Children;

        public Dictionary<string, object> PropertyValues;

        public int UniqueIndex;

        public void InitializeTransformArrays()
        {
            if (PositionArray == null) PositionArray = new float[3];
            if (EulerAnglesArray == null) EulerAnglesArray = new float[3];
            if (ScaleArray == null) ScaleArray = new float[3];
        }

        public void ResetRootTransform()
        {
            if (GetPosition() != Vector3.zero) SetPosition(Vector3.zero);
            if (GetEulerAngles() != Vector3.zero) SetEulerAngles(Vector3.zero);
            if (GetScale() != Vector3.one) SetScale(Vector3.one);
        }

        public void SetPosition(Vector3 vector)
        {
            PositionArray[0] = roundValue(vector.x);
            PositionArray[1] = roundValue(vector.y);
            PositionArray[2] = roundValue(vector.z);
        }

        public Vector3 GetPosition() => new Vector3(PositionArray[0], PositionArray[1], PositionArray[2]);

        public void SetEulerAngles(Vector3 vector)
        {
            EulerAnglesArray[0] = roundValue(vector.x);
            EulerAnglesArray[1] = roundValue(vector.y);
            EulerAnglesArray[2] = roundValue(vector.z);
        }

        public Vector3 GetEulerAngles() => new Vector3(EulerAnglesArray[0], EulerAnglesArray[1], EulerAnglesArray[2]);

        public void SetScale(Vector3 vector)
        {
            ScaleArray[0] = roundValue(vector.x);
            ScaleArray[1] = roundValue(vector.y);
            ScaleArray[2] = roundValue(vector.z);
        }

        public Vector3 GetScale() => new Vector3(ScaleArray[0], ScaleArray[1], ScaleArray[2]);

        private float roundValue(float value)
        {
            return Mathf.Round(value * 1000f) / 1000f;
        }

        public PersonalizationEditorObjectBehaviour Deserialize(Transform parent, PersonalizationControllerInfo personalizationControllerInfo)
        {
            PersonalizationEditorObjectManager personalizationEditorObjectManager = PersonalizationEditorObjectManager.Instance;
            if (personalizationEditorObjectManager.GetObjectInfo(Path) == null)
                return null;

            if (!parent)
            {
                GameObject gameObject = new GameObject(Name);
                parent = gameObject.transform;
            }

            PersonalizationEditorObjectBehaviour behaviour = personalizationEditorObjectManager.PlaceObject(Path, parent, false);
            behaviour.UniqueIndex = UniqueIndex;
            behaviour.ControllerInfo = personalizationControllerInfo;
            behaviour.Name = Name;
            behaviour.PropertyValues = PropertyValues;
            behaviour.SerializedScale = GetScale();
            if (PersonalizationEditorManager.IsInEditorMode()) PersonalizationEditorObjectManager.Instance.AddInstantiatedObject(behaviour);
            Transform transform = behaviour.transform;
            transform.localPosition = GetPosition();
            transform.localEulerAngles = GetEulerAngles();
            transform.localScale = behaviour.SerializedScale;
            if (!Children.IsNullOrEmpty())
            {
                foreach (PersonalizationEditorObjectInfo info in Children)
                    _ = info.Deserialize(behaviour.transform, personalizationControllerInfo);
            }
            return behaviour;
        }
    }
}
