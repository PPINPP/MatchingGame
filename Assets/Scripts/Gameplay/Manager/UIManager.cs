using System;
using System.Collections;
using System.Collections.Generic;
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

        public UnityAction OnTime;

        [Header("Mock Countdown")]
        [SerializeField] List<GameObject> countdownObjList;
        [SerializeField] GameObject CountDownGroup;
        private int countIndex;


        private float targetAlphaBG;
        private float timer;
        private float durationLerp;

        public void Init()
        {
            timer = 0;
            countIndex = 0;
            targetAlphaBG = bgDimImg.color.a;
            Color color = bgDimImg.color;
            color.a = 0;
            bgDimImg.color = color;
            canvasGroup.alpha = 0;
            playArea.SetActive(false);
            CountDownGroup.SetActive(false);
        }

        public void StartCountDown()
        {
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
        }

        private void Update()
        {
            if (GameManager.Instance.State == GameState.FADE_IN_UI && timer <= countdownStartDuration)
            {
                durationLerp += Time.deltaTime;

                canvasGroup.alpha = Mathf.Lerp(0,1,durationLerp/countdownStartDuration);
                Color color = bgDimImg.color;
                color.a = Mathf.Lerp(0, targetAlphaBG, durationLerp / countdownStartDuration);
                bgDimImg.color = color;
            }
        }
    }
}