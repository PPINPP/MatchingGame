using System;
using System.Collections.Generic;
using Manager;
using Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UxTest
{
  public class UXTest : MonoBehaviour
  {
    public GameObject UXtest_Title, UXtest_Q1, UXtest_Q2, UXtest_Q3;
    DateTime startedAt;
    int totalClick = 0;

    List<UxClickLog> uxClickLogs = new List<UxClickLog>();

    DataManager dataManager;

    // Start is called before the first frame update
    void Start()
    {
      dataManager = DataManager.Instance;
    }

    void Update()
    {
      if (Input.GetMouseButtonDown(0))
      {
        totalClick++;
        uxClickLogs.Add(new UxClickLog(Input.mousePosition.x, Input.mousePosition.y, DateTime.Now, Enum.UxClickStatusEnum.MISS));
      }
    }



    public void ToUXtest_Q1()
    {
      startedAt = DateTime.Now;
      UXtest_Q1.SetActive(true);
      UXtest_Q2.SetActive(false);
      UXtest_Q3.SetActive(false);
      UXtest_Title.SetActive(false);
    }

    public void ToUXtest_Q2()
    {
      uxClickLogs[^1].ClickStatus = Enum.UxClickStatusEnum.HIT;
      DateTime completedAt = DateTime.Now;
      float timeUsed = (float)completedAt.Subtract(startedAt).TotalSeconds;
      UxTestResult uxTestResult = new UxTestResult(totalClick, startedAt, completedAt, timeUsed, new List<UxClickLog>(uxClickLogs));
      dataManager.UxTestResultList.Add(uxTestResult);

      totalClick = 0;
      startedAt = DateTime.Now;
      uxClickLogs = new List<UxClickLog>();
      UXtest_Q2.SetActive(true);
      UXtest_Q3.SetActive(false);
      UXtest_Title.SetActive(false);
      UXtest_Q1.SetActive(false);
    }

    public void ToUXtest_Q3()
    {
      uxClickLogs[^1].ClickStatus = Enum.UxClickStatusEnum.HIT;
      DateTime completedAt = DateTime.Now;
      float timeUsed = (float)completedAt.Subtract(startedAt).TotalSeconds;
      UxTestResult uxTestResult = new UxTestResult(totalClick, startedAt, completedAt, timeUsed, new List<UxClickLog>(uxClickLogs));
      dataManager.UxTestResultList.Add(uxTestResult);

      totalClick = 0;
      startedAt = DateTime.Now;
      uxClickLogs = new List<UxClickLog>();
      startedAt = DateTime.Now;
      UXtest_Q3.SetActive(true);
      UXtest_Q1.SetActive(false);
      UXtest_Q2.SetActive(false);
      UXtest_Title.SetActive(false);
    }

    public void FinishTest(string target)
    {
      uxClickLogs[^1].ClickStatus = Enum.UxClickStatusEnum.HIT;
      DateTime completedAt = DateTime.Now;
      float timeUsed = (float)completedAt.Subtract(startedAt).TotalSeconds;
      UxTestResult uxTestResult = new UxTestResult(totalClick, startedAt, completedAt, timeUsed, new List<UxClickLog>(uxClickLogs));
      dataManager.UxTestResultList.Add(uxTestResult);

      SceneManager.LoadScene(target);
    }
  }
}