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

      for (int i = 0; i <= GamePlayResultList.Count; i++)
      {
        tasks.Add(FirebaseManager.Instance.CreateDataWithDoc(UserInfo.Username, $"DemoResult/GameplayLog/task{i:D2}", GamePlayResultList[i].ConverToFirestoreModel(), SetOptions.Overwrite));
      }

      await Task.WhenAll(tasks.ToArray());
    }
  }
}