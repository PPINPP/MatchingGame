using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constant;
using Firebase.Firestore;
using Model;

namespace Manager
{
  class DataManager : MonoSingleton<DataManager>
  {
    public UserInfo UserInfo { get; set; } = new UserInfo();
    public List<UxTestResult> UxTestResultList { get; set; } = new List<UxTestResult>();
    public List<UiTestResult> UiTestResultList { get; set; } = new List<UiTestResult>();
    public List<SmileyoMeterResult> SmileyoMeterResultList { get; set; } = new List<SmileyoMeterResult>();
    public List<GamePlayResult> GamePlayResultList { get; set; } = new List<GamePlayResult>();
    public List<GamePlayResult> TutorialResultList { get; set; } = new List<GamePlayResult>();
    public List<MinigameResult> MinigameResultList { get; set; } = new List<MinigameResult>();

    public async void PushDataToFirebase()
    {
      List<Task> tasks = new List<Task>
      {
        FirebaseManager.Instance.RegisterUserAsync(UserInfo.Username + FirebaseConstant.EMAIL_SUFFIX, UserInfo.Password),
        FirebaseManager.Instance.CreateDataWithDoc(UserInfo.Username, $"UserInfo", UserInfo.ConvertUserInfoToUserInfoFs(), SetOptions.Overwrite)
      };

      for (int i = 0; i < UiTestResultList.Count; i++)
      {
        tasks.Add(FirebaseManager.Instance.CreateDataWithDoc(UserInfo.Username, $"DemoResult/UI Test/task{i:D2}", UiTestResultList[i].ConvertUiTestResultToUiTestResultFs(), SetOptions.Overwrite));
      }

      for (int i = 0; i < UxTestResultList.Count; i++)
      {
        tasks.Add(FirebaseManager.Instance.CreateDataWithDoc(UserInfo.Username, $"DemoResult/UX Test/task{i:D2}", UxTestResultList[i].ConvertUxTestResultToUxTestResultFs(), SetOptions.Overwrite));
      }

      for (int i = 0; i < SmileyoMeterResultList.Count; i++)
      {
        tasks.Add(FirebaseManager.Instance.CreateDataWithDoc(UserInfo.Username, $"DemoResult/SmileyoMeter/task{i:D2}", SmileyoMeterResultList[i].ConvertSmileyoMeterResultToSmileyoMeterResultFs(), SetOptions.Overwrite));
      }

      for (int i = 0; i < MinigameResultList.Count; i++)
      {
        tasks.Add(FirebaseManager.Instance.CreateDataWithDoc(UserInfo.Username, $"DemoResult/SpecialTask/task{i:D2}_{MinigameResultList[i].CompletedAt:s}", MinigameResultList[i].ConverToFirestoreModel(), SetOptions.Overwrite));
      }
      
      for (int i = 0; i < TutorialResultList.Count; i++)
      {
        tasks.Add(FirebaseManager.Instance.CreateDataWithDoc(UserInfo.Username, $"DemoResult/TutorialLog/{TutorialResultList[i].StageID}_{TutorialResultList[i].CompletedAt:s}", TutorialResultList[i].ConvertToFirestoreModel(), SetOptions.Overwrite));
      }
      
      for (int i = 0; i < GamePlayResultList.Count; i++)
      {
        tasks.Add(FirebaseManager.Instance.CreateDataWithDoc(UserInfo.Username, $"DemoResult/GameplayLog/{GamePlayResultList[i].StageID}_{GamePlayResultList[i].CompletedAt:s}", GamePlayResultList[i].ConvertToFirestoreModel(), SetOptions.Overwrite));
      }

      await Task.WhenAll(tasks.ToArray());
    }
  }
}