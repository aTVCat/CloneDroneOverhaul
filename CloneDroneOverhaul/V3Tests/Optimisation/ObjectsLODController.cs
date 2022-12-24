using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Optimisation
{
    public class ObjectsLODController : V3Tests.Base.V3_ModControllerBase
    {
        public static bool LoDEnabled;
        public static bool HighLoDThreshold;

        public static void AddLODGroup(in GameObject gObject, in GameObject[] renderers, in float size)
        {
            if(!LoDEnabled)
            {
                return;
            }

            LODGroup group = gObject.AddComponent<LODGroup>();
            gObject.GetComponent<Renderer>().enabled = false;

            LOD[] lods = new LOD[renderers.Length];
            for(int i = 0; i < renderers.Length; i++)
            {
                Renderer r = renderers[i].GetComponent<Renderer>();
                Renderer[] rens = new Renderer[] { r };

                r.transform.SetParent(gObject.transform);
                r.transform.localPosition = Vector3.zero;
                r.transform.localEulerAngles = Vector3.zero;
                r.transform.localScale = Vector3.one;

                lods[i] = new LOD(1.0F / (i + 1), rens);
            }

            group.SetLODs(lods);
            group.RecalculateBounds();
            group.size = size * (HighLoDThreshold ? 1.25f : 1f);
        }

        public override void OnSettingRefreshed(in string settingName, in object value)
        {
            if(settingName == "Graphics.Optimisation.Level of detail [LoD]")
            {
                LoDEnabled = (bool)value;
            }
            if(settingName == "Graphics.Optimisation.High LoD threshold")
            {
                HighLoDThreshold = (bool)value;
            }
        }
    }
}
