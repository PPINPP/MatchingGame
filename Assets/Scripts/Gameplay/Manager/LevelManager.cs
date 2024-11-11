using Model;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MatchingGame.Gameplay
{
    public class LevelManager : GameManager
    {
        [SerializeField] GameObject settingPanel;
        [SerializeField] GameObject pausePanel;
        [SerializeField] GameObject rewardPanel;
        [SerializeField] Button addTime;
        [SerializeField] Button flipCard;
        [SerializeField] Button pauseGame;
        [SerializeField] Image pauseImage;
        [SerializeField] GameObject disableArea;
        [SerializeField] Sprite pauseSprite;
        [SerializeField] Sprite playSprite;

        private int clickCount = 0;
        private int matchFalseCount = 0;
        private bool successInit = false;
        private float stopTime = 0f;
        private float lastClick = -1f;
        private float referenceTime = 0f;
        private float startHintTime = 0f;
        private bool addedTime = false;
        private List<string> keyContain = new List<string>();

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
                lastClick = Time.time;
                ClearHint();
                clickCount++;
                GameplayResultManager.Instance.GameplayClickLogList.Add(new GameplayClickLog(Input.mousePosition.x, Input.mousePosition.y, UIManager.Instance.Timer, GameplayClickStatusEnum.OUT_CARD, GameplayClickResultEnum.REPEAT));
                SoundManager.Instance.PlaySoundEffect(SoundType.Click);
            }
            if (Time.time - lastClick > 5.0f && lastClick != -1f)
            {
                lastClick = -1f;
                TriggerPassive();
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

            _cardList.ForEach(card =>
            {
                RectTransform rectTransform = card.gameObject.GetComponent<RectTransform>();
                GameplayResultManager.Instance.CreateCardPosLog(card.CardProperty.sprite.name.ToString(),
                    rectTransform.position.x, rectTransform.position.y);
                //Debug.Log(rectTransform.position);
            });
            successInit = true;
            foreach (var item in _cardList)
            {
                keyContain.Add(item.CardProperty.key);
            }
            referenceTime = Time.time;
        }

        public override void OnCardClick()
        {
            if (!successInit)
            {
                return;
            }
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
                for (int i = 0; i < keyContain.Count; i++)
                {
                    if (keyContain[i] == _selectedCardList[0].CardProperty.key)
                    {
                        keyContain[i] = "";
                    }
                }
                AudioController.SetnPlay("audio/SFX/Correct_Match");
                GameplayResultManager.Instance.GameplayClickLogList[_selectedCardList[1].IndexClick].ClickResult = GameplayClickResultEnum.MATCHED;
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
                        //SceneManager.LoadScene("Menu");
                        ClearHint();
                        lastClick = -1f;
                        rewardPanel.SetActive(true);
                        GameplayResultManager.Instance.GamePlayResult.TimeUsed = addedTime ? 180-UIManager.Instance.Timer:150-UIManager.Instance.Timer;
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
                AudioController.SetnPlay("audio/SFX/Wrong_Match");
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
            pauseImage.sprite = playSprite;
            Time.timeScale = 0;
            stopTime = Time.realtimeSinceStartup;
            pausePanel.SetActive(true);
        }

        public void Resume()
        {
            pauseImage.sprite = pauseSprite;
            Time.timeScale = 1;
            PauseLog pauseLog = new PauseLog(addedTime ? (180.0f-UIManager.Instance.GetTimer()).ToString() : (150.0f-UIManager.Instance.GetTimer()).ToString(), (Time.realtimeSinceStartup - stopTime).ToString());
            GameplayResultManager.Instance.GamePlayResult.PauseLogList.Add(pauseLog);
            pausePanel.SetActive(false);
        }
        public void AddTime()
        {
            GameplayResultManager.Instance.GamePlayResult.AddTimeUsed = 150.0f - UIManager.Instance.GetTimer();
            UIManager.Instance.AddTime(30.0f);
            addTime.interactable = false;
            addedTime = true;
            

        }
        public void FlipAll()
        {
            flipCard.interactable = false;
            disableArea.SetActive(true);
            foreach (var item in _cardList)
            {
                if (item.IsInComplete())
                {
                    item.FlipCard(CardState.FACE_UP);
                }

            }
            System.Collections.IEnumerator startFlip()
            {
                yield return new WaitForSeconds(5);// Wait a bit
                disableArea.SetActive(false);
                foreach (var item in _cardList)
                {
                    if (item.IsInComplete())
                    {
                        item.FlipCard(CardState.FACE_DOWN);
                    }

                }
            }
            if(addedTime){
                GameplayResultManager.Instance.GamePlayResult.FlipAllUsed = 180.0f- UIManager.Instance.GetTimer();
            }else{
                GameplayResultManager.Instance.GamePlayResult.FlipAllUsed = 150.0f- UIManager.Instance.GetTimer();
            }
            
            StartCoroutine(startFlip());
        }
        public void StartPassive()
        {
            lastClick = Time.time;
        }
        public void TriggerPassive()
        {
            var a = Random.Range(0, keyContain.Count);
            while(keyContain[a] == "")
            {
                a = a + 1;
                if (a == keyContain.Count)
                {
                    a = 0;
                }
            }
            foreach (var item in _cardList)
            {
                if (item.CardProperty.key == keyContain[a])
                {
                    _hintCardList.Add(item);
                }
            }
            _hintCardList[0].StartFading();
            _hintCardList[1].StartFading();
            startHintTime = Time.time;

        }
        public void ClearHint()
        {
            if (_hintCardList.Count == 2)
            {
                if(addedTime){

                }
                PassiveLog passiveLog = new PassiveLog(startHintTime-referenceTime,Time.time-referenceTime,Time.time-startHintTime,_hintCardList[0].CardProperty.key);
                GameplayResultManager.Instance.GamePlayResult.PassiveLogList.Add(passiveLog);
                foreach (var item in _hintCardList)
                {
                    item.StopFading();
                }
                _hintCardList.Clear();
            }
            return;

        }
        public void EndGame()
        {
            ClearHint();
            lastClick = -1f;
            AudioController.StopPlayGBM();
            _state = GameState.RESULT;
            disposable = GameplayUtils.CountDown(0.1f).ObserveOnMainThread().Subscribe(_ => { }, () =>
            {
                //SceneManager.LoadScene("Menu");
                rewardPanel.SetActive(true);
                GameplayResultManager.Instance.GamePlayResult.TimeUsed = addedTime ? 180-UIManager.Instance.Timer:150-UIManager.Instance.Timer;
                GameplayResultManager.Instance.GamePlayResult.ClickCount = clickCount;
                GameplayResultManager.Instance.GamePlayResult.MatchFalseCount = matchFalseCount;
                GameplayResultManager.Instance.GamePlayResult.CompletedAt = DateTime.Now;
                GameplayResultManager.Instance.OnEndGame();

                disposable.Dispose();
            }).AddTo(this);
        }
        public void EnableTools()
        {
            addTime.interactable = true;
            flipCard.interactable = true;
            pauseGame.interactable = true;
        }
    }
}