using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class PlayerAccessoryModel : IPlayerAccessoryModel
    {
        /// <summary>
        /// Prefab
        /// </summary>
        private GameObject m_Model;
        void IPlayerAccessoryModel.SetModel(GameObject model) => m_Model = model;
        GameObject IPlayerAccessoryModel.GetModel() => m_Model;

        public static IPlayerAccessoryModel CreateModel(GameObject prefab)
        {
            IPlayerAccessoryModel model = new PlayerAccessoryModel();
            model.SetModel(prefab);
            return model;
        }
    }
}
