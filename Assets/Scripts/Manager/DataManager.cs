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

      await Task.WhenAll(tasks.ToArray());
    }
  }
}