using Model;
using System.Collections.Generic;

namespace Manager
{
  class DataManager : MonoSingleton<DataManager>
  {
    public UserInfo UserInfo { get; set; } = new UserInfo();
    public List<UiTestResult> UiTestResultList { get; set; } = new List<UiTestResult>();
    public UxTestResult UxTestResult { get; set; } = new UxTestResult();
  }
}