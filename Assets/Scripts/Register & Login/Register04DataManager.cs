using UnityEngine;
using UnityEngine.SceneManagement;
using Enum;
using Manager;
using UnityEngine.UI;
using Constant;

namespace Register
{
  public class Register04DataManager : MonoBehaviour
  {
    public GameObject noButton, mciButton, alzButton;
    public string nextScene;
    DementiaStageEnum dementiaStage;

    private void Start()
    {
      noSelected();
    }

    public void SubmitForm()
    {
      Debug.Log($"brainDiagnostic: {dementiaStage}");

      DataManager.Instance.UserInfo.DementiaStage = dementiaStage;

      SceneManager.LoadScene(nextScene);
    }

    public void noSelected()
    {
      dementiaStage = DementiaStageEnum.HEALTHY;

      var noButtonNormalColor = noButton.GetComponent<Button>().colors;
      var mciButtonNormalColor = mciButton.GetComponent<Button>().colors;
      var alzButtonNormalColor = alzButton.GetComponent<Button>().colors;

      noButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.SELECTED_COLOR); ;
      mciButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.UNSELECTED_COLOR); ;
      alzButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.UNSELECTED_COLOR); ;

      noButton.GetComponent<Button>().colors = noButtonNormalColor;
      mciButton.GetComponent<Button>().colors = mciButtonNormalColor;
      alzButton.GetComponent<Button>().colors = alzButtonNormalColor;
    }

    public void alzSelected()
    {
      dementiaStage = DementiaStageEnum.ALZHEIMER;

      var noButtonNormalColor = noButton.GetComponent<Button>().colors;
      var mciButtonNormalColor = mciButton.GetComponent<Button>().colors;
      var alzButtonNormalColor = alzButton.GetComponent<Button>().colors;

      noButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.UNSELECTED_COLOR); ;
      mciButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.UNSELECTED_COLOR); ;
      alzButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.SELECTED_COLOR); ;

      noButton.GetComponent<Button>().colors = noButtonNormalColor;
      mciButton.GetComponent<Button>().colors = mciButtonNormalColor;
      alzButton.GetComponent<Button>().colors = alzButtonNormalColor;
    }

    public void mciSelected()
    {
      dementiaStage = DementiaStageEnum.MCI;

      var noButtonNormalColor = noButton.GetComponent<Button>().colors;
      var mciButtonNormalColor = mciButton.GetComponent<Button>().colors;
      var alzButtonNormalColor = alzButton.GetComponent<Button>().colors;

      noButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.UNSELECTED_COLOR); ;
      mciButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.SELECTED_COLOR); ;
      alzButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.UNSELECTED_COLOR); ;

      noButton.GetComponent<Button>().colors = noButtonNormalColor;
      mciButton.GetComponent<Button>().colors = mciButtonNormalColor;
      alzButton.GetComponent<Button>().colors = alzButtonNormalColor;
    }
  }
}