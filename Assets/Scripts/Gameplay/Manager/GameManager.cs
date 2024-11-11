using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public enum CategoryTheme
    {
        HOME,
        CLOTH,
        MARKET,
        DESSERT
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
        protected SettingGameplay setting;
        protected PairConfig pairConfig = new PairConfig();
        protected int _targetPairMatchCount;
        protected int _remainPairMatchCount;
        protected Transform _targetCardParent;
        protected List<Card> _cardList = new List<Card>();
        protected List<Card> _selectedCardList = new List<Card>();
        protected List<Card> _hintCardList = new List<Card>();
        protected IDisposable disposable;
        protected List<IDisposable> disposableList = new List<IDisposable>();
        protected GameState _state;

        public GameState State { get { return _state; } }

        protected virtual void Start()
        {
            setting = SettingGameplay.Instance;
            _state = GameState.START;

            
        }

        protected virtual void Update()
        {
            //if (Input.GetButtonDown("Fire1"))
            //{
            //    Vector3 mousePos = Input.mousePosition;
            //    {
            //        Debug.Log(mousePos.x);
            //        Debug.Log(mousePos.y);
            //    }
            //}
        }

        protected virtual void InitGame()
        {
            
        }

        //[Button]
        //public void ReCreate()
        //{
        //    _cardList.ForEach(x =>Destroy(x.gameObject));
        //    _cardList.Clear();
        //    InitializeCards();
        //    SettingLayout();
        //}


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

        protected virtual void InitializeCards()
        {
            
        }

        protected virtual void InitializeCards(GameplaySequenceSetting sequenceSetting)
        {

        }

        protected virtual void SettingLayout()
        {
            if (setting.GameLayout == GameLayout.GRID)
            {
                setting.gridLayout.cellSize = new Vector2(pairConfig.cellSize.x, pairConfig.cellSize.y);
                // setting.gridLayout.cellSize = new Vector2(pairConfig.cellSize.x * Screen.width / 1920, pairConfig.cellSize.y * Screen.height / 1080);
                // Debug.Log(pairConfig.spacing.x * (float)Mathf.Pow(1920.0f / Screen.width,2));
                // setting.gridLayout.spacing = new Vector2(pairConfig.spacing.x * Mathf.Pow(1920.0f / Screen.width,2f) , pairConfig.spacing.y * Mathf.Pow(1080.0f / Screen.height,1.9f));
                setting.gridLayout.constraintCount = pairConfig.ConstraintRow;
                _targetCardParent = setting.gridLayout.transform;

                _cardList.ForEach(x => x.gameObject.transform.parent = _targetCardParent);
                for(int i=0;i<setting.gridLayout.transform.childCount;i++){
                    setting.gridLayout.transform.GetChild(i).transform.localScale = new Vector3(1.0f,1.0f,1.0f);
                }
            }
            else if (setting.GameLayout == GameLayout.RANDOM)
            {
                _targetCardParent = setting.randomLayout.transform;
                SetCardTranform setTranform = GameplayResources.Instance.RandomPatternTranformData.GetRandomSetCard(setting.TargetPairType);
                for (int i = 0; i < _cardList.Count; i++)
                {
                    Card card = _cardList[i];
                    RectTransform rectTransform = card.gameObject.GetComponent<RectTransform>();
                    card.gameObject.transform.parent = _targetCardParent;
                    rectTransform.sizeDelta = new Vector2(pairConfig.cellSize.x , pairConfig.cellSize.y );
                    // setting.gridLayout.spacing = new Vector2(pairConfig.spacing.x * (1+ Screen.width / 1920) , pairConfig.spacing.y * (1+Screen.height / 1080) );
                    rectTransform.localPosition = setTranform.tranformCardInSet[i].position;
                    rectTransform.localEulerAngles = setTranform.tranformCardInSet[i].rotation;
                }
                for(int i=0;i<setting.randomLayout.transform.childCount;i++){
                    setting.randomLayout.transform.GetChild(i).transform.localScale = new Vector3(1.0f,1.0f,1.0f);
                }

                //var rect = _targetCardParent.GetComponent<RectTransform>();
                //rect.rect.xMax
            }
        }

        public virtual void OnFadeInComplete()
        {
            
        }

        public virtual void StartGame()
        {
           
        }

        public virtual void OnCardClick()
        {

        }

        public bool CheckCanFlipCard()
        {
            return _selectedCardList.Count < 2 && _state == GameState.PLAYING;
        }

        public virtual void AddCardToCheck(Card card)
        {
            if (_selectedCardList.Count >= 2)
                return;

            if (_selectedCardList.Contains(card))
                return;

            _selectedCardList.Add(card);
            OnSelectCardAdd(card);
            disposableList.Add(card.onFlipComplete.Subscribe(_ => CheckCard()).AddTo(this));
        }

        protected virtual void OnSelectCardAdd(Card card)
        {

        }

        public virtual void CheckCard()
        {
            
        }

        public void ClearSelectCardList()
        {
            _selectedCardList.Clear();
        }
    }
}