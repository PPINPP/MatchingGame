using System;
using System.Collections;
using System.Collections.Generic;
using Model;
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
        private int clickCount = 0;
        private int matchFalseCount = 0;
        private bool successInit = false;
        private int outCard = 0;
        private int repeatCount = 0;

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
        
        protected override void Update()
        {
            if (Input.GetMouseButtonDown(0) && _state == GameState.PLAYING)
            {
                clickCount++;
                //SoundManager.Instance.PlaySoundEffect(SoundType.Click);
                TutorialResultManager.Instance.TutorialClickLogList.Add(new GameplayClickLog(Input.mousePosition.x, Input.mousePosition.y, 180.0f-UIManager.Instance.Timer, GameplayClickStatusEnum.OUT_CARD, GameplayClickResultEnum.REPEAT));
                outCard++;
                repeatCount++;

            }
        }

        void SetCurrentStageSequence()
        {
            if(curStageObj.sequences[curStageObjIndex].currentSequence == "start_game"){
                UIManager.Instance.BeginCountDownShowCard();
                UIManager.Instance.OnTime += StartGame;
                
                var gameResult = TutorialResultManager.Instance;
                gameResult.TutorialResult.StageID = SequenceManager.Instance.GetSequenceDetail().stageID;
                gameResult.TutorialResult.CardPair = setting.TargetPairType;
                gameResult.TutorialResult.CardPatternLayout = setting.GameLayout;
                gameResult.TutorialResult.GameDifficult = setting.GameDifficult;
                gameResult.TutorialResult.ScreenHeight = Screen.height;
                gameResult.TutorialResult.ScreenWidth = Screen.width;
                return;
            }
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
                
                var gameResult = TutorialResultManager.Instance;
                gameResult.TutorialResult.StageID = SequenceManager.Instance.GetSequenceDetail().stageID;
                gameResult.TutorialResult.CardPair = setting.TargetPairType;
                gameResult.TutorialResult.CardPatternLayout = setting.GameLayout;
                gameResult.TutorialResult.GameDifficult = setting.GameDifficult;
                gameResult.TutorialResult.ScreenHeight = Screen.height;
                gameResult.TutorialResult.ScreenWidth = Screen.width;

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
            successInit = true;
            _cardList.ForEach(card => {
                RectTransform rectTransform = card.gameObject.GetComponent<RectTransform>();
                TutorialResultManager.Instance.CreateCardPosLog(card.CardProperty.sprite.name.ToString(),
                    rectTransform.position.x, rectTransform.position.y);
                //Debug.Log(rectTransform.position);
            });
        }
        
        public override void OnCardClick()
        {
            if (!successInit)
            {
                return;
            }
            TutorialResultManager.Instance.TutorialClickLogList[^1].ClickStatus = GameplayClickStatusEnum.ON_CARD;
        }
        
        protected override void OnSelectCardAdd(Card card)
        {
            if (_selectedCardList.Count == 1)
                TutorialResultManager.Instance.TutorialClickLogList[^1].ClickResult = GameplayClickResultEnum.UNMATCH;
            else
                card.IndexClick = TutorialResultManager.Instance.TutorialClickLogList.Count - 1;
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
                TutorialResultManager.Instance.TutorialClickLogList[_selectedCardList[1].IndexClick].ClickResult = GameplayClickResultEnum.MATCHED;
                ShowMatchCount.Instance.OnMatch(_targetPairMatchCount - _remainPairMatchCount);
                _selectedCardList.ForEach(card => card.SelectedCorrect());
                _remainPairMatchCount--;

                ClearSelectCardList();

                if (_remainPairMatchCount <= 0)
                {
                    AudioController.StopPlayGBM();
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
                        
                        TutorialResultManager.Instance.TutorialResult.TimeUsed = 180-UIManager.Instance.Timer;
                        TutorialResultManager.Instance.TutorialResult.ClickCount = clickCount;
                        TutorialResultManager.Instance.TutorialResult.MatchFalseCount = matchFalseCount;
                        TutorialResultManager.Instance.TutorialResult.CompletedAt = DateTime.Now;
                        GameplayResultManager.Instance.GamePlayResult.OutareaCount = outCard;
                        GameplayResultManager.Instance.GamePlayResult.RepeatCount = repeatCount;
                        TutorialResultManager.Instance.OnEndTutorial();

                        disposable.Dispose();
                    }).AddTo(this);
                    if(SequenceCreator.Instance._testmode){
                        EndTutorial();
                    }
                }
                else
                {
                    //SoundManager.Instance.PlaySoundEffect(SoundType.CorrectMatch);
                }
            }
            else
            {
                matchFalseCount++;
                //SoundManager.Instance.PlaySoundEffect(SoundType.WrongMatch);
                TutorialResultManager.Instance.TutorialClickLogList[_selectedCardList[1].IndexClick].ClickResult = GameplayClickResultEnum.FALSE_MATCH;
                _selectedCardList[1].IndexClick = -1;
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