using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MatchingGame.Gameplay
{


    public class SequenceManager : MonoSingleton<SequenceManager>
    {
        private int currentSequenceIndex = -1;

        private GameplaySequenceSO _sequenceSO;

        public bool _testmode = false;
        public bool _selectormode = false;
        public bool _test2mode = false;
        public bool _ttr4 = false;
        public bool _ttr4_play = false;
        public int game_no;
        public int game_score = 1;

        public override void Init()
        {
            base.Init();
            if (this._sequenceSO == null)
                this._sequenceSO = Resources.Load<GameplaySequenceSO>("Gameplay/ScriptableObjects/GameplaySequence");
        }

        public void ResetGame()
        {
            currentSequenceIndex = -1;
        }

        public void ReloadSequence(GameplaySequenceSO newSequence)
        {
            this._sequenceSO = newSequence;
            ResetGame();
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

            // End Of Sequence
            if (currentSequenceIndex >= _sequenceSO.sequences.Count)
            {
                //Check
                if (PlayerPrefs.HasKey("daily_check"))
                {
                    if (DateTime.Now.Day != PlayerPrefs.GetInt("daily_check"))
                    {
                        SequenceDetail sequenceDetail = new SequenceDetail()
                        {
                            isDailyFeeling = true,
                        };
                        _sequenceSO.sequences.Add(sequenceDetail);
                        PlayerPrefs.SetInt("daily_check", DateTime.Now.Day);
                        LoadScene();
                        return;
                    }
                }
                else
                {
                    SequenceDetail sequenceDetail = new SequenceDetail()
                    {
                        isDailyFeeling = true,
                    };
                    _sequenceSO.sequences.Add(sequenceDetail);
                    PlayerPrefs.SetInt("daily_check", DateTime.Now.Day);
                    LoadScene();
                    return;
                }

                //EndCheck
                currentSequenceIndex = _sequenceSO.sequences.Count - 1;
                // SceneManager.LoadScene(GameplayResources.Instance.SceneNames.uiTestScene);
                if (_test2mode)
                {
                    _test2mode = false;
                    SceneManager.LoadScene("LevelSelector");
                    return;
                }
                if (_testmode)
                {
                    SequenceCreator.Instance.Reset();
                    SceneManager.LoadScene("SequenceScriptTester");
                    return;
                }
                if (_selectormode)
                {
                    LevelSelectorManager.Instance.OnSuccessLevel(game_no, game_score);
                    LevelSelectorManager.Instance.Reset();
                    SceneManager.LoadScene("LevelSelector");
                    return;
                }
                SceneManager.LoadScene("EndTest");
            }
            // Next Sequence
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
            else if (_sequenceSO.sequences[currentSequenceIndex].isDailyFeeling)
            {
                SceneManager.LoadScene(GameplayResources.Instance.SceneNames.dailyScene);
            }

        }
        public GameplaySequenceSO GetSO()
        {
            return _sequenceSO;
        }
    }
}
