using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.DevTools
{
    public class OverhaulJsonObjectPreset
    {
        public static string PresetsFolder => OverhaulCore.ModDirectoryStatic + "Assets/JsonObjects/Presets/";

        public List<string> Values;

        public OverhaulJsonObjectPreset()
        {
            Values = new List<string>();
        }

        public OverhaulJsonObjectPreset(OverhaulJsonObject jsonObject)
        {
            Values = new List<string>(jsonObject.Values.Count);
            foreach(string @string in jsonObject.Values.Keys)
            {
                Values.Add(@string);
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static OverhaulJsonObjectPreset FromJson(string data)
        {
            return JsonConvert.DeserializeObject<OverhaulJsonObjectPreset>(data);
        }

        public void Dispose()
        {
            Values.Clear();
            Values = null;
        }
    }
}
