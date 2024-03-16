using Model;

namespace Manager
{
  class DataManager : MonoSingleton<DataManager>
  {
    public UserInfo UserInfo { get; set; } = new UserInfo();
    public UiTestResult UiTestResult { get; set; } = new UiTestResult();
    public UxTestResult UxTestResult { get; set; } = new UxTestResult();
  }
}