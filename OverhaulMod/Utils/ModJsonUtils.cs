using Newtonsoft.Json;
using System.IO;

namespace OverhaulMod.Utils
{
    internal static class ModJsonUtils
    {
        public static string Serialize(object @object)
        {
            string result;

            using (StringWriter sr = new StringWriter())
            {
                JsonSerializer.Create(ModCache.jsonSerializerSettings).Serialize(sr, @object);
                result = sr.ToString();
            }

            return result;
        }

        public static T Deserialize<T>(string @string)
        {
            T result;

            using (StringReader sr = new StringReader(@string))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                result = JsonSerializer.Create(ModCache.jsonSerializerSettings).Deserialize<T>(reader);
            }

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
