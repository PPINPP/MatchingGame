using MatchingGame.Gameplay;
using System;
using System.Collections.Generic;
using Model;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MinigameManager : MonoInstance<MinigameManager>
{
    [SerializeField] Image clickObjImg;
    [SerializeField] List<Sprite> popupSprites = new List<Sprite>();
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
    private int spawnCounter;
    private bool isRoundActive = false;
    private IDisposable disposable;
    private float timer = 0.0f;
    private bool isStartGame = false;
    private int object_type;
    private int curr_obj;
    private int curr_index = 0;
    private List<int> sequenceObj = new List<int>();


    public override void Init()
    {
        base.Init();

        minX = offset;
        minY = offset;
        maxX = Screen.width - offset;
        maxY = Screen.height - offset;
        GameplayResultManager.Instance.MinigameResult.ScreenHeight = Screen.height;
        GameplayResultManager.Instance.MinigameResult.ScreenWidth = Screen.width;
        clickObjImg.gameObject.SetActive(false);
        finishUIObj.SetActive(false);
        object_type = UnityEngine.Random.Range(0, 4);

        for (int i = 0; i < 10; i++)
        {
            sequenceObj.Add(0);
        }
        for (int i = 0; i < 5; i++)
        {
            var correct_order = UnityEngine.Random.Range(0, 10);
            while (sequenceObj[correct_order] == 1)
            {
                correct_order = (correct_order + 1) % 10;
            }
            sequenceObj[correct_order] = 1;
        }
        foreach(var item in sequenceObj){
            Debug.Log(item);
        }
        popupStartGameObj.transform.GetChild(1).GetComponent<Image>().sprite = popupSprites[object_type];
        popupStartGameObj.SetActive(true);
        GameplayResultManager.Instance.MinigameResult.ObjectType = object_type;
    }

    public void StartGame()
    {
        isStartGame = true;
        popupStartGameObj.SetActive(false);
        CountDown();
    }

    private void Update()
    {
        if (isStartGame)
        {
            timer += Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                GameplayResultManager.Instance.MinigameClickLogList.Add(new MinigameClickLog(Input.mousePosition.x, Input.mousePosition.y, timer
                    , isRoundActive ? MinigameClickStatusEnum.FALSE : MinigameClickStatusEnum.LATE)); ;
            }
        }
    }

    void CountDown()
    {
        disposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            isRoundActive = !isRoundActive;
            if (isRoundActive)
            {
                // RandomImg();
                NextImg();
                RandomPosition();
                timer = 0;
                SoundManager.Instance.PlaySoundEffect(SoundType.Spawn);
            }
            else
            {
                GameplayResultManager.Instance.MinigameResult.TimeUsed.Add(0);
            }
            clickObjImg.gameObject.SetActive(isRoundActive);

            if (spawnCounter >= spawnTime)
                onComplete?.Invoke();

            if (isRoundActive)
                spawnCounter++;
        }).AddTo(this);

        onComplete = () =>
        {
            isStartGame = false;
            disposable.Dispose();
            clickObjImg.gameObject.SetActive(false);
            finishUIObj.SetActive(true);

            GameplayResultManager.Instance.MinigameResult.CompletedAt = DateTime.Now;
            GameplayResultManager.Instance.OnEndMiniGame();
            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ => { }, () =>
            {
                SequenceManager.Instance.NextSequence();
            });
        };
    }

    public void RandomImg()
    {
        curr_obj = UnityEngine.Random.Range(0, objSprites.Count);
        clickObjImg.sprite = objSprites[curr_obj];
        GameplayResultManager.Instance.MinigameResult.RandomObject.Add(curr_obj);
    }
    public void NextImg()
    {
        if (sequenceObj[curr_index] == 1)
        {
            clickObjImg.sprite = objSprites[object_type];
        }
        else{
            do{
                curr_obj = UnityEngine.Random.Range(0, objSprites.Count);
            }while(curr_obj == object_type);
            clickObjImg.sprite = objSprites[curr_obj];
        }
        curr_index++;
    }

    public void RandomPosition()
    {
        var posX = UnityEngine.Random.Range(minX, maxX);
        var posY = UnityEngine.Random.Range(minY, maxY);
        GameplayResultManager.Instance.MinigameResult.TargetPosX.Add(posX);
        GameplayResultManager.Instance.MinigameResult.TargetPosY.Add(posY);

        RectTransform rectTransform = clickObjImg.GetComponent<RectTransform>();
        rectTransform.position = new Vector3(posX, posY, 0);
    }

    public void OnClick()
    {
        GameplayResultManager.Instance.MinigameResult.TimeUsed.Add(timer);
        GameplayResultManager.Instance.MinigameClickLogList[^1].ClickStatus = MinigameClickStatusEnum.NORMAL;
        GameplayResultManager.Instance.MinigameClickLogList[^1].isCorrect = object_type == curr_obj ? true : false;
        clickObjImg.gameObject.SetActive(false);
        isRoundActive = false;
        disposable.Dispose();
        CountDown();
    }
}
