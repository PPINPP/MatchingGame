using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MatchingGame.Gameplay
{
    public enum GameDifficult
    {
        EASY,
        NORMAL,
        HARD,
        ADVANCE
    }

    public enum GameLayout
    {
        GRID,
        RANDOM
    }

    public enum ThemeCategory
    {
        HOME,
        CLOTH,
        MARKET
    }

    public enum GameState
    {
        START,
        PENDING,
        FADE_IN_UI,
        PLAYING,
        PAUSE,
        RESULT
    }

    public class GameManager : MonoInstance<GameManager>
    {
        [Header("Gamplay Setting")]
        [SerializeField] ThemeCategory categoryTheme;
        [SerializeField] PairType targetPairType;
        [SerializeField] GameDifficult gameDifficult;
        [SerializeField] GameLayout gameLayout;

        [SerializeField] Card cardPrefab;

        [Space(10)]
        [Header("UI Elements")]
        [SerializeField] GridLayoutGroup gridLayout;
        [SerializeField] GameObject randomLayout;
        [SerializeField] TextMeshProUGUI matchText;
        

        private PairConfig pairConfig = new PairConfig();
        private int _targetPairMatchCount;
        private int _remainPairMatchCount;
        private Transform _targetCardParent;
        private List<Card> _cardList = new List<Card>();
        private List<Card> _selectedCardList = new List<Card>();
        private IDisposable disposable;
        private List<IDisposable> disposableList = new List<IDisposable>();

        private GameState _state;

        public GameState State { get { return _state; } }

        private void Start()
        {
            _state = GameState.START;
            GameplayResources.Instance.Init();
            
            InitGame();
        }

        private void InitGame()
        {
            _state = GameState.PENDING;
            pairConfig = GameplayResources.Instance.PairConfigData.pairConfigs.Find(x => targetPairType == x.pairType);
            _targetPairMatchCount = (int)pairConfig.pairType;
            _remainPairMatchCount = _targetPairMatchCount;
            matchText.text = $"Number of Matches : {_remainPairMatchCount}";
            ShowMatchCount.Instance.Init(_remainPairMatchCount);


            SettingLayout();
            InitializeCards();

            UIManager.Instance.Init();
            UIManager.Instance.OnTime += StartGame;

            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => { 
                UIManager.Instance.StartCountDown();
                _state = GameState.FADE_IN_UI;
            }).AddTo(this);
        }

        private void SettingLayout()
        {
            if (gameLayout == GameLayout.GRID)
            {
                gridLayout.cellSize = new Vector2(pairConfig.cellSize.x, pairConfig.cellSize.y);
                gridLayout.constraintCount = pairConfig.ConstraintRow;
                _targetCardParent = gridLayout.transform;
            }
            else if (gameLayout == GameLayout.RANDOM)
            {
                _targetCardParent = randomLayout.transform;
                var rect = _targetCardParent.GetComponent<RectTransform>();
                //rect.rect.xMax
            }
        }

        //[Button]
        //private void TestRandom()
        //{
        //    var randomedCard = Utils.GetRndCardFromTargetAmount(_targetPairMatchCount, gameDifficult, GameplayResources.Instance.CardCatedoryDataDic[categoryTypeKey]);
        //    randomedCard.ForEach(x => {
        //        print($"===== Check ====");
        //        print($"Name : {x.Key}");
        //        print($"Easy : {x.Value.isEasy}");
        //        print($"2. Easy : {x.Value.easySprite}");
        //        print($"Normal : {x.Value.isNormal}");
        //        print($"2. Normal : {x.Value.normalSprite}");
        //        print($"Hard : {x.Value.isHard}");
        //        print($"2. Hard : {x.Value.hardSprite}");
        //        print($"Advance : {x.Value.isAdvanced}");
        //        print($"2. Advance : {x.Value.advanceSprite}");
        //    });

        //    var cardPropList = Utils.CreateCardPair(gameDifficult, randomedCard);
        //    print("***********");
        //    print($"Prop Count : {cardPropList.Count}");

        //    foreach (var item in cardPropList)
        //    {
        //        print("====");
        //        print($"1.key : {item.key}");
        //        print($"2.sprite {item.sprite.name}");
        //    }
        //}

        private void InitializeCards()
        {
            var randomedCard = GameplayUtils.GetRndCardFromTargetAmount(_targetPairMatchCount, gameDifficult, GameplayResources.Instance.CardCategoryDataDic[categoryTheme]);
            var cardPropList = GameplayUtils.CreateCardPair(gameDifficult, randomedCard);
            cardPropList = new List<CardProperty>(GameplayUtils.ShuffleCard(cardPropList, pairConfig.roundShuffle));

            foreach (var cardProp in cardPropList)
            {
                Card card = Instantiate(cardPrefab, _targetCardParent);
                card.Init(cardProp);
                _cardList.Add(card);
            }
        }

        public void StartGame()
        {
            disposable = GameplayUtils.CountDown(GameplayResources.Instance.GameplayProperty.FirstTimeShowDuration).ObserveOnMainThread().Subscribe(_ => { }, () =>
            {
                foreach (var item in _cardList)
                {
                    item.FlipCard(CardState.FACE_DOWN);
                }

                _state = GameState.PLAYING;
                disposable.Dispose();
            }).AddTo(this);
        }

        public bool CheckCanFlipCard()
        {
            return _selectedCardList.Count < 2 && _state == GameState.PLAYING;
        }

        public void AddCardToCheck(Card card)
        {
            if (_selectedCardList.Count >= 2)
                return;

            if (_selectedCardList.Contains(card))
                return;

            _selectedCardList.Add(card);
            disposableList.Add(card.onFlipComplete.Subscribe(_ => CheckCard()).AddTo(this));
        }

        public void CheckCard()
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
                matchText.text = $"Number of Matches : {_remainPairMatchCount}";

                ClearCardList();

                if (_remainPairMatchCount <= 0)
                {
                    disposable = GameplayUtils.CountDown(1.0f).ObserveOnMainThread().Subscribe(_ => { }, () =>
                    {
                        SceneManager.LoadScene("Menu");

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
                    ClearCardList();

                    disposable.Dispose();
                }).AddTo(this);
            }
        }

        public void ClearCardList()
        {
            _selectedCardList.Clear();
        }

    }
}