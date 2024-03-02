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
        
        private UITest UI_Tester;
        private DateTime startTime;
        private DateTime endTime;

        public void Init(UITest uiTester) // ถ้าในกรณีที่ต้องใช้ class ของ class ใหญ่ ควรทำเป็น init แยกจาก void start ของ class ตัวเล็ก
        {
            UI_Tester = uiTester; 
            
        }
        public void ShowQuestion(bool show) // void คือไม่ return อะไร
        {
            this.gameObject.SetActive(show);
            if (!show) return ;
            startTime = DateTime.Now; // guard condition

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
            Debug.Log("Process tiem: " + (endTime - startTime).TotalSeconds);
            
            Dictionary<string, string> result = new Dictionary<string, string>()
            {
                {"Question", Question},
                {"Answer", Answer},
                {"Process time", (endTime - startTime).TotalSeconds.ToString()}
            };
            UI_Tester.CollectAnswer(result);
            

        }
    }