using Model;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MatchingGame.Gameplay
{
    public class LevelManager : GameManager
    {
        [SerializeField] GameObject settingPanel;
        [SerializeField] GameObject rewardPanel;

        private int clickCount = 0;
        private int matchFalseCount = 0;

        protected override void Start()
        {
            base.Start();
            settingPanel.SetActive(false);
            rewardPanel.SetActive(false);

            //TODO: Follow Sequence To Setting
            var sequenceSetting = SequenceManager.Instance.GetSequenceDetail().GetGameplaySequenceSetting();
            GamplayLayoutSetting layoutSetting = new GamplayLayoutSetting();
            layoutSetting.categoryTheme = sequenceSetting.categoryTheme;
            layoutSetting.targetPairType = sequenceSetting.pairType;
            layoutSetting.gameDifficult = sequenceSetting.GameDifficult;
            layoutSetting.gameLayout = sequenceSetting.layout;
            setting.SetGameplaySetting(layoutSetting);

            InitGame();
        }

        protected override void Update()
        {
            if (Input.GetMouseButtonDown(0) && _state == GameState.PLAYING)
            {
                clickCount++;
                GameplayResultManager.Instance.GameplayClickLogList.Add(new GameplayClickLog(Input.mousePosition.x, Input.mousePosition.y, UIManager.Instance.Timer, GameplayClickStatusEnum.OUT_CARD, GameplayClickResultEnum.REPEAT)); 
                SoundManager.Instance.PlaySoundEffect(SoundType.Click);
            }
        }

        protected override void InitGame()
        {
            clickCount = 0;
            matchFalseCount = 0;
            _state = GameState.PENDING;
            pairConfig = GameplayResources.Instance.PairConfigData.pairConfigs.Find(x => setting.TargetPairType == x.pairType);
            _targetPairMatchCount = (int)pairConfig.pairType;
            _remainPairMatchCount = _targetPairMatchCount;
            ShowMatchCount.Instance.Init(_remainPairMatchCount);

            InitializeCards();
            SettingLayout();

            UIManager.Instance.Init();
            UIManager.Instance.OnTime += OnFadeInComplete;

            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                UIManager.Instance.BeginCountDownStartGame(true);
                _state = GameState.FADE_IN_UI;
            }).AddTo(this);
        }

        protected override void InitializeCards()
        {
            List<CardProperty> cardPropList = new List<CardProperty>();
            if (SequenceManager.Instance.GetSequenceDetail().GetGameplaySequenceSetting().isForceCardID)
            {
                var randomedCard = GameplayUtils.GetCardFromTargetID
                    (SequenceManager.Instance.GetSequenceDetail().GetGameplaySequenceSetting().cardIDList,
                    GameplayResources.Instance.CardCategoryDataDic[setting.CategoryTheme]);
                cardPropList = GameplayUtils.CreateCardPair(setting.GameDifficult, randomedCard);
                cardPropList = new List<CardProperty>(GameplayUtils.ShuffleCard(cardPropList, pairConfig.roundShuffle));

                //foreach (var cardProp in cardPropList)
                //{
                //    Card card = Instantiate(setting.cardPrefab);
                //    card.Init(cardProp);
                //    _cardList.Add(card);
                //}
            }
            else
            {
                var randomedCard = GameplayUtils.GetRndCardFromTargetAmount(_targetPairMatchCount, setting.GameDifficult, GameplayResources.Instance.CardCategoryDataDic[setting.CategoryTheme]);
                cardPropList = GameplayUtils.CreateCardPair(setting.GameDifficult, randomedCard);
                cardPropList = new List<CardProperty>(GameplayUtils.ShuffleCard(cardPropList, pairConfig.roundShuffle));

                //foreach (var cardProp in cardPropList)
                //{
                //    Card card = Instantiate(setting.cardPrefab);
                //    card.Init(cardProp);
                //    _cardList.Add(card);
                //}
            }

            foreach (var cardProp in cardPropList)
            {
                Card card = Instantiate(setting.cardPrefab);
                card.Init(cardProp);
                _cardList.Add(card);
            }

        }

        public override void OnFadeInComplete()
        {
            UIManager.Instance.OnTime -= OnFadeInComplete;

            UIManager.Instance.BeginCountDownShowCard();
            UIManager.Instance.OnTime += StartGame;

            var gameResult = GameplayResultManager.Instance;
            gameResult.GamePlayResult.StageID = SequenceManager.Instance.GetSequenceDetail().stageID;
            gameResult.GamePlayResult.CardPair = setting.TargetPairType;
            gameResult.GamePlayResult.CardPatternLayout = setting.GameLayout;
            gameResult.GamePlayResult.GameDifficult = setting.GameDifficult;
            gameResult.GamePlayResult.ScreenHeight = Screen.height;
            gameResult.GamePlayResult.ScreenWidth = Screen.width;
        }

        public override void StartGame()
        {
            UIManager.Instance.OnTime += StartGame;
            foreach (var item in _cardList)
            {
                item.FlipCard(CardState.FACE_DOWN);
            }

            _state = GameState.PLAYING;

            _cardList.ForEach(card => {
                RectTransform rectTransform = card.gameObject.GetComponent<RectTransform>();
                GameplayResultManager.Instance.CreateCardPosLog(card.CardProperty.sprite.name.ToString(),
                    rectTransform.position.x, rectTransform.position.y);
                //Debug.Log(rectTransform.position);
            });
        }

        public override void OnCardClick()
        {
            GameplayResultManager.Instance.GameplayClickLogList[^1].ClickStatus = GameplayClickStatusEnum.ON_CARD;
        }

        protected override void OnSelectCardAdd(Card card)
        {
            if (_selectedCardList.Count == 1)
                GameplayResultManager.Instance.GameplayClickLogList[^1].ClickResult = GameplayClickResultEnum.UNMATCH;
            else
                card.IndexClick = GameplayResultManager.Instance.GameplayClickLogList.Count - 1;
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
                GameplayResultManager.Instance.GameplayClickLogList[_selectedCardList[1].IndexClick].ClickResult = GameplayClickResultEnum.MATCHED;
                ShowMatchCount.Instance.OnMatch(_targetPairMatchCount - _remainPairMatchCount);
                _selectedCardList.ForEach(card => card.SelectedCorrect());
                _remainPairMatchCount--;

                ClearSelectCardList();

                if (_remainPairMatchCount <= 0)
                {
                    _state = GameState.RESULT;
                    disposable = GameplayUtils.CountDown(1.0f).ObserveOnMainThread().Subscribe(_ => { }, () =>
                    {
                        //SceneManager.LoadScene("Menu");
                        rewardPanel.SetActive(true);
                        GameplayResultManager.Instance.GamePlayResult.TimeUsed = UIManager.Instance.Timer;
                        GameplayResultManager.Instance.GamePlayResult.ClickCount = clickCount;
                        GameplayResultManager.Instance.GamePlayResult.MatchFalseCount = matchFalseCount;
                        GameplayResultManager.Instance.GamePlayResult.CompletedAt = DateTime.Now;
                        GameplayResultManager.Instance.OnEndGame();

                        disposable.Dispose();
                    }).AddTo(this);
                }
                else
                {
                    SoundManager.Instance.PlaySoundEffect(SoundType.CorrectMatch);
                }
            }
            else
            {
                matchFalseCount++;
                SoundManager.Instance.PlaySoundEffect(SoundType.WrongMatch);
                GameplayResultManager.Instance.GameplayClickLogList[_selectedCardList[1].IndexClick].ClickResult = GameplayClickResultEnum.FALSE_MATCH;
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

        public void Rematch()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void NextMatch()
        {
            SequenceManager.Instance.NextSequence();
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            settingPanel.SetActive(true);
        }

        public void Resume()
        {
            Time.timeScale = 1;
        }
    }
}