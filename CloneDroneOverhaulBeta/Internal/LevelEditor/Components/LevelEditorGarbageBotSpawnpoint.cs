using CDOverhaul.Gameplay;
using CDOverhaul.Patches;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    public class LevelEditorGarbageBotSpawnpoint : MonoBehaviour
    {
        private AdvancedGarbageController m_Controller;

        private Transform m_Bot;
        private bool m_SpawnedABot;

        [IncludeInLevelEditor(false, false)]
        public string PlayOnceEnded;

        public bool HasAliveBot => m_SpawnedABot;

        private void Start()
        {
            GetComponent<ModdedObject>().GetObject<Transform>(0).gameObject.SetActive(GameModeManager.IsInLevelEditor());

            if (!GameModeManager.IsInLevelEditor())
            {
                m_Controller = OverhaulController.GetController<AdvancedGarbageController>();
                m_Controller.AddGarbageBotSpawnPoint(this.transform);
            }
        }

        [CallFromAnimation]
        public void SpawnBot()
        {
            if(m_Bot != null)
            {
                return;
            }
            m_SpawnedABot = true;
            m_Bot = EnemyFactory.Instance.SpawnEnemy(EnemyType.HappyGarbageRobot, base.transform.position, base.transform.eulerAngles);
        }

        [CallFromAnimation]
        public void RemoveBot()
        {
            if (m_Bot == null)
            {
                return;
            }
            Destroy(m_Bot.gameObject);
        }

        private void check()
        {
            if (m_Controller.HasToEndGarbageCollection())
            {
                if (!string.IsNullOrEmpty(PlayOnceEnded))
                {
                    List<LevelEditorAnimation> anims = LevelEditorAnimationManager.Instance.GetAnimationsInLevel();
                    foreach(LevelEditorAnimation anim in anims)
                    {
                        if (!anim.IsPlaying() && anim.AnimationName.Equals(PlayOnceEnded))
                        {
                            anim.Play();
                            break;
                        }
                    }
                }
            }
        }

        private void Update()
        {
            if(m_SpawnedABot && (m_Bot == null || !m_Bot.GetComponent<GarbageRobot>().IsAlive()))
            {
                m_SpawnedABot = false;
                check();
            }
        }

        private void OnDestroy()
        {
            OverhaulController.GetController<AdvancedGarbageController>().RemoveGarbageBotSpawnPoint(this.transform);
        }
    }
}