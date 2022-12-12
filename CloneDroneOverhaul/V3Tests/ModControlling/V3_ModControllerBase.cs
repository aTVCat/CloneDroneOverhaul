namespace CloneDroneOverhaul.V3Tests.Base
{
    /// <summary>
    /// The base class for mod controllers. New version of ModuleBase class
    /// </summary>
    public class V3_ModControllerBase : Singleton<V3_ModControllerBase>
    {
        /// <summary>
        /// Get an instance of controller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInstance<T>() where T : V3_ModControllerBase
        {
            return Instance as T;
        }
    }
}