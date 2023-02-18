using Newtonsoft.Json;

namespace OverhaulAPI
{
    public class FastSerialization
    {
        public static string SerializeObject(object @object)
        {
            return JsonConvert.SerializeObject(@object);
        }

        public static T DeserializeObject<T>(string @object)
        {
            return JsonConvert.DeserializeObject<T>(@object);
        }
    }
}
