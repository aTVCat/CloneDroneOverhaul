using CDOverhaul.LevelEditor;
using System.Collections;
using TMPro;
using UnityEngine;

namespace CDOverhaul.ArenaRemaster
{
    public class ArenaRemasterEnemyCounter : MonoBehaviour
    {
        private TextMeshPro _header;
        private TextMeshPro _label;

        private bool _isRefreshingText;

        private ArenaRemasterController _controller;
        private Vector3 _ogPos;
        private Vector3 _ogRot;

        public void Initialize(in ModdedObject moddedObject, in ArenaRemasterController arenaController)
        {
            _ogPos = base.transform.position;
            _ogRot = base.transform.eulerAngles;
            _controller = arenaController;

            _label = moddedObject.GetObject<TextMeshPro>(0);
            _label.text = "-";
            _header = moddedObject.GetObject<TextMeshPro>(1);

            _ = OverhaulEventManager.AddEventListener<Character>(GlobalEvents.CharacterKilled, refreshEnemiesLeft, true);
            _ = OverhaulEventManager.AddEventListener<Character>(GlobalEvents.CharacterStarted, refreshEnemiesLeft, true);
        }

        private void refreshEnemiesLeft()
        {
            if (GameModeManager.IsInLevelEditor())
            {
                refreshLabel(CharacterTracker.Instance.GetNumEnemyCharactersAlive());
                return;
            }
            refreshEnemiesLeft(null);
        }

        private void refreshEnemiesLeft(Character c)
        {
            int count = CharacterTracker.Instance.GetNumEnemyCharactersAlive();
            if (!base.gameObject.activeInHierarchy)
            {
                refreshLabel(count);
                return;
            }

            if (_isRefreshingText)
            {
                _isRefreshingText = false;
                StopAllCoroutines();
            }
            _ = StartCoroutine(refreshText(CharacterTracker.Instance.GetNumEnemyCharactersAlive()));
        }

        private IEnumerator refreshText(int count)
        {
            _isRefreshingText = true;
            yield return new WaitForSeconds(0.2f);

            refreshLabel(count);

            _isRefreshingText = false;
            yield break;
        }

        private void refreshLabel(int count)
        {
            _label.text = count == 0 ? "-" : count.ToString();
        }

        private void stopAll()
        {
            if (_isRefreshingText)
            {
                _isRefreshingText = false;
                StopAllCoroutines();
            }
        }

        private void OnDestroy()
        {
            OverhaulEventManager.RemoveEventListener<Character>(GlobalEvents.CharacterKilled, refreshEnemiesLeft, true);
            stopAll();
        }

        private void OnDisable()
        {
            stopAll();
        }

        private void Update()
        {
            if (GameModeManager.IsInLevelEditor() && Time.frameCount % 30 == 0)
            {
                refreshEnemiesLeft();
            }
            if (Time.frameCount % 10 == 0)
            {
                LevelEditorArenaEnemiesCounterPoser poser = _controller.EnemiesLeftPositionOverride;
                if (poser != null)
                {
                    base.transform.position = poser.transform.position;
                    base.transform.eulerAngles = poser.transform.eulerAngles;
                }
                else
                {
                    base.transform.transform.position = _ogPos;
                    base.transform.transform.eulerAngles = _ogRot;
                }
            }
        }
    }
}
