using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace MatchingGame.Gameplay
{
    [Serializable]
    public class StageObj
    {
        public List<StageSequence> sequences = new List<StageSequence>();
        public Dictionary<string, Transform> map = new Dictionary<string, Transform>();
    }


    [Serializable]
    public class StageSequence
    {
        public string currentSequence;
        public string nextSequence;
    }

    public class TutorialManager : GameManager
    {
        [SerializeField] public Dictionary<string, StageObj> stage;
        [SerializeField] GameObject fisnishTutorialPanel;
        [SerializeField] GameObject endTutorialPanel;

        private string stageID;
        private StageObj curStageObj;

        private int curStageObjIndex = 0;

        protected override void Start()
        {
            base.Start();

            InitGame();
            fisnishTutorialPanel.SetActive(false);
            endTutorialPanel.SetActive(false);
        }

        protected override void InitGame()
        {
            UIManager.Instance.SetEnableUI();
            var sequence = SequenceManager.Instance.GetSequenceDetail();
            var sequenceSetting = sequence.GetGameplaySequenceSetting();

            GamplayLayoutSetting layoutSetting = new GamplayLayoutSetting();
            layoutSetting.categoryTheme = sequenceSetting.categoryTheme;
            layoutSetting.targetPairType = sequenceSetting.pairType;
            layoutSetting.gameDifficult = sequenceSetting.GameDifficult;
            layoutSetting.gameLayout = sequenceSetting.layout;
            setting.SetGameplaySetting(layoutSetting);

            if (sequence != null && sequence.isGamePlay)
            {
                if (sequenceSetting.isTutorial)
                {
                    stageID = sequence.stageID;
                    if (stage.TryGetValue(stageID, out curStageObj))
                    {
                        SetCurrentStageSequence();

                        _state = GameState.PENDING;
                        pairConfig = GameplayResources.Instance.PairConfigData.pairConfigs.Find(x => setting.TargetPairType == x.pairType);
                        _targetPairMatchCount = (int)pairConfig.pairType;
                        _remainPairMatchCount = _targetPairMatchCount;
                        ShowMatchCount.Instance.Init(_remainPairMatchCount);

                        InitializeCards(sequenceSetting);
                        SettingLayout();
                    }
                    else
                    {
                        print($"Can't Find {stageID}");
                    }
                }
            }
        }

        void SetCurrentStageSequence()
        {
            curStageObj.map.TryGetValue(curStageObj.sequences[curStageObjIndex].currentSequence, out Transform sequenceTrans);
            sequenceTrans.gameObject.SetActive(true);
            Button button = sequenceTrans.GetComponentInChildren<Button>();
            button.onClick.AddListener(NextStageSequence);
            //TODO: Set button to click next
        }

        void NextStageSequence()
        {
            curStageObj.map.TryGetValue(curStageObj.sequences[curStageObjIndex].currentSequence, out Transform oldSequenceTrans);
            Button oldButton = oldSequenceTrans.GetComponentInChildren<Button>();
            oldButton.onClick.RemoveAllListeners();
            oldSequenceTrans.gameObject.SetActive(false);

            if (curStageObj.sequences[curStageObjIndex].nextSequence != "start_game")
            {
                curStageObjIndex++;
                if (curStageObjIndex >= curStageObj.sequences.Count)
                    curStageObjIndex = curStageObj.sequences.Count - 1;
                SetCurrentStageSequence();
            }
            else
            {
                UIManager.Instance.BeginCountDownShowCard();
                UIManager.Instance.OnTime += StartGame;

                //Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ => {
                //    EndTutorial();
                //}).AddTo(this);

            }
        }

        protected override void InitializeCards(GameplaySequenceSetting sequenceSetting)
        {
            var randomedCard = GameplayUtils.GetCardFromTargetID(sequenceSetting.cardIDList, GameplayResources.Instance.CardCategoryDataDic[setting.CategoryTheme]);
            var cardPropList = GameplayUtils.CreateCardPair(setting.GameDifficult, randomedCard);
            cardPropList = new List<CardProperty>(GameplayUtils.ShuffleCard(cardPropList, pairConfig.roundShuffle));
            
            foreach (var cardProp in cardPropList)
            {
                Card card = Instantiate(setting.cardPrefab);
                card.Init(cardProp);
                _cardList.Add(card);
            }
        }

        public override void StartGame()
        {
            UIManager.Instance.OnTime += StartGame;
            foreach (var item in _cardList)
            {
                item.FlipCard(CardState.FACE_DOWN);
            }

            _state = GameState.PLAYING;
        }

        public override void CheckCard()
        {
            if (_selectedCardList.Count < 2) return;

            var cardFliping = _selectedCardList.Find(x => x.IsFliping);

            if (cardFliping != null) return;

            disposableList.ForEach(dispos => dispos.Dispose());
            disposableList.Clear();

            if (string.Equals(_selectedCardList[0].CardProperty.key, _selectedCardList[1].CardProperty.key))
            {
                ShowMatchCount.Instance.OnMatch(_targetPairMatchCount - _remainPairMatchCount);
                _selectedCardList.ForEach(card => card.SelectedCorrect());
                _remainPairMatchCount--;

                ClearSelectCardList();

                if (_remainPairMatchCount <= 0)
                {
                    _state = GameState.RESULT;
                    disposable = GameplayUtils.CountDown(1.0f).ObserveOnMainThread().Subscribe(_ => { }, () =>
                    {
                        if (SequenceManager.Instance.GetNextSequenceDetail() != null)
                        {
                            if (SequenceManager.Instance.GetNextSequenceDetail().isGamePlay &&
                                SequenceManager.Instance.GetNextSequenceDetail().GetGameplaySequenceSetting().isTutorial)
                            {
                                fisnishTutorialPanel.SetActive(true);
                            }
                            else
                            {
                                endTutorialPanel.SetActive(true);
                            }
                        }
                       

                        disposable.Dispose();
                    }).AddTo(this);
                }
            }
            else
            {
                disposable = GameplayUtils.CountDown(GameplayResources.Instance.GameplayProperty.WrongPairShowDuration).ObserveOnMainThread().Subscribe(_ => { }, () =>
                {
                    _selectedCardList.ForEach(card =>
                    {
                        card.FlipCard(CardState.FACE_DOWN);
                    });
                    ClearSelectCardList();

                    disposable.Dispose();
                }).AddTo(this);
            }
        }

        public void Retutorial()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void EndTutorial()
        {
            SequenceManager.Instance.NextSequence();
        }
    }
}