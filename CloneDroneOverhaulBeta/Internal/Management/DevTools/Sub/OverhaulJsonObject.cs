using CDOverhaul.DevTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul
{
    public class OverhaulJsonObject
    {
        public static string JsonObjectsFolder => OverhaulCore.ModDirectoryStatic + "Assets/JsonObjects/";

        public Dictionary<string, string> Values;

        public OverhaulJsonObject()
        {
            Values = new Dictionary<string, string>();
        }

        public OverhaulJsonObject(OverhaulJsonObjectPreset preset)
        {
            Values = new Dictionary<string, string>();
            foreach(string @string in preset.Values)
            {
                Values.Add(@string, string.Empty);
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static OverhaulJsonObject FromJson(string data)
        {
            return JsonConvert.DeserializeObject<OverhaulJsonObject>(data);
        }

        public void Dispose()
        {
            Values.Clear();
            Values = null;
        }
    }
}
