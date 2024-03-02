using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UITest : MonoBehaviour
{
    // Start is called before the first frame update
    //[SerializeField] private GameObject TestUI_Q01, TestUI_Q02, TestUI_Q03, TestUI_Q04, TestUI_Q05, TestUI_Q06;
    public GameObject intoClick;
    public GameObject background;
    int q,a;
    private string question;
    private UI_Question UIQuestion;
    DateTime startTime=DateTime.Now;
    DateTime endTime=DateTime.Now;


    // Push to DB
    //DocumentReference docref = db.Collection("GamePlayHistory").Document(loginuser.DisplayName);
    Dictionary<string, object> UIAnswerDict = new Dictionary<string, object>();

    
    /*docref.SetAsync(user, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to db successfully.");
        }
    );*/
    // Finish add to DB


    public void SwitchUITest()
    {
        //intoToTest.GetComponent<Image>().sprite = Test;
        //UIQuestion.SetQuestion();
        Debug.Log("SwitchUITest button pressed");
        Debug.Log("CheckName: "+background.name);
        background.SetActive(false);
        //TestUI_Q01.SetActive(true);
        

        intoClick.SetActive(false);
        startTime=DateTime.Now;

    }


    public void CollectScore()
    {
        //string question;
        string answer = EventSystem.current.currentSelectedGameObject.name;
        
        

        /*Debug.Log("Question button pressed: " + question);
        Debug.Log("Answer button pressed: " + answer);
        Debug.Log("Time taken: "+(endTime-startTime));
        UIAnswerDict.Add("Question", answer);
        UIAnswerDict.Add("Answer", answer);
        UIAnswerDict.Add("Process time", (endTime-startTime));*/
        

        Dictionary<string, Dictionary<string, object>> user = new Dictionary<string, Dictionary<string, object>>()
        {
            {
                "UITest", UIAnswerDict
            }   
        };
        /*docref.SetAsync(UIAnswerDict, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
            {
                Debug.Log("Added data to db successfully.");
            }
        );*/
        // เพิ่มเรื่องของการจับเวลา
        

    }

    void Start()
    {
        /*TestUI_Q01.SetActive(false);
        TestUI_Q02.SetActive(false);
        TestUI_Q03.SetActive(false);
        TestUI_Q04.SetActive(false);
        TestUI_Q05.SetActive(false);
        TestUI_Q06.SetActive(false);*/
        
    }


    
}
