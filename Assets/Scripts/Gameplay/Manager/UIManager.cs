using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MatchingGame.Gameplay
{
    public class UIManager : MonoInstance<UIManager>
    {
        [SerializeField] GameObject playArea;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] Image bgDimImg;
        [SerializeField] float countdownStartDuration;
        [SerializeField] TextMeshProUGUI timerTxt;

        public UnityAction OnTime;

        [Header("Mock Countdown")]
        [SerializeField] List<GameObject> countdownObjList;
        [SerializeField] GameObject CountDownGroup;
        private int countIndex;


        private float targetAlphaBG;
        private float timer;
        private float durationLerp;
        private bool isFadeInCountDown;
        private bool isFadeInComplete;

        public float Timer { get =>  timer; set => timer = value; } 

        protected override void Awake()
        {
            targetAlphaBG = bgDimImg.color.a;
            base.Awake();
        }

        public override void Init()
        {
            timer = 0;
            countIndex = 0;
            SetDisableUI();
            CountDownGroup.SetActive(false);
        }

        public void BeginCountDownStartGame(bool isFade = false) 
        {
            AudioController.SetnPlay("audio/SFX/Countdown");
            CountDownGroup.SetActive(true);
            IDisposable disposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                timer++;
                countdownObjList[countIndex].SetActive(false);
                countIndex++;
                if (countIndex < countdownObjList.Count)
                    countdownObjList[countIndex].SetActive(true);

                if (timer >= countdownStartDuration)
                {
                    OnTime?.Invoke();
                }
            }).AddTo(this);

            OnTime += () =>
            {
                disposable.Dispose();
                playArea.SetActive(true);
                CountDownGroup.SetActive(false);
                canvasGroup.alpha = 1;
                
            };
            isFadeInCountDown = isFade;
        }

        public void BeginCountDownShowCard()
        {
            
            timer = GameplayResources.Instance.GameplayProperty.FirstTimeShowDuration;
            timerTxt.text = $"{timer}";

            IDisposable disposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                timer--;
                timerTxt.text = $"{timer}";

                if (timer <= 0)
                {
                    AudioController.SetnPlayBGM("audio/BGM/BGM");
                    timerTxt.text = "";
                    OnTime?.Invoke();
                }
            }).AddTo(this);

            OnTime += () =>
            {
                timer = 0;
                disposable.Dispose();
            };
        }

        private void Update()
        {
            if (!isFadeInComplete && isFadeInCountDown)
            {
                if (GameManager.Instance.State == GameState.FADE_IN_UI && timer <= countdownStartDuration)
                {
                    durationLerp += Time.deltaTime;

                    canvasGroup.alpha = Mathf.Lerp(0, 1, durationLerp / countdownStartDuration);
                    Color color = bgDimImg.color;
                    color.a = Mathf.Lerp(0, targetAlphaBG, durationLerp / countdownStartDuration);
                    bgDimImg.color = color;

                    if (durationLerp > countdownStartDuration)
                        isFadeInComplete = true;
                }
            }
            else
            {
                if (GameManager.Instance.State == GameState.PLAYING)
                {
                    timer += Time.deltaTime;

                    timerTxt.text = $"{Mathf.Floor(timer)}";
                }
            }
        }

        public void SetEnableUI()
        {
            Color color = bgDimImg.color;
            color.a = targetAlphaBG;
            bgDimImg.color = color;
            canvasGroup.alpha = 1;
            playArea.SetActive(true);
        }

        public void SetDisableUI()
        {
            Color color = bgDimImg.color;
            color.a = 0;
            bgDimImg.color = color;
            canvasGroup.alpha = 0;
            timerTxt.text = "";
            playArea.SetActive(false);
        }
    }
}