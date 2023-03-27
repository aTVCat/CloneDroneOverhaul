using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public interface IPlayerAccessoryModel
    {
        void SetModel(GameObject model);
        GameObject GetModel();
    }
}
