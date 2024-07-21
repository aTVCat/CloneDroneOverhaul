using Newtonsoft.Json;
using System.IO;

namespace OverhaulMod.Utils
{
    internal static class ModJsonUtils
    {
        public static string Serialize(object @object)
        {
            string result;

            result = JsonConvert.SerializeObject(@object, ModCache.jsonSerializerSettings);

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
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                result = JsonSerializer.Create(ModCache.jsonSerializerSettings).Deserialize<T>(reader);
            }

            return result;
        }

        public static void WriteStream(string path, object contents)
        {
            FileMode fileMode = File.Exists(path) ? FileMode.Truncate : FileMode.Create;
            using (Stream s = new FileStream(path, fileMode, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(s))
            using (JsonTextWriter reader = new JsonTextWriter(sw))
            {
                JsonSerializer.Create(ModCache.jsonSerializerSettings).Serialize(reader, contents);
            }
        }
    }
}
