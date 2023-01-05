using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Base
{
    /// <summary>
    /// The base class for mod controllers. New version of ModuleBase class
    /// </summary>
    public class V3_ModControllerBase : MonoBehaviour
    {
        public static T GetInstance<T>() where T : V3_ModControllerBase
        {
            return V3_MainModController.GetManager<T>();
        }

        public void OnAdded()
        {
        }

        public virtual void OnEvent(in string eventName, in object[] args)
        {

        }

        public virtual void OnSettingRefreshed(in string settingName, in object value)
        {

        }
    }
}