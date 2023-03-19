using CDOverhaul.LevelEditor;
using TMPro;
using UnityEngine;

namespace CDOverhaul.ArenaRemaster
{
    public class ArenaRemasterEnemyCounter : OverhaulBehaviour
    {
        private TextMeshPro m_Header;
        private TextMeshPro m_Label;

        private ArenaRemasterPrototypeController m_Controller;
        private Vector3 m_originalPosition;
        private Vector3 m_originalEulerAngles;

        private int m_EnemiesLeftLastTimeChecked;

        public void Initialize(in ModdedObject moddedObject, in ArenaRemasterPrototypeController arenaController)
        {
            m_originalPosition = base.transform.position;
            m_originalEulerAngles = base.transform.eulerAngles;
            m_Controller = arenaController;

            m_Label = moddedObject.GetObject<TextMeshPro>(0);
            m_Label.text = "-";
            m_Header = moddedObject.GetObject<TextMeshPro>(1);
        }

        protected override void OnDisposed()
        {
            m_Header = null;
            m_Label = null;
            m_Controller = null;
        }

        private void eventRefreshEnemiesLeft(Character c)
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            int count = CharacterTracker.Instance.GetNumEnemyCharactersAlive();
            if(count != m_EnemiesLeftLastTimeChecked)
            {
                m_Label.text = count == 0 ? "-" : count.ToString();
            }
            m_EnemiesLeftLastTimeChecked = count;
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }

            if (Time.frameCount % 10 == 0)
            {
                eventRefreshEnemiesLeft(null);
                LevelEditorArenaEnemiesCounterPoser poser = m_Controller.EnemiesLeftPositionOverride;
                if (poser != null)
                {
                    base.transform.position = poser.transform.position;
                    base.transform.eulerAngles = poser.transform.eulerAngles;
                }
                else
                {
                    base.transform.transform.position = m_originalPosition;
                    base.transform.transform.eulerAngles = m_originalEulerAngles;
                }
            }
        }
    }
}
