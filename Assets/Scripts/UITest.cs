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
    public GameObject TestUI_Q01;
    public GameObject intoClick;
    public GameObject background;
    //private bool _stateChange = false;
    [SerializeField] Dictionary<string, string> qAPairs= new Dictionary<string, string>();
    //[SerializeField] DictAnswerPair newAnswer;
    
    
    //[SerializeField] QAPair[] qAPairs;

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
                UIAnswer.Add(qAPair, qAPair);
            }
            return UIAnswer;
        } 
    }  */
    public void SwitchUITest() 
    {
        //intoToTest.GetComponent<Image>().sprite = Test;
        Debug.Log("SwitchUITest button pressed");
        Debug.Log("CheckName: "+background.name);
        background.SetActive(false);
        TestUI_Q01.SetActive(true);
        intoClick.SetActive(false);
        //_stateChange = true;

    }

    public void CollectScore()
    {
        string question = EventSystem.current.currentSelectedGameObject.name;
        string answer = EventSystem.current.currentSelectedGameObject.name;

        Debug.Log("Question button pressed" + question);
        Debug.Log("Answer button pressed" + answer);
        qAPairs.Add(question, answer);

    }

    void Start()
    {
        foreach (var qAPair in qAPairs)
            {
                Debug.Log("QAPair: " + qAPairs );
            }
    }

        /*void Update()
        {
        if (Input.GetMouseButtonDown(0))
            {
                //empty RaycastHit object which raycast puts the hit details into
                var hit : RaycastHit;
                //ray shooting out of the camera from where the mouse is
                var ray : Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, hit))
                {
                    //print out the name if the raycast hits something
                    Debug.Log(hit.collider.name);
                }
            }
        }*/


    
}
