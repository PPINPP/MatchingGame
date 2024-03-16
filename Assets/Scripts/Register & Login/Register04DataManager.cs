using UnityEngine;
using UnityEngine.SceneManagement;
using Enum;
using Manager;

namespace Register
{
  public class Register04DataManager : MonoBehaviour
  {
    public GameObject noButton, mciButton, alzButton;
    public string nextScene;
    DementiaStageEnum dementiaStage;

    public void SubmitForm()
    {
      Debug.Log($"brainDiagnostic: {dementiaStage}");

      DataManager.Instance.UserInfo.DementiaStage = dementiaStage;

      SceneManager.LoadScene(nextScene);
    }

    public void noSelected()
    {
      dementiaStage = DementiaStageEnum.HEALTHY;
    }

    public void alzSelected()
    {
      dementiaStage = DementiaStageEnum.ALZHEIMER;
    }

    public void mciSelected()
    {
      dementiaStage = DementiaStageEnum.MCI;
    }
  }
}