using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UI_Question : MonoBehaviour //ต้องตั้งชื่อ class ตรงกับชื่อไฟล์
    {
        [SerializeField] private string Question;
        [SerializeField] private GameObject UIQuestionBG;
        [SerializeField] private Button Answer1, Answer2, Answer3;
        
        private UITest[] UI_Tester = new UITest[6];
        private DateTime startTime;
        private DateTime endTime;

        public void Init(UITest[] uiTester) // void คือไม่ return อะไร
        {
            UI_Tester = uiTester;

        }

        public void SetQuestion()
        {
            UIQuestionBG.SetActive(true);
            startTime = DateTime.Now;
        }

        void Start()
        {
            Answer1.onClick.AddListener(() => OnAnswerClick("A"));
            Answer2.onClick.AddListener(() => OnAnswerClick("B"));
            Answer3.onClick.AddListener(() => OnAnswerClick("C"));
        }

        private void OnAnswerClick(string Answer)
        {
            endTime=DateTime.Now;
            UIQuestionBG.SetActive(false);
            Debug.Log("Question: " + Question);
            Debug.Log("Answer: " + Answer);
            Debug.Log("Process tiem: " + (endTime - startTime));
            
            Dictionary<string, object> result = new Dictionary<string, object>()
            {
                {"Question", Question},
                {"Answer", Answer},
                {"Process time", (endTime - startTime)}
            };

            //UI_Tester[0].SetResult(result);
            UIQuestionBG.SetActive(false);


            startTime = DateTime.Now;

        }
    }