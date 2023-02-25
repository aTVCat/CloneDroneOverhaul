using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public interface IPlayerAccessoryModel
    {
        void SetModel(GameObject model);
        GameObject GetModel();
    }
}
