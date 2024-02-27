using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;


public class UITest : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject intoToTest;
    public GameObject TestUI_Q01, TestUI_Q02, TestUI_Q03, TestUI_Q04, TestUI_Q05, TestUI_Q06;
    public GameObject intoClick;
    public GameObject background;
    private string question;
    //private bool _stateChange = false;
    [SerializeField] Dictionary<string, string> qAPairs= new Dictionary<string, string>();

    [Serializable]
    public class QAPair 
    {
        [SerializeField] private GameObject Question;
        [SerializeField] private GameObject Answer;
        
    }
    public void SwitchUITest() 
    {
        //intoToTest.GetComponent<Image>().sprite = Test;
        Debug.Log("SwitchUITest button pressed");
        Debug.Log("CheckName: "+background.name);
        //background.SetActive(false);
        TestUI_Q01.SetActive(true);
        intoClick.SetActive(false);
        Debug.Log(TestUI_Q01.activeSelf);
        Debug.Log(TestUI_Q01.activeInHierarchy);
        //_stateChange = true;

    }

    public void CollectScore()
    {
        //string question;
        string answer = EventSystem.current.currentSelectedGameObject.name;

        if (TestUI_Q01.activeInHierarchy==true){
            TestUI_Q01.SetActive(false);
            TestUI_Q02.SetActive(true);
            question = "Q01";
        }
        else if (TestUI_Q02.activeInHierarchy==true){
            TestUI_Q02.SetActive(false);
            TestUI_Q03.SetActive(true);
            question = "Q02";
        }
        else if (TestUI_Q03.activeInHierarchy==true){
            TestUI_Q03.SetActive(false);
            TestUI_Q04.SetActive(true);
            question = "Q03";
        }
        else if (TestUI_Q04.activeInHierarchy==true){
            TestUI_Q04.SetActive(false);
            TestUI_Q05.SetActive(true);
            question = "Q04";
        }
        else if (TestUI_Q05.activeInHierarchy==true){
            TestUI_Q05.SetActive(false);
            TestUI_Q06.SetActive(true);
            question = "Q05";
        }
        else if (TestUI_Q06.activeInHierarchy==true){
            TestUI_Q06.SetActive(false);
            intoClick.SetActive(true);
            question = "Q06";
        }
        else{
            Debug.Log("No active UI");
            Debug.Log("All answers collected: "+qAPairs);
        }
        

        Debug.Log("Question button pressed: " + question);
        Debug.Log("Answer button pressed: " + answer);
        qAPairs.Add(question, answer);

    }

    void Start()
    {
        TestUI_Q01.SetActive(false);
        TestUI_Q02.SetActive(false);
        TestUI_Q03.SetActive(false);
        TestUI_Q04.SetActive(false);
        TestUI_Q05.SetActive(false);
        TestUI_Q06.SetActive(false);
        foreach (var qAPair in qAPairs)
            {
                Debug.Log("QAPair: " + qAPairs );
            }
    }


    
}
