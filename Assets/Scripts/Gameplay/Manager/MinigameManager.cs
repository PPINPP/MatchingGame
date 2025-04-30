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
    [SerializeField] Image clickObjLateImg;
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
    private float startTime = 0f;
    private int object_type;
    private int curr_obj;
    private int curr_index = 0;
    private List<int> sequenceObj = new List<int>();
    private float timeLate = 0.0f;
    private List<int> game_score = new List<int>();
    private List<float> time_use = new List<float>();
    private List<int> click_type_list = new List<int>(){0,0,0,0};


    public override void Init()
    {
        base.Init();
        var a = (Screen.height / 9.0f) * 16.0f;
        if (Screen.width - ((Screen.height / 9.0f) * 16.0f) >= 10.0f)
        {
            minX = offset + (Screen.width - ((Screen.height / 9.0f) * 16.0f));
            minY = offset;
            maxX = Screen.width - (offset + (Screen.width - ((Screen.height / 9.0f) * 16.0f)));
            maxY = Screen.height - offset;
        }
        else if (Screen.width - ((Screen.height / 9.0f) * 16.0f) <= 10.0f)
        {
            minX = offset;
            minY = offset + (Screen.height - ((Screen.width / 16.0f) * 9.0f));
            maxX = Screen.width - offset;
            maxY = Screen.height - (offset + (Screen.height - ((Screen.width / 16.0f) * 9.0f)));
        }
        else
        {
            minX = offset;
            minY = offset;
            maxX = Screen.width - offset;
            maxY = Screen.height - offset;
        }
        GameplayResultManager.Instance.MinigameResult.ScreenHeight = Screen.height;
        GameplayResultManager.Instance.MinigameResult.ScreenWidth = Screen.width;
        clickObjImg.gameObject.SetActive(false);
        finishUIObj.SetActive(false);
        object_type = UnityEngine.Random.Range(0, 4);
        sequenceObj = GenerateList();
        int _tempval = 0;
        for (int i = 0; i < 20; i++)
        {
            game_score.Add(0);
            if (sequenceObj[i] == 1)
            {
                sequenceObj[i] = 5; //Incorrect
            }
            else
            {
                sequenceObj[i] = 6; //Correct
            }
        }
        for (int i = 0; i < 20; i++)
        {
            if (sequenceObj[i] == 6)
            {
                sequenceObj[i] = object_type;
            }
            else
            {
                do
                {
                    _tempval = UnityEngine.Random.Range(0, 4);
                } while (object_type == _tempval);
                sequenceObj[i] = _tempval;
            }
        }

        GameplayResultManager.Instance.MinigameResult.RandomIDLogList = sequenceObj;
        // for (int i = 0; i < 10; i++)
        // {
        //     sequenceObj.Add(0);
        // }
        // for (int i = 0; i < 5; i++)
        // {
        //     var correct_order = UnityEngine.Random.Range(0, 10);
        //     while (sequenceObj[correct_order] == 1)
        //     {
        //         correct_order = (correct_order + 1) % 10;
        //     }
        //     sequenceObj[correct_order] = 1;
        // }
        popupStartGameObj.transform.GetChild(1).GetComponent<Image>().sprite = popupSprites[object_type];
        popupStartGameObj.SetActive(true);
        GameplayResultManager.Instance.MinigameResult.ObjectID = object_type;
        AudioController.ForceVolume();
        startTime = Time.time;

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
                    , MinigameClickStatusEnum.FALSE));
                    
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
                if (curr_index == spawnTime)
                {
                    onComplete?.Invoke();
                    return;
                }
                NextImg();
                RandomPosition();
                timer = 0;
            }
            else
            {
                GameplayResultManager.Instance.MinigameResult.TimeUsed.Add(0);
                if (object_type == curr_obj)
                {
                    game_score[spawnCounter - 1] = 1;
                }
                else
                {
                    game_score[spawnCounter - 1] = 0;
                }
            }
            clickObjImg.gameObject.SetActive(isRoundActive);
            clickObjLateImg.gameObject.SetActive(!isRoundActive);
            timeLate = Time.time;
            clickObjLateImg.GetComponent<RectTransform>().position = clickObjImg.GetComponent<RectTransform>().position;
            if (spawnCounter >= spawnTime)
                onComplete?.Invoke();
            if (isRoundActive)
            {
                spawnCounter++;
            }
            else
            {
                //ADD ALPHA IMAGE
            }
        }).AddTo(this);

        onComplete = () =>
        {
            isStartGame = false;
            disposable.Dispose();
            clickObjImg.gameObject.SetActive(false);
            finishUIObj.SetActive(true);
            AudioController.SetVolume();
            GameplayResultManager.Instance.SpecialFuzzyData.GameID = FuzzyBrain.Instance.minigameCount.ToString();
            GameplayResultManager.Instance.SpecialFuzzyData.GameScore = game_score;
            GameplayResultManager.Instance.SpecialFuzzyData.TimeUsed = time_use;
            foreach(var item in GameplayResultManager.Instance.MinigameClickLogList){
                if(item.ClickStatus == MinigameClickStatusEnum.FALSE){
                    click_type_list[3]++;
                }
            }
            GameplayResultManager.Instance.SpecialFuzzyData.ClickTypeList = click_type_list;
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
    }
    public void NextImg()
    {
        clickObjImg.sprite = objSprites[sequenceObj[curr_index]];
        curr_obj = sequenceObj[curr_index];
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
        if (object_type == curr_obj)
        {
            AudioController.SetnPlay("audio/SFX/Correct_SpecialT");
            game_score[spawnCounter-1] = 1;
            time_use.Add(timer);
        }
        else
        {
            AudioController.SetnPlay("audio/SFX/Wrong_SpecialT");
            game_score[spawnCounter-1] = 0;
            click_type_list[1]++;
        }
        clickObjImg.gameObject.SetActive(false);
        isRoundActive = false;
        disposable.Dispose();
        CountDown();
        Debug.Log(GameplayResultManager.Instance.MinigameClickLogList[^1].ClickStatus);
    }
    public void OnLateClick()
    {
        GameplayResultManager.Instance.MinigameResult.TimeUsed[^1] = Time.time - timeLate;
        GameplayResultManager.Instance.MinigameClickLogList[^1].ClickStatus = MinigameClickStatusEnum.LATE;
        GameplayResultManager.Instance.MinigameClickLogList[^1].isCorrect = object_type == curr_obj ? true : false;
        if(object_type == curr_obj){
            // game_score[spawnCounter-1] = 1;
            click_type_list[0]++;
        }
        else
        {
            // game_score[spawnCounter-1] = 0;
            click_type_list[2]++;
        }
        clickObjLateImg.gameObject.SetActive(false);
    }

    public List<int> GenerateList()
    {
        int zeros = 10;
        int ones = 10;
        List<int> result = new List<int>();

        while (result.Count < 20)
        {
            // Calculate how many slots are left
            int slotsLeft = 20 - result.Count;

            // Ensure the remaining numbers can fit
            if (ones > 0 && (zeros + ones == slotsLeft ||
                (result.Count < 3 || result[^1] != 1 || result[^2] != 1 || result[^3] != 1)))
            {
                // Add a 1 if it fits the rules
                if (zeros == 0 || UnityEngine.Random.Range(0, zeros + ones) < ones)
                {
                    result.Add(1);
                    ones--;
                    continue;
                }
            }

            // Add a 0 otherwise
            if (zeros > 0)
            {
                result.Add(0);
                zeros--;
            }
        }

        return result;
    }
}
