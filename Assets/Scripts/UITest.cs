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
    public GameObject intoClick;
    public GameObject background;
    private string question;
    private int qNum=0;
    private Dictionary<string, List<Dictionary<string, string>>> user = new()
    { 
        {"UITest", new() } 
    };

    [SerializeField] private UI_Question[] all_question;
    DateTime startTime=DateTime.Now;
    DateTime endTime=DateTime.Now;


    // Push to DB
    //DocumentReference docref = db.Collection("GamePlayHistory").Document(loginuser.DisplayName);


    /*docref.SetAsync(user, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to db successfully.");
        }
    );*/
    // Finish add to DB

    private bool TryShowQuestion() //ควรต้ั้งชื่อฟังก์ชันให้สื่อถึงการทำงานของฟังก์ชัน
    {
        // Check is more question?
        if (qNum >= all_question.Length)
        {
            return false;
        }
        
        // Show Next Question
        all_question[qNum].ShowQuestion(true);
        return true;

        /*
        string test = "1";
        bool test2 = int.TryParse(test, out int result); // out คือ return (สามารถ return ได้หลายค่าโดยการใส่วงเล็บ)
        */
    }


    public void SwitchUITest()
    {
        Debug.Log("SwitchUITest button pressed");
        Debug.Log("CheckName: "+background.name);
        background.SetActive(false);
        intoClick.SetActive(false);
        startTime=DateTime.Now;

        TryShowQuestion();
        
    }



    public void CollectAnswer(Dictionary<string, string> result)
    {

        //user.Add("UITest", result);
        user["UITest"].Add(result);
        all_question[qNum].ShowQuestion(false);
        qNum++;

        if (!TryShowQuestion())
        {
            var test = user;
            Debug.Log("End!");
        }



        /*docref.SetAsync(UIAnswerDict, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
            {
                Debug.Log("Added data to db successfully.");
            }
        );*/


    }

    void Start()
    {
        foreach (UI_Question question in all_question) // ถ้าใช้ var จะหาให้ว่า datatype คืออะไร แต่คนเขียนจะไม่รู้
        {
            question.ShowQuestion(false);
            question.Init(this);
        }
    }


    
}
