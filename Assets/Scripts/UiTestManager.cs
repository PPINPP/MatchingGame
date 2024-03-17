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
        [SerializeField] List<GameObject> questionsList;
        private int questionLenght = 0;
        private int currQuestionIdx = 0;

        private List<UiTestResult> uiTestResultsList;

        private DateTime startedAt = new DateTime(1999,1,1);
        private DateTime completedAt = new DateTime(1999, 1, 1);

        // Use this for initialization
        void Start()
        {
            mainMenu.SetActive(true);
            questionsHolder.SetActive(false);

            questionLenght = questionsList.Count;

            startBtn.onClick.AddListener(() =>
            {
                Debug.Log("Start asking questions");
                mainMenu.SetActive(false);
                questionsHolder.SetActive(true);
                initiateNextQuestion(currQuestionIdx);
            });

            uiTestResultsList = new List<UiTestResult>();
        }

        private void initiateNextQuestion(int idx) 
        {
            GameObject question = questionsList[idx];

            foreach (Button btn in question.GetComponentsInChildren<Button>())
            {
                btn.onClick.AddListener(() => answer(btn));
            }

            question.gameObject.SetActive(true);

            startedAt = DateTime.Now;
        }

        private void answer(Button clickedBtn)
        { 
            completedAt = DateTime.Now;
            float timeUsed = (float)completedAt.Subtract(startedAt).TotalSeconds;

            uiTestResultsList.Add(new UiTestResult(
                    clickedBtn.name,
                    DateTime.Parse(startedAt.ToString()),
                    DateTime.Parse(completedAt.ToString()),
                    timeUsed
                ));

            startedAt = new DateTime(1999, 1, 1);
            completedAt = new DateTime(1999, 1, 1);

            currQuestionIdx = currQuestionIdx + 1;

            if (currQuestionIdx >= questionLenght)
            {
                Debug.Log("Finish all Questions! congrat");
                foreach (var e in uiTestResultsList)
                {
                    Debug.Log($"{e.SelectedChoice} {e.StartedAt} {e.CompletedAt} {e.TimeUsed}");
                }
                return;
            }

            questionsList[currQuestionIdx].gameObject.SetActive(false);
            initiateNextQuestion(currQuestionIdx);
        }
    }
}