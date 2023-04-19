using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class SkyboxOverhaulController : OverhaulController
    {
        private static readonly string[] m_Materials = new string[]
        {
            "CasualDay",
            "Cloudymorning",
            "CoriolisNight4k"
        };

        private bool m_HasEverAddedSkyboxes;
        private int m_InitialSkyboxesCount;

        public override void Initialize()
        {
            RefreshSkyboxes();
        }

        public void RefreshSkyboxes()
        {
            int listLengthBefore = SkyBoxManager.Instance.LevelConfigurableSkyboxes.Length;
            if (!m_HasEverAddedSkyboxes)
            {
                m_HasEverAddedSkyboxes = true;
                m_InitialSkyboxesCount = listLengthBefore;
            }

            if(!Equals(m_InitialSkyboxesCount, listLengthBefore))
            {
                return;
            }

            if (!AssetsController.HasAssetBundle("overhaulassets_skyboxes"))
            {
                List<Material> listDef = SkyBoxManager.Instance.LevelConfigurableSkyboxes.ToList();
                for(int i = 0; i < m_Materials.Length; i++)
                {
                    listDef.Add(new Material(SkyBoxManager.Instance.LevelConfigurableSkyboxes[0]) { name = "MISSING: " + m_Materials[i] });
                }
                SkyBoxManager.Instance.LevelConfigurableSkyboxes = listDef.ToArray();
                return;
            }

            List<Material> list = SkyBoxManager.Instance.LevelConfigurableSkyboxes.ToList();
            for (int i = 0; i < m_Materials.Length; i++)
            {
                if(!AssetsController.TryGetAsset<Material>(m_Materials[i], OverhaulAssetsPart.Skyboxes, out Material skybox))
                {
                    list.Add(new Material(SkyBoxManager.Instance.LevelConfigurableSkyboxes[0]) { name = "MISSING: " + m_Materials[i] });
                    continue;
                }
                list.Add(skybox);
            }
            SkyBoxManager.Instance.LevelConfigurableSkyboxes = list.ToArray();
        }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}