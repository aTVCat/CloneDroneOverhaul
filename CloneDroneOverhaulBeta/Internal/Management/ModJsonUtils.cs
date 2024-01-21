using Newtonsoft.Json;
using System.IO;

namespace CDOverhaul
{
    internal static class ModJsonUtils
    {
        public static string Serialize(object @object)
        {
            string result;

            result = JsonConvert.SerializeObject(@object, DataRepository.Instance.GetSettings());

            return result;
        }

        public static T Deserialize<T>(string @string)
        {
            T result;

            result = JsonConvert.DeserializeObject<T>(@string, DataRepository.Instance.GetSettings());

            return result;
        }

        public static T DeserializeStream<T>(string path)
        {
            T result;

            using (Stream s = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = JsonSerializer.Create(DataRepository.Instance.GetSettings());
                result = serializer.Deserialize<T>(reader);
            }

            return result;
        }
    }
}
