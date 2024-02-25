using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class LevelEditorObjectAdvancedBehaviour : MonoBehaviour
    {
        public LevelEditorLevelObject SerializedObject;

        public void RespawnObject()
        {
            if (SerializedObject == null)
                return;

            ObjectPlacedInLevel objectPlacedInLevel2 = LevelEditorObjectPlacementManager.Instance.PlaceObjectInLevelRoot(SerializedObject.LevelObjectEntry, base.transform.parent);
            if (objectPlacedInLevel2)
            {
                Destroy(base.gameObject);
                objectPlacedInLevel2.gameObject.SetActive(base.gameObject.activeSelf);
                SerializedObject.DeserializeInto(objectPlacedInLevel2);
                objectPlacedInLevel2.SetLoadedFromSaveFile(true);
            }
        }
    }
}
