using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class UITest : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject intoToTest;
    public GameObject TestUI_Q01;
    public GameObject intoClick;
    public GameObject background;
    private bool _stateChange = false;
    [SerializeField] Dictionary<string, string> objectNames;
    //[SerializeField] DictAnswerPair newAnswer;
    
    
    [SerializeField] QAPair[] qAPairs;

    [Serializable]
    public class QAPair 
    {
        [SerializeField] private GameObject Question;
        [SerializeField] private GameObject Answer;
        
    }

    /*public class DictAnswerPair{
        [SerializeField] QAPair[] qAPairs;
        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string,string> UIAnswer = new Dictionary<string, string>();
            foreach (var qAPair in qAPairs)
            {
                UIAnswer.Add(qAPair.Question, qAPair.Answer);
            }
            return UIAnswer;
        } 
    }  */
    

    public void SwitchUITest() 
    {
        //intoToTest.GetComponent<Image>().sprite = Test;
        Debug.Log("SwitchUITest button pressed");
        background.SetActive(false);
        TestUI_Q01.SetActive(true);
        intoClick.SetActive(false);
        _stateChange = true;

    }

    public void CollectScore(QAPair[] UIAnswer)
    {
        
        if (_stateChange == true)
        {
            Debug.Log("CollectScore: " + UIAnswer + ", " + UIAnswer);
            //objectNames = DictAnswerPair.ToDictionary();
        }

        _stateChange = false;

        public voide AnswerA1(string question)
        {
            Debug.Log("AnswerA1 button pressed");
            qAPairs.add(question, "A1");
        }
       

    }

    void Start()
    {
        foreach (var qAPair in qAPairs)
            {
                Debug.Log("QAPair: " + qAPairs );
            }
        
    }


    
}
