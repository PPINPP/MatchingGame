using System;
using System.Collections;
using System.Collections.Generic;
using MatchingGame.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectorUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    float curr_page = 0.0f;
    [SerializeField] List<Image> BackgroudTile = new List<Image>();
    [SerializeField] List<GameObject> LevelButton = new List<GameObject>();
    [SerializeField] GameObject EndDayPopup;
    [SerializeField] GameObject EndWeekPopup;
    [SerializeField] List<Sprite> EndWeekTile;
    [SerializeField] Transform BGTile;
    float checkTime;
    List<string> dayinweek = new List<string>() { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
    void Start()
    {
        curr_page = LevelSelectorManager.Instance.save_curr_page;
        BGTile.localPosition = new Vector3(curr_page, 0, 0);
        checkTime = 0.0f;

        LevelSelectorManager.Instance.UpdateTile(BackgroudTile, LevelButton);
        AudioController.SetnPlayBGM("audio/BGM/BGM_Main");
        //AddFirst
        if (!FirebaseManagerV2.Instance.week_day[FirebaseManagerV2.Instance.curr_week.ToString()].Contains(DateTime.Now.ToString("yyyyMMdd")))
        {
            FirebaseManagerV2.Instance.week_day[FirebaseManagerV2.Instance.curr_week.ToString()].Add(DateTime.Now.ToString("yyyyMMdd"));
            FirebaseManagerV2.Instance.UpdateWeekDays();
        }
        //Check
        if (LevelSelectorManager.Instance.AllCompleteCheck())
        {
            if (FirebaseManagerV2.Instance.week_day[FirebaseManagerV2.Instance.curr_week.ToString()].Count == 7)
            {
                if (FirebaseManagerV2.Instance.curr_week != 8)
                {
                    Debug.Log(FirebaseManagerV2.Instance.timeRules.Count);
                    Debug.Log(FirebaseManagerV2.Instance.curr_week * 7);
                    var dayStr = DateTime.ParseExact(FirebaseManagerV2.Instance.timeRules[FirebaseManagerV2.Instance.curr_week * 7], "yyyyMMdd", null).DayOfWeek.ToString();
                    EndWeekPopup.transform.GetChild(0).GetComponent<Image>().sprite = EndWeekTile[dayinweek.IndexOf(dayStr)];
                    EndWeekPopup.SetActive(true);
                    return;
                }
                else
                {
                    SceneManager.LoadScene("EndTest");
                    return;
                }

            }
            else
            {
                EndDayPopup.SetActive(true);
                return;
            }

        }
        else
        {
            if (FirebaseManagerV2.Instance.week_day[FirebaseManagerV2.Instance.curr_week.ToString()].Count >= 7)
            {
                if (FirebaseManagerV2.Instance.curr_week != 8)
                {
                    Debug.Log(FirebaseManagerV2.Instance.timeRules.Count);
                    Debug.Log(FirebaseManagerV2.Instance.curr_week * 7);
                    var dayStr = DateTime.ParseExact(FirebaseManagerV2.Instance.timeRules[FirebaseManagerV2.Instance.curr_week * 7], "yyyyMMdd", null).DayOfWeek.ToString();
                    EndWeekPopup.transform.GetChild(0).GetComponent<Image>().sprite = EndWeekTile[dayinweek.IndexOf(dayStr)];
                    EndWeekPopup.SetActive(true);
                    return;
                }
                else
                {
                    SceneManager.LoadScene("EndTest");
                    return;
                }
            }
        }
        checkTime = Time.time;
        FirebaseManagerV2.Instance.checkTimeChange();
    }

    // Update is called once per frame
    public void OnButtonClickToStartGame(int levelnum)
    {
        AudioController.StopPlayBGM();
        curr_page = BGTile.localPosition.x;
        LevelSelectorManager.Instance.StartLevel(levelnum, curr_page);
    }

    public void SyncData()
    {
        FirebaseManagerV2.Instance.SyncData();
    }
    public void ConfirmExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    void Update()
    {
        if (checkTime != 0.0f)
        {
            if (Time.time - checkTime > 5.0f)
            {
                FirebaseManagerV2.Instance.checkTimeChange();
                checkTime = Time.time;
            }
        }

    }

}
