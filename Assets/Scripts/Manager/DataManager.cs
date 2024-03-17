using System.Collections.Generic;
using Model;

namespace Manager
{
  class DataManager : MonoSingleton<DataManager>
  {
    public UserInfo UserInfo { get; set; } = new UserInfo();
    public List<UxTestResult> UxTestResultList { get; set; } = new List<UxTestResult>();
    public List<UiTestResult> UiTestResultList { get; set; } = new List<UiTestResult>();
  }
}