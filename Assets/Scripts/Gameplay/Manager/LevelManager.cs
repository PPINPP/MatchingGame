using Model;
using System;
using System.Collections.Generic;
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

        private int clickCount = 0;
        private int matchFalseCount = 0;
        private bool successInit = false;
        private float stopTime = 0f;
        private float lastClick = -1f;
        private float referenceTime = 0f;
        private float startHintTime = 0f;
        private bool addedTime = false;
        private bool flipped = false;
        private int outCard = 0;
        private int repeatCount = 0;
        private bool inTutorialState = false;
        private int[] tutorialIndex = new int[2];


        private List<string> keyContain = new List<string>();

        protected override void Start()
        {
            base.Start();
            settingPanel.SetActive(false);
            rewardPanel.SetActive(false);
            playArea.onClick.AddListener(OnPlayAreaClick);
            //TODO: Follow Sequence To Setting
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
            }
            if (inTutorialState)
            {
                if (!_hintCardList[0].IsInComplete() && !_hintCardList[1].IsInComplete())
                {
                    inTutorialState = false;
                    lastClick = Time.time;
                    _hintCardList[0].transform.SetParent(gridObject.transform);
                    _hintCardList[0].transform.SetSiblingIndex(tutorialIndex[0]);
                    _hintCardList[1].transform.SetParent(gridObject.transform);
                    _hintCardList[1].transform.SetSiblingIndex(tutorialIndex[1]);
                    gridObject.GetComponent<GridLayoutGroup>().enabled = true;
                    dimBackground.SetActive(false);
                    FirebaseManagerV2.Instance.gameData["PASSIVE"] = true;
                    FirebaseManagerV2.Instance.SetTutorial(true);
                    ClearHint();
                }
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
            GameplayResultManager.Instance.GameplayClickLogList[^1].ClickStatus = GameplayClickStatusEnum.ON_CARD;
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
            }

            else
            {
                card.IndexClick = GameplayResultManager.Instance.GameplayClickLogList.Count - 1;
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
                GameplayResultManager.Instance.GameplayClickLogList[_selectedCardList[1].IndexClick].ClickResult = GameplayClickResultEnum.MATCHED;
                ShowMatchCount.Instance.OnMatch(_targetPairMatchCount - _remainPairMatchCount);
                _selectedCardList.ForEach(card => card.SelectedCorrect());
                _remainPairMatchCount--;

                ClearSelectCardList();

                if (_remainPairMatchCount <= 0)
                {
                    AudioController.StopPlayBGM();
                    _state = GameState.RESULT;
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
                        {
                            if (clickc.ClickStatus == GameplayClickStatusEnum.ON_CARD)
                            {
                                click_count++;
                            }
                        }
                        score = ((float)a/(float)click_count)*100.0f;
                        if(score>=70){
                            flower = 3;
                        }
                        else if(score <70 && score>=30){
                            flower = 2;
                        }
                        else{
                            flower = 1;
                        }
                        if (flipped || addedTime)
                        {
                            flower--;
                        }
                        if(flower <=0){
                            flower = 1;
                        }
                        rewardPanel.GetComponent<RewardManager>().SetScore(flower, 4*(int)Mathf.Pow(2,flower-1));
                        SequenceManager.Instance.game_score = flower;
                        foreach (var itemc in GameplayResultManager.Instance.GameplayClickLogList)
                        {
                            if (itemc.ClickResult == GameplayClickResultEnum.REPEAT)
                            {
                                rp++;
                            }
                            if (itemc.ClickStatus == GameplayClickStatusEnum.OUT_CARD)
                            {
                                oc++;
                            }
                            Debug.Log(rp);
                            Debug.Log(oc);
                        }
                        List<string> allCard = new List<string>();
                        List<string> saveCard = new List<string>();

                        foreach (var item in _cardList)
                        {
                            Debug.Log(item.CardProperty.key);
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
                        foreach (var item in saveCard)
                        {
                            Debug.Log(item);
                        }

                        FirebaseManagerV2.Instance.SaveCard(SequenceManager.Instance.GetSequenceDetail().GetGameplaySequenceSetting().categoryTheme.ToString(), saveCard);
                        GameplayResultManager.Instance.GamePlayResult.TimeUsed = addedTime ? 210 - UIManager.Instance.Timer : 180 - UIManager.Instance.Timer;
                        GameplayResultManager.Instance.GamePlayResult.ClickCount = clickCount;
                        GameplayResultManager.Instance.GamePlayResult.MatchFalseCount = matchFalseCount;
                        GameplayResultManager.Instance.GamePlayResult.CompletedAt = DateTime.Now;
                        GameplayResultManager.Instance.GamePlayResult.OutareaCount = outCard;
                        GameplayResultManager.Instance.GamePlayResult.RepeatCount = repeatCount;
                        GameplayResultManager.Instance.OnEndGame();

                        disposable.Dispose();
                    }).AddTo(this);
                }
                else
                {
                    //SoundManager.Instance.PlaySoundEffect(SoundType.CorrectMatch);
                }
            }
            else
            {
                AudioController.SetnPlay("audio/SFX/Wrong_Match");
                matchFalseCount++;
                //SoundManager.Instance.PlaySoundEffect(SoundType.WrongMatch);
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
        public void SettingGame()
        {
            Time.timeScale = 0;
            stopTime = Time.realtimeSinceStartup;
        }

        public void Resume()
        {
            pauseImage.sprite = pauseSprite;
            Time.timeScale = 1;
            PauseLog pauseLog = new PauseLog(addedTime ? (210.0f - UIManager.Instance.GetTimer()).ToString() : (180.0f - UIManager.Instance.GetTimer()).ToString(), (Time.realtimeSinceStartup - stopTime).ToString());
            GameplayResultManager.Instance.GamePlayResult.PauseLogList.Add(pauseLog);
            pausePanel.SetActive(false);
        }
        public void AddTime()
        {
            GameplayResultManager.Instance.GamePlayResult.AddTimeUsed = 180.0f - UIManager.Instance.GetTimer();
            UIManager.Instance.AddTime(30.0f);
            addTime.interactable = false;
            addedTime = true;


        }
        public void FlipAll()
        {
            flipped = true;
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
                    Debug.Log(item.CardProperty.key);
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
                gridObject.GetComponent<GridLayoutGroup>().enabled = false;

                tutorialIndex[0] = _hintCardList[0].transform.GetSiblingIndex();
                tutorialIndex[1] = _hintCardList[1].transform.GetSiblingIndex();
                Debug.Log(tutorialIndex[0]);
                Debug.Log(tutorialIndex[1]);
                _hintCardList[0].transform.SetParent(dimBackground.transform);
                _hintCardList[1].transform.SetParent(dimBackground.transform);
                //create dim
                //random new one
                //
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
            gridObject.GetComponent<GridLayoutGroup>().enabled = true;
            dimBackground.SetActive(false);
            return;

        }
        public void EndGame()
        {
            //timeout
            ClearHint();
            lastClick = -1f;
            AudioController.StopPlayBGM();
            _state = GameState.RESULT;
            disposable = GameplayUtils.CountDown(0.1f).ObserveOnMainThread().Subscribe(_ => { }, () =>
            {
                //SceneManager.LoadScene("Menu");
                rewardPanel.SetActive(true);
                rewardPanel.GetComponent<RewardManager>().SetScore(1, 4);
                GameplayResultManager.Instance.GamePlayResult.TimeUsed = addedTime ? 210 - UIManager.Instance.Timer : 180 - UIManager.Instance.Timer;
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
            settingPage.interactable = true;
        }
    }
}