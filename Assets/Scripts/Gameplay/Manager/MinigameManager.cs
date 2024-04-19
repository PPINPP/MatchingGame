using MatchingGame.Gameplay;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MinigameManager : MonoInstance<MinigameManager>
{
    [SerializeField] Image clickObjImg;
    [SerializeField] GameObject popupStartGameObj;
    [SerializeField] GameObject finishUIObj;
    [SerializeField] List<Sprite> objSprites = new List<Sprite>();
    [SerializeField] float offset;
    [SerializeField] int spawnTime;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private UnityAction onComplete;
    private int counter;
    private bool isRoundActive = false;
    private IDisposable disposable;
    public override void Init()
    {
        base.Init();

        minX = offset;
        minY = offset;
        maxX = Screen.width - offset;
        maxY = Screen.height - offset;
        clickObjImg.gameObject.SetActive(false);
        finishUIObj.SetActive(false);

        popupStartGameObj.SetActive(true);
    }

    public void StartGame()
    {
        popupStartGameObj.SetActive(false);
        CountDown();
    }

    void CountDown()
    {
        disposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            Randomimg();
            RandomPosition();
            isRoundActive = !isRoundActive;
            clickObjImg.gameObject.SetActive(isRoundActive);

            if (counter >= spawnTime)
                onComplete?.Invoke();

            if (isRoundActive)
                counter++;
        }).AddTo(this);

        onComplete = () =>
        {
            clickObjImg.gameObject.SetActive(false);
            disposable.Dispose();

            finishUIObj.SetActive(true);
            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ => { }, () =>
            {
                SequenceManager.Instance.NextSequence();
            });
        };
    }

    public void Randomimg()
    {
        var index = UnityEngine.Random.Range(0, objSprites.Count);
        clickObjImg.sprite = objSprites[index];
    }

    public void RandomPosition()
    {
        var posX = UnityEngine.Random.Range(minX, maxX);
        var posY = UnityEngine.Random.Range(minY, maxY);

        RectTransform rectTransform = clickObjImg.GetComponent<RectTransform>();
        rectTransform.position = new Vector3(posX,posY,0);
    }

    public void OnClick()
    {
        clickObjImg.gameObject.SetActive(false);
        isRoundActive = false;
        disposable.Dispose();
        CountDown();
    }
}
