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

        protected override void InitGame()
        {
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
                GameplayResultManager.Instance.CreateCardPosLog(card.CardProperty.sprite.ToString(),
                    rectTransform.position.x, rectTransform.position.y);
                //Debug.Log(rectTransform.position);
            });
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
                        //SceneManager.LoadScene("Menu");
                        rewardPanel.SetActive(true);

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