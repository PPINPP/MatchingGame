using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MatchingGame.Gameplay
{
    public class SequenceManager : MonoSingleton<SequenceManager>
    {
        private int currentSequenceIndex;

        private GameplaySequenceSO _sequenceSO;

        //public GameplaySequenceSO SequenceSO { get { 
            
        //        if (this._sequenceSO == null)
        //        {
        //            this._sequenceSO =  Resources.Load<GameplaySequenceSO>("GameplaySequence");
        //            return this._sequenceSO;
        //        }
        //        else { return this._sequenceSO; }
        //}}

        public override void Init()
        {
            base.Init();
            if (this._sequenceSO == null)
                this._sequenceSO = Resources.Load<GameplaySequenceSO>("ScriptableObjects/GameplaySequence");
        }

        public SequenceDetail GetSequenceDetail()
        {
            return _sequenceSO.sequences[currentSequenceIndex];
        }

        public SequenceDetail GetNextSequenceDetail()
        {
            return currentSequenceIndex + 1 < _sequenceSO.sequences.Count ?
                _sequenceSO.sequences[currentSequenceIndex + 1] : null;
        }

        public void NextSequence()
        {
            currentSequenceIndex++;

            if (currentSequenceIndex >= _sequenceSO.sequences.Count)
            {
                currentSequenceIndex = _sequenceSO.sequences.Count - 1;
                SceneManager.LoadScene(GameplayResources.Instance.SceneNames.uiTestScene);
            }
            else
            {
                LoadScene();
            }

        }

        private void LoadScene()
        {
            if (_sequenceSO.sequences[currentSequenceIndex].isGamePlay)
            {
                if (_sequenceSO.sequences[currentSequenceIndex].GetGameplaySequenceSetting().isTutorial)
                {
                    SceneManager.LoadScene(GameplayResources.Instance.SceneNames.tutorialScene);
                }
                else
                {
                    SceneManager.LoadScene(GameplayResources.Instance.SceneNames.gameplayScene);
                }
            }
            else if (_sequenceSO.sequences[currentSequenceIndex].isSmileyOMeter)
            {
                SceneManager.LoadScene(GameplayResources.Instance.SceneNames.smileScene);
            }
            else if (_sequenceSO.sequences[currentSequenceIndex].isMinigame)
            {
                SceneManager.LoadScene(GameplayResources.Instance.SceneNames.minigameScene);
            }

        }
    }
}
