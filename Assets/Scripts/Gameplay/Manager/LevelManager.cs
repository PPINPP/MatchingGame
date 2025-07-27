using Model;
using System;
using System.Collections.Generic;
using Enum;
using Experiment;
using UniRx;
using Unity.VisualScripting;
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
        [SerializeField] Button settingPage;
        [SerializeField] Image pauseImage;
        [SerializeField] GameObject disableArea;
        [SerializeField] Sprite pauseSprite;
        [SerializeField] Sprite playSprite;
        [SerializeField] Image backGround;
        [SerializeField] List<Sprite> themeBackGround;
        [SerializeField] Button playArea;
        [SerializeField] GameObject dimBackground;
        [SerializeField] GameObject gridObject;
        [SerializeField] GameObject randObject;
        [SerializeField] GameObject helper;
        [SerializeField] GameObject helpPanel;
        [SerializeField] GameObject rightPanel;
        [SerializeField] GameObject endPanel;

        private int clickCount = 0;
        private int matchFalseCount = 0;
        private int matchTotalCount = 0;
        private bool successInit = false;
        private float stopTime = 0f;
        private float lastClick = -1f;
        private float referenceTime = 0f;
        private float startHintTime = 0f;
        private bool addedTime = false;
        private bool flipped = false;
        private bool passiveUsed = false;
        private int outCard = 0;
        private int repeatCount = 0;
        private bool inTutorialState = false;
        private int[] tutorialIndex = new int[2];
        private int ttr4_state = 0;
        private string game_state = "";
        private bool ccOpen = true;
        private bool allCardOpen = false;
        private List<int> CardPhase = new List<int>() { 0, 0, 0 };
        private List<string> HelperSeq = new List<string>();
        private float firstMatchTime = 0f;
        private bool canUseFlipAll = true;
        private List<PhaseData> _gameplayPhaseData = new List<PhaseData>();


        private List<string> keyContain = new List<string>();

        protected override void Start()
        {
            base.Start();
            settingPanel.SetActive(false);
            rewardPanel.SetActive(false);
            playArea.onClick.AddListener(OnPlayAreaClick);
            var sequenceSetting = SequenceManager.Instance.GetSequenceDetail().GetGameplaySequenceSetting();
            GamplayLayoutSetting layoutSetting = new GamplayLayoutSetting();
            backGround.sprite = themeBackGround[(int)sequenceSetting.categoryTheme];
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
                if (!inTutorialState)
                {
                    lastClick = Time.time;
                    ClearHint();
                }
                clickCount++;
                GameplayResultManager.Instance.GameplayClickLogList.Add(new GameplayClickLog(Input.mousePosition.x, Input.mousePosition.y, addedTime ? 210 - UIManager.Instance.Timer : 180 - UIManager.Instance.Timer, GameplayClickStatusEnum.OUT_CARD, GameplayClickResultEnum.REPEAT));
                //SoundManager.Instance.PlaySoundEffect(SoundType.Click);
            }
            if (Time.time - lastClick > 10.0f && lastClick != -1f)
            {
                lastClick = -1f;
                TriggerPassive();
                passiveUsed = true;
                HelperSeq.Add("Passive");
            }
            if (inTutorialState)
            {
                if (_state == GameState.PLAYING)
                {
                    if (_hintCardList.Count > 0)
                    {
                        if (!_hintCardList[0].IsInComplete() && !_hintCardList[1].IsInComplete())
                        {
                            inTutorialState = false;
                            lastClick = Time.time;
                            if (GameplayResultManager.Instance.GamePlayResult.CardPatternLayout == GameLayout.GRID)
                            {
                                _hintCardList[0].transform.SetParent(gridObject.transform);
                                _hintCardList[0].transform.SetSiblingIndex(tutorialIndex[0]);
                                _hintCardList[1].transform.SetParent(gridObject.transform);
                                _hintCardList[1].transform.SetSiblingIndex(tutorialIndex[1]);
                                gridObject.GetComponent<GridLayoutGroup>().enabled = true;
                            }
                            else
                            {

                                _hintCardList[0].transform.SetParent(randObject.transform);
                                _hintCardList[0].transform.SetSiblingIndex(tutorialIndex[0]);
                                _hintCardList[1].transform.SetParent(randObject.transform);
                                _hintCardList[1].transform.SetSiblingIndex(tutorialIndex[1]);
                            }
                            dimBackground.SetActive(false);
                            FirebaseManagerV2.Instance.gameData["PASSIVE"] = true;
                            FirebaseManagerV2.Instance.SetTutorial(true);
                            ClearHint();
                        }
                    }
                }

            }
        }

        protected override void InitGame()
        {
            clickCount = 0;
            matchFalseCount = 0;
            matchTotalCount = 0;
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
                if (FirebaseManagerV2.Instance.curr_username == "hfelab.come")
                {
                    var randomedCard = GameplayUtils.GetRndCardFromTargetAmountWithPreviousCard(_targetPairMatchCount, setting.GameDifficult, GameplayResources.Instance.CardCategoryDataDic[setting.CategoryTheme], SequenceManager.Instance.GetSequenceDetail().GetGameplaySequenceSetting().categoryTheme.ToString());
                    cardPropList = GameplayUtils.CreateCardPair(setting.GameDifficult, randomedCard);
                    cardPropList = new List<CardProperty>(GameplayUtils.ShuffleCard(cardPropList, pairConfig.roundShuffle));
                }
                else
                {
                    var randomedCard = GameplayUtils.GetRndCardFromTargetAmount(_targetPairMatchCount, setting.GameDifficult, GameplayResources.Instance.CardCategoryDataDic[setting.CategoryTheme]);
                    cardPropList = GameplayUtils.CreateCardPair(setting.GameDifficult, randomedCard);
                    cardPropList = new List<CardProperty>(GameplayUtils.ShuffleCard(cardPropList, pairConfig.roundShuffle));
                }



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
            if (GameplayResultManager.Instance.GameplayClickLogList.Count > 0)
            {
                GameplayResultManager.Instance.GameplayClickLogList[^1].ClickStatus = GameplayClickStatusEnum.ON_CARD;
            }

            //Here
        }
        public override void OnCardRepeat()
        {
            repeatCount++;
        }
        public void OnPlayAreaClick()
        {
            outCard++;
        }

        protected override void OnSelectCardAdd(Card card)
        {
            if (_selectedCardList.Count == 1)
            {
                GameplayResultManager.Instance.GameplayClickLogList[^1].ClickResult = GameplayClickResultEnum.UNMATCH;
                // Debug.Log(2);
                //Here
            }

            else
            {
                card.IndexClick = GameplayResultManager.Instance.GameplayClickLogList.Count - 1;
                //Here
            }

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
                // matchFalseCount++;
                if (firstMatchTime == 0f)
                {
                    firstMatchTime = addedTime ? 210 - UIManager.Instance.Timer : 180 - UIManager.Instance.Timer;
                }
                
                var phaseDataLength = _gameplayPhaseData.Count;
                var timeClock = addedTime ? 210 - UIManager.Instance.Timer : 180 - UIManager.Instance.Timer;
                var timeUsed = phaseDataLength == 0
                    ? timeClock
                    : timeClock - _gameplayPhaseData[phaseDataLength - 1].ClockTime;
                if (ccOpen)
                {
                    CardPhase[0]++;
                    PhaseData p = new PhaseData(PhaseEnum.IRM,timeClock, timeUsed);
                    _gameplayPhaseData.Add(p);
                }
                else if (!allCardOpen)
                {
                    CardPhase[1]++;
                    PhaseData p = new PhaseData(PhaseEnum.SPM,timeClock, timeUsed);
                    _gameplayPhaseData.Add(p);
                }
                else
                {
                    CardPhase[2]++;
                    PhaseData p = new PhaseData(PhaseEnum.ESM,timeClock, timeUsed);
                    _gameplayPhaseData.Add(p);
                }
                AllCardOpen();

                GameplayResultManager.Instance.GameplayClickLogList[_selectedCardList[1].IndexClick].ClickResult = GameplayClickResultEnum.MATCHED;
                // Debug.Log(3);
                //Here
                ShowMatchCount.Instance.OnMatch(_targetPairMatchCount - _remainPairMatchCount);
                _selectedCardList.ForEach(card => card.SelectedCorrect());
                _remainPairMatchCount--;

                ClearSelectCardList();

                if (_remainPairMatchCount <= 0)
                {
                    AudioController.StopPlayBGM();
                    _state = GameState.RESULT;
                    if (SequenceManager.Instance._ttr4_play)
                    {
                        ClearHint();
                        lastClick = -1f;
                        endPanel.SetActive(true);
                    }
                    else
                    {
                        disposable = GameplayUtils.CountDown(1.0f).ObserveOnMainThread().Subscribe(_ => { }, () =>
                                            {
                                                //SceneManager.LoadScene("Menu");
                                                var rp = 0;
                                                var oc = 0;
                                                ClearHint();
                                                lastClick = -1f;
                                                rewardPanel.SetActive(true);
                                                // I need to calculate here
                                                float score = 100.0f;
                                                int flower = 3;
                                                int click_count = 0;
                                                int a = (int)SequenceManager.Instance.GetSequenceDetail().GetGameplaySequenceSetting().pairType * 2;
                                                foreach (var clickc in GameplayResultManager.Instance.GameplayClickLogList)
                                                //Here
                                                {
                                                    if (clickc.ClickStatus == GameplayClickStatusEnum.ON_CARD)
                                                    {
                                                        click_count++;
                                                    }
                                                }
                                                score = ((float)a / (float)click_count) * 100.0f;
                                                if (score >= 70)
                                                {
                                                    flower = 3;
                                                }
                                                else if (score < 70 && score >= 30)
                                                {
                                                    flower = 2;
                                                }
                                                else
                                                {
                                                    flower = 1;
                                                }
                                                if (flipped || addedTime || passiveUsed)
                                                {
                                                    flower--;
                                                }
                                                if (flower <= 0)
                                                {
                                                    flower = 1;
                                                }
                                                rewardPanel.GetComponent<RewardManager>().SetScore(flower, (int)SequenceManager.Instance.GetSequenceDetail().GetGameplaySequenceSetting().pairType * (int)Mathf.Pow(2, flower - 1));
                                                SequenceManager.Instance.game_score = flower;
                                                foreach (var itemc in GameplayResultManager.Instance.GameplayClickLogList)
                                                //Here
                                                {
                                                    if (itemc.ClickResult == GameplayClickResultEnum.REPEAT)
                                                    {
                                                        rp++;
                                                    }
                                                    if (itemc.ClickStatus == GameplayClickStatusEnum.OUT_CARD)
                                                    {
                                                        oc++;
                                                    }
                                                }
                                                List<string> allCard = new List<string>();
                                                List<string> saveCard = new List<string>();

                                                foreach (var item in _cardList)
                                                {
                                                    if (!allCard.Contains(item.CardProperty.key))
                                                    {
                                                        allCard.Add(item.CardProperty.key);
                                                    }
                                                }
                                                int cardReq = (int)Mathf.Floor((int)GameplayResultManager.Instance.GamePlayResult.CardPair / 2);
                                                while (cardReq > 0)
                                                {
                                                    cardReq--;
                                                    int cardIndex = Random.Range(0, allCard.Count);
                                                    saveCard.Add(allCard[cardIndex]);
                                                    allCard.RemoveAt(cardIndex);
                                                }
                                                FirebaseManagerV2.Instance.SaveCard(SequenceManager.Instance.GetSequenceDetail().GetGameplaySequenceSetting().categoryTheme.ToString(), saveCard);
                                                //GameplayResult
                                                GameplayResultManager.Instance.GamePlayResult.TimeUsed = addedTime ? 210 - UIManager.Instance.Timer : 180 - UIManager.Instance.Timer;
                                                GameplayResultManager.Instance.GamePlayResult.ClickCount = clickCount;
                                                GameplayResultManager.Instance.GamePlayResult.MatchFalseCount = matchFalseCount;
                                                GameplayResultManager.Instance.GamePlayResult.CompletedAt = DateTime.Now;
                                                GameplayResultManager.Instance.GamePlayResult.OutareaCount = outCard;
                                                GameplayResultManager.Instance.GamePlayResult.RepeatCount = repeatCount;
                                                //FuzzyGameData
                                                //PrepareFuzzyData(true);
                                                PrepareQData(true);

                                                // GameplayResultManager.Instance.FuzzyGameResult.Helper = new List<bool>{addedTime,flipped,passiveUsed};

                                                GameplayResultManager.Instance.OnEndGame();

                                                disposable.Dispose();
                                            }).AddTo(this);
                    }

                }
                else
                {
                    //SoundManager.Instance.PlaySoundEffect(SoundType.CorrectMatch);
                }
            }
            else
            {
                AudioController.SetnPlay("audio/SFX/Wrong_Match");
                AllCardOpen();
                ccOpen = false;
                matchFalseCount++;
                // matchTotalCount++;
                GameplayResultManager.Instance.GameplayClickLogList[_selectedCardList[1].IndexClick].ClickResult = GameplayClickResultEnum.FALSE_MATCH;
                // Debug.Log(4);

                //Here
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

        public void AllCardOpen()
        {
            bool checker = true;
            foreach (var item in _cardList)
            {
                if (item.FlipOnce == false)
                {
                    checker = false;
                }
            }
            if (checker)
            {
                allCardOpen = true;
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
            GameplayResultManager.Instance.GameplayClickLogList[^1].ClickStatus = GameplayClickStatusEnum.OTHER;
            GameplayResultManager.Instance.GameplayClickLogList[^1].ClickResult = GameplayClickResultEnum.UNMATCH;
            // pauseImage.sprite = playSprite;
            Time.timeScale = 0;
            stopTime = Time.realtimeSinceStartup;
            pausePanel.SetActive(true);
            game_state = "PAUSE";
        }
        public void SettingGame()
        {
            GameplayResultManager.Instance.GameplayClickLogList[^1].ClickStatus = GameplayClickStatusEnum.OTHER;
            GameplayResultManager.Instance.GameplayClickLogList[^1].ClickResult = GameplayClickResultEnum.REPEAT;
            Time.timeScale = 0;
            stopTime = Time.realtimeSinceStartup;
            game_state = "SETTING";
        }
        public void StartTutorial4()
        {
            helper.SetActive(true);
            disposable = GameplayUtils.CountDown(1.0f).ObserveOnMainThread().Subscribe(_ => { }, () =>
                                    {
                                        helper.transform.GetChild(0).gameObject.SetActive(true);
                                        helper.transform.GetChild(6).gameObject.SetActive(false);
                                        Time.timeScale = 0;
                                    }).AddTo(this);


        }
        public void Tutorial4NextStep()
        {
            if (ttr4_state == 0)//press next
            {
                helper.transform.GetChild(5).gameObject.SetActive(true);
                helpPanel.transform.parent = helper.transform.GetChild(5);
                flipCard.interactable = false;
                helper.transform.GetChild(7).gameObject.SetActive(true);
                helper.transform.GetChild(7).GetChild(0).gameObject.SetActive(true);
            }
            else if (ttr4_state == 1) //press time
            {
                helper.transform.GetChild(1).gameObject.SetActive(false);
                helper.transform.GetChild(3).gameObject.SetActive(true);
                helper.transform.GetChild(6).gameObject.SetActive(true);
                helper.transform.GetChild(7).GetChild(0).gameObject.SetActive(false);
                flipCard.interactable = true;
                disposable = GameplayUtils.CountDown(2.0f).ObserveOnMainThread().Subscribe(_ => { }, () =>
                                    {
                                        helper.transform.GetChild(3).gameObject.SetActive(false);
                                        helper.transform.GetChild(6).gameObject.SetActive(false);
                                        helper.transform.GetChild(2).gameObject.SetActive(true);
                                        helper.transform.GetChild(7).GetChild(1).gameObject.SetActive(true);
                                        disposable.Dispose();
                                    }).AddTo(this);
            }

            else if (ttr4_state == 2)
            { //press flip
                helpPanel.transform.parent = rightPanel.transform;
                helper.transform.GetChild(2).gameObject.SetActive(false);
                helper.transform.GetChild(6).gameObject.SetActive(true);
                helper.transform.GetChild(5).gameObject.SetActive(false);
                helper.transform.GetChild(7).gameObject.SetActive(false);

                Time.timeScale = 1;
                disposable = GameplayUtils.CountDown(6.0f).ObserveOnMainThread().Subscribe(_ => { }, () =>
                                    {
                                        Time.timeScale = 0;
                                        helper.transform.GetChild(3).gameObject.SetActive(false);
                                        helper.transform.GetChild(4).gameObject.SetActive(true);
                                        helper.transform.GetChild(6).gameObject.SetActive(false);
                                        disposable.Dispose();

                                    }).AddTo(this);
            }
            else if (ttr4_state == 3)
            {
                Time.timeScale = 1;
                SequenceManager.Instance._ttr4_play = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            ttr4_state++;
        }

        public void ReTTR4()
        {
            SequenceManager.Instance._ttr4_play = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        public void EndTTR4()
        {
            SequenceManager.Instance._ttr4_play = false;
            SequenceManager.Instance._ttr4 = false;
            FirebaseManagerV2.Instance.SaveTutorialUserGameData("4", true);
            SequenceManager.Instance.NextSequence();
        }

        public void Resume()
        {
            if (game_state == "PAUSE")
            {
                GameplayResultManager.Instance.GameplayClickLogList[^1].ClickStatus = GameplayClickStatusEnum.OTHER;
                GameplayResultManager.Instance.GameplayClickLogList[^1].ClickResult = GameplayClickResultEnum.UNMATCH;
            }
            else
            {
                GameplayResultManager.Instance.GameplayClickLogList[^1].ClickStatus = GameplayClickStatusEnum.OTHER;
                GameplayResultManager.Instance.GameplayClickLogList[^1].ClickResult = GameplayClickResultEnum.REPEAT;
            }
            game_state = "";

            pauseImage.sprite = pauseSprite;
            Time.timeScale = 1;
            PauseLog pauseLog = new PauseLog(addedTime ? (210.0f - UIManager.Instance.GetTimer()).ToString() : (180.0f - UIManager.Instance.GetTimer()).ToString(), (Time.realtimeSinceStartup - stopTime).ToString());
            GameplayResultManager.Instance.GamePlayResult.PauseLogList.Add(pauseLog);
            GameplayResultManager.Instance.FuzzyGameResult.PauseUsed = true;
            GameplayResultManager.Instance.QLogResult.PauseUsed = true;
            pausePanel.SetActive(false);
        }
        public void AddTime()
        {
            if (ttr4_state == 1)
            {
                Tutorial4NextStep();
            }
            GameplayResultManager.Instance.GameplayClickLogList[^1].ClickStatus = GameplayClickStatusEnum.OTHER;
            GameplayResultManager.Instance.GameplayClickLogList[^1].ClickResult = GameplayClickResultEnum.MATCHED;
            GameplayResultManager.Instance.GamePlayResult.AddTimeUsed = 180.0f - UIManager.Instance.GetTimer();
            UIManager.Instance.AddTime(30.0f);
            addTime.interactable = false;
            addedTime = true;
            HelperSeq.Add("Time");


        }
        public void FlipAll()
        {
            if (ttr4_state == 2)
            {
                Tutorial4NextStep();
            }
            foreach(var item in _cardList){
                if(item.IsFliping && item.IsInComplete()){
                    return;
                }
            }
            if(_selectedCardList.Count!=0){
                return;
            }
            GameplayResultManager.Instance.GameplayClickLogList[^1].ClickStatus = GameplayClickStatusEnum.OTHER;
            GameplayResultManager.Instance.GameplayClickLogList[^1].ClickResult = GameplayClickResultEnum.FALSE_MATCH;
            flipped = true;
            HelperSeq.Add("Flip");
            lastClick = -1f;
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

                yield return new WaitForSecondsRealtime(5);// Wait a bit
                foreach (var item in _cardList)
                {
                    if (item.IsInComplete())
                    {
                        item.FlipCard(CardState.FACE_DOWN);
                    }

                }
                yield return new WaitForSecondsRealtime(0.3f);
                disableArea.SetActive(false);
                lastClick = Time.time;
                UIManager.Instance.freezeTimer = false;
            }
            if (addedTime)
            {
                GameplayResultManager.Instance.GamePlayResult.FlipAllUsed = 210.0f - UIManager.Instance.GetTimer();
            }
            else
            {
                GameplayResultManager.Instance.GamePlayResult.FlipAllUsed = 180.0f - UIManager.Instance.GetTimer();
            }
            UIManager.Instance.freezeTimer = true;
            StartCoroutine(startFlip());

        }
        public void StartPassive()
        {
            lastClick = Time.time;
        }
        public void TriggerPassive()
        {
            if (_selectedCardList.Count > 0)
            {
                foreach (var item in _cardList)
                {

                    if (item.CardProperty.key == _selectedCardList[0].CardProperty.key)
                    {
                        _hintCardList.Add(item);
                    }
                }
            }
            else
            {
                var a = Random.Range(0, keyContain.Count);
                while (keyContain[a] == "")
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

            }
            _hintCardList[0].StartFading();
            _hintCardList[1].StartFading();
            startHintTime = Time.time;
            if (!FirebaseManagerV2.Instance.gameData["PASSIVE"])
            {
                inTutorialState = true;
                dimBackground.SetActive(true);
                if (GameplayResultManager.Instance.GamePlayResult.CardPatternLayout == GameLayout.GRID)
                {
                    gridObject.GetComponent<GridLayoutGroup>().enabled = false;
                }

                tutorialIndex[0] = _hintCardList[0].transform.GetSiblingIndex();
                tutorialIndex[1] = _hintCardList[1].transform.GetSiblingIndex();
                _hintCardList[0].transform.SetParent(dimBackground.transform);
                _hintCardList[1].transform.SetParent(dimBackground.transform);
            }


        }
        public void ClearHint()
        {
            if (_hintCardList.Count == 2)
            {
                if (addedTime)
                {

                }
                PassiveLog passiveLog = new PassiveLog(startHintTime - referenceTime, Time.time - referenceTime, Time.time - startHintTime, _hintCardList[0].CardProperty.key);
                GameplayResultManager.Instance.GamePlayResult.PassiveLogList.Add(passiveLog);
                foreach (var item in _hintCardList)
                {
                    item.StopFading();
                }
                _hintCardList.Clear();
            }
            if (GameplayResultManager.Instance.GamePlayResult.CardPatternLayout == GameLayout.GRID)
            {
                gridObject.GetComponent<GridLayoutGroup>().enabled = true;
            }
            dimBackground.SetActive(false);
            return;

        }
        public void EndGame()
        {
            if (SequenceManager.Instance._ttr4_play)
            {
                ClearHint();
                lastClick = -1f;
                endPanel.SetActive(true);
            }
            else
            {
                if (_hintCardList.Count > 0)
                {
                    if (GameplayResultManager.Instance.GamePlayResult.CardPatternLayout == GameLayout.GRID)
                    {
                        _hintCardList[0].transform.SetParent(gridObject.transform);
                        _hintCardList[0].transform.SetSiblingIndex(tutorialIndex[0]);
                        _hintCardList[1].transform.SetParent(gridObject.transform);
                        _hintCardList[1].transform.SetSiblingIndex(tutorialIndex[1]);
                        gridObject.GetComponent<GridLayoutGroup>().enabled = true;
                    }
                    else
                    {
                        _hintCardList[0].transform.SetParent(randObject.transform);
                        _hintCardList[0].transform.SetSiblingIndex(tutorialIndex[0]);
                        _hintCardList[1].transform.SetParent(randObject.transform);
                        _hintCardList[1].transform.SetSiblingIndex(tutorialIndex[1]);
                    }
                    dimBackground.SetActive(false);
                }
                ClearHint();
                lastClick = -1f;
                AudioController.StopPlayBGM();
                _state = GameState.RESULT;
                disposable = GameplayUtils.CountDown(0.1f).ObserveOnMainThread().Subscribe(_ => { }, () =>
                {
                    //SceneManager.LoadScene("Menu");
                    rewardPanel.SetActive(true);
                    rewardPanel.GetComponent<RewardManager>().SetScore(1, 4);
                    SequenceManager.Instance.game_score = 0;
                    //GameplayResultData
                    GameplayResultManager.Instance.GamePlayResult.TimeUsed = addedTime ? 210 - UIManager.Instance.Timer : 180 - UIManager.Instance.Timer;
                    GameplayResultManager.Instance.GamePlayResult.ClickCount = clickCount;
                    GameplayResultManager.Instance.GamePlayResult.MatchFalseCount = matchFalseCount;
                    GameplayResultManager.Instance.GamePlayResult.CompletedAt = DateTime.Now;
                    //FuzzyGameData
                    //PrepareFuzzyData(false);
                    PrepareQData(false);
                    
                    GameplayResultManager.Instance.OnEndGame();
                    disposable.Dispose();

                }).AddTo(this);
            }
            //timeout

        }
        public void EnableTools()
        {
            addTime.interactable = true;
            flipCard.interactable = true;
            pauseGame.interactable = true;
            settingPage.interactable = true;
        }

        // public void PrepareFuzzyData(bool IsGameComplete)
        // {
        //     GameplayResultManager.Instance.FuzzyGameResult.GameID = FuzzyBrain.Instance.gameCount.ToString();
        //     GameplayResultManager.Instance.FuzzyGameResult.Phase = CardPhase;
        //     GameplayResultManager.Instance.FuzzyGameResult.TimeUsed = addedTime ? 210 - UIManager.Instance.Timer : 180 - UIManager.Instance.Timer;
        //     GameplayResultManager.Instance.FuzzyGameResult.Complete = IsGameComplete;
        //     GameplayResultManager.Instance.FuzzyGameResult.Helper = new List<bool> { addedTime, flipped, passiveUsed };
        //     GameplayResultManager.Instance.FuzzyGameResult.HelperSeq = this.HelperSeq;
        //     GameplayResultManager.Instance.FuzzyGameResult.FalseMatch = matchFalseCount;
        //     GameplayResultManager.Instance.FuzzyGameResult.TotalMatch = (int)_cardList.Count;
        //     GameplayResultManager.Instance.FuzzyGameResult.FirstMatchTime = firstMatchTime;
        //     var (gm, cl, cd) = FuzzyBrain.Instance.DLS.GetLevelData();
        //     GameplayResultManager.Instance.FuzzyGameResult.Difficulty = cd;
        //     GameplayResultManager.Instance.FuzzyGameResult.GridMode = gm;
        //     GameplayResultManager.Instance.FuzzyGameResult.GameLevel = cl;
        // }
        
        public void PrepareQData(bool IsGameComplete)
        {
            GameplayResultManager.Instance.QLogResult.GameID = QBrain.Instance.gameCount.ToString();
            GameplayResultManager.Instance.QLogResult.Phase = CardPhase;
            GameplayResultManager.Instance.QLogResult.TimeUsed = addedTime ? 210 - UIManager.Instance.Timer : 180 - UIManager.Instance.Timer;
            GameplayResultManager.Instance.QLogResult.Complete = IsGameComplete;
            GameplayResultManager.Instance.QLogResult.Helper = new List<bool> { addedTime, flipped, passiveUsed };
            GameplayResultManager.Instance.QLogResult.HelperSeq = this.HelperSeq;
            GameplayResultManager.Instance.QLogResult.FalseMatch = matchFalseCount;
            GameplayResultManager.Instance.QLogResult.TotalMatch = (int)_cardList.Count;
            GameplayResultManager.Instance.QLogResult.FirstMatchTime = firstMatchTime;
            GameplayResultManager.Instance.QLogResult.PhaseDataList = _gameplayPhaseData;
            GameplayResultManager.Instance.QLogResult.ClickCount = clickCount;
            var (gl, di, gm) = QBrain.Instance.GetLevelData();
            GameplayResultManager.Instance.QLogResult.Difficulty = di;
            GameplayResultManager.Instance.QLogResult.GridMode = gm;
            GameplayResultManager.Instance.QLogResult.GameLevel = gl;
        } 
    }
}