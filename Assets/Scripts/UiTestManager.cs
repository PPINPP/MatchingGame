using Model;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UiTestManager : MonoBehaviour
    {
        [Title("Main Menu")]
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private Button startBtn;

        [Title("Questions")]
        [SerializeField] private GameObject questionsHolder;
        [SerializeField] List<Question> questionsList;
        private int questionLenght = 0;
        private int currQuestionIdx = 0;

        private List<UiTestResult> uiTestResultsList;

        // Use this for initialization
        void Start()
        {
            questionLenght = questionsList.Count;
            startBtn.onClick.AddListener(() =>
            {
                Debug.Log("Start asking questions");
                mainMenu.SetActive(false);
                questionsHolder.SetActive(true);
                initiateQuestion(currQuestionIdx);
            });
            uiTestResultsList = new List<UiTestResult>();
        }

        private void initiateQuestion(int questionIdx) 
        {
            Question question = questionsList[questionIdx];

            question.gameObject.SetActive(true);
            question.correctAnswer.onClick.AddListener(answer);
        }

        private void answer() 
        {
            if(currQuestionIdx >= questionLenght) 
            {
                Debug.Log("Finish all Questions! congrat");
                return;
            }

            UiTestResult result = new UiTestResult(
                    "test",
                    DateTime.Now,
                    DateTime.Now,
                    10f
                );

            questionsList[currQuestionIdx].gameObject.SetActive(false);
            initiateQuestion(++currQuestionIdx);
        }
    }

    [Serializable]
    public struct Question
    {
        public GameObject gameObject;
        public Button correctAnswer;
    }
}