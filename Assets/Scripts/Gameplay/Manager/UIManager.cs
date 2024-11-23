using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using Unity.VisualScripting;
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
        [SerializeField] TMP_Text timerTxtB;
        [SerializeField] GameObject timerGameObject;
        [SerializeField] LevelManager levelManager;

        [SerializeField] Material timerMaterial;

        public UnityAction OnTime;

        [Header("Mock Countdown")]
        [SerializeField] List<GameObject> countdownObjList;
        [SerializeField] GameObject CountDownGroup;
        private int countIndex;


        private float targetAlphaBG;
        private float timer;
        public bool freezeTimer = false;
        private float durationLerp;
        public bool isFadeInCountDown;
        public bool isFadeInComplete;

        public float Timer { get => timer; set => timer = value; }

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
            timerMaterial.SetColor("_OutlineColor", new Color(1.0f, 0f, 0f));
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
                    var sequence = SequenceManager.Instance.GetSequenceDetail();
                    var sequenceSetting = sequence.GetGameplaySequenceSetting();
                    if (!sequenceSetting.isTutorial)
                    {
                        levelManager.EnableTools();
                    }


                    timerTxt.text = "";
                    OnTime?.Invoke();
                }
            }).AddTo(this);

            OnTime += () =>
            {
                timer = 180;
                var sequence = SequenceManager.Instance.GetSequenceDetail();
                var sequenceSetting = sequence.GetGameplaySequenceSetting();
                if (!sequenceSetting.isTutorial)
                {
                    levelManager.StartPassive();
                }

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
                    if (!freezeTimer)
                    {
                        timer -= Time.deltaTime;
                        if (timer <= 0)
                        {
                            var sequence = SequenceManager.Instance.GetSequenceDetail();
                            var sequenceSetting = sequence.GetGameplaySequenceSetting();
                            if (!sequenceSetting.isTutorial)
                            {
                                levelManager.EndGame();
                            }

                            print("end");

                        }
                        else
                        {
                            timerTxt.text = $"{Mathf.Floor(timer)}";
                        }
                        float r = (210.0f - timer) * 1.67f;
                        if (r >= 150.0f)
                        {
                            r = 150.0f;
                        }
                        float g = timer * 1.67f;
                        if (g >= 150.0f)
                        {
                            g = 150.0f;
                        }
                        Color ncolor = new Color(r / 255.0f, g / 255.0f, 0.0f);

                        timerGameObject.GetComponent<TMP_Text>().color = ncolor;
                        // timerGameObject.GetComponent<TMP_Text>().outlineColor = ncolor;
                        timerMaterial.SetColor("_OutlineColor", ncolor);
                        // timerTxtB.color = ncolor;
                        // timerTxtB.outlineColor = ncolor;
                        // timerTxt.outlineColor = ncolor;
                        // timerTxt.color = new Color(r/255f,g/255f,0f);
                        // timerTxt.color = new Color(r/255f,g/255f,0f);
                    }

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

        public void AddTime(float sec)
        {
            timer += sec;
        }

        public float GetTimer()
        {
            return timer;
        }
    }
}