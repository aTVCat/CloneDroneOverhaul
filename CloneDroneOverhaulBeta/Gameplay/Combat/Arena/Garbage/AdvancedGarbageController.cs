using CDOverhaul.LevelEditor;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class AdvancedGarbageController : OverhaulController
    {
        private Transform[] m_OgGarbageBotSpawns;
        private readonly List<Transform> m_ModdedGarbageBotSpawns = new List<Transform>();

        private Transform[] m_OgGarbageDropPoints;
        private readonly List<Transform> m_ModdedGarbageDropPoints = new List<Transform>();

        public override void Initialize()
        {
            GarbageManager m = GarbageManager.Instance;
            m_OgGarbageBotSpawns = m.GarbageRobotSpawnLocations;
            m_OgGarbageDropPoints = m.GarbageDropPoints;
        }

        public void ReplaceVanillaDropPoints(bool modded)
        {
            GarbageManager.Instance.GarbageDropPoints = modded ? m_ModdedGarbageDropPoints.ToArray() : m_OgGarbageDropPoints;
        }

        public void AddGarbageDropPoint(Transform point)
        {
            if (m_ModdedGarbageDropPoints.Contains(point))
            {
                return;
            }

            m_ModdedGarbageDropPoints.Add(point);
            ReplaceVanillaDropPoints(true);
        }

        public void RemoveGarbageDropPoint(Transform point)
        {
            _ = m_ModdedGarbageDropPoints.Remove(point);
            if (m_ModdedGarbageDropPoints.Count == 0)
            {
                ReplaceVanillaDropPoints(false);
            }
        }

        public void ReplaceGarbageBotSpawnPoints(bool modded)
        {
            GarbageManager.Instance.GarbageRobotSpawnLocations = modded ? m_ModdedGarbageBotSpawns.ToArray() : m_OgGarbageBotSpawns;
        }

        public void AddGarbageBotSpawnPoint(Transform point)
        {
            if (m_ModdedGarbageBotSpawns.Contains(point))
            {
                return;
            }

            m_ModdedGarbageBotSpawns.Add(point);
            ReplaceGarbageBotSpawnPoints(true);
        }

        public void RemoveGarbageBotSpawnPoint(Transform point)
        {
            _ = m_ModdedGarbageBotSpawns.Remove(point);
            if (m_ModdedGarbageBotSpawns.Count == 0)
            {
                ReplaceGarbageBotSpawnPoints(false);
            }
        }

        public bool HasToEndGarbageCollection()
        {
            foreach (Transform t in m_ModdedGarbageBotSpawns)
            {
                if (t.GetComponent<LevelEditorGarbageBotSpawnpoint>().HasAliveBot)
                {
                    return false;
                }
            }
            return true;
        }

        public Material GetForcefieldMaterial()
        {
            return GarbageManager.Instance.GarbageForceField.transform.GetChild(0).GetComponent<Renderer>().material;
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