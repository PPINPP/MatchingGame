using Manager;
using Model;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UiTest
{
  public class UiTestManager : MonoBehaviour
  {
    [Title("Next Scene")]
    [SerializeField] private string targetScene;

    [Title("Main Menu")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Button startBtn;

    [Title("Questions")]
    [SerializeField] private GameObject questionsHolder;
    [SerializeField] List<GameObject> questionsList;
    private int questionLength = 0;
    private int currQuestionIdx = 0;

    private DateTime startedAt = new DateTime(1999, 1, 1);
    private DateTime completedAt = new DateTime(1999, 1, 1);
    FirebaseManagerV2 fbm;

    // Use this for initialization
    void Start()
    {
      fbm = FirebaseManagerV2.GetInstance();
      mainMenu.SetActive(true);
      questionsHolder.SetActive(false);

      questionLength = questionsList.Count;

      startBtn.onClick.AddListener(() =>
      {
        Debug.Log("Start asking questions");
        mainMenu.SetActive(false);
        questionsHolder.SetActive(true);
        initiateNextQuestion(currQuestionIdx);
      });
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
      UiTestResult uiTestResult = new UiTestResult(clickedBtn.name,
              DateTime.Parse(startedAt.ToString()),
              DateTime.Parse(completedAt.ToString()),
              timeUsed);
      DataManager.Instance.UiTestResultList.Add(uiTestResult);
      FirebaseManagerV2.Instance.UploadUiTestResult(uiTestResult.ConvertUiTestResultToUiTestResultFs(),DataManager.Instance.UiTestResultList.Count-1);


      startedAt = new DateTime(1999, 1, 1);
      completedAt = new DateTime(1999, 1, 1);

      currQuestionIdx = currQuestionIdx + 1;

      if (currQuestionIdx >= questionLength)
      {
        Debug.Log("Finish all Questions! congrat");
        foreach (var e in DataManager.Instance.UiTestResultList)
        {
          Debug.Log($"{e.SelectedChoice} {e.StartedAt} {e.CompletedAt} {e.TimeUsed}");
        }

        SceneManager.LoadScene(targetScene);

        return;
      }

      questionsList[currQuestionIdx].gameObject.SetActive(false);
      initiateNextQuestion(currQuestionIdx);
    }
  }
}