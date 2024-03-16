using UnityEngine;
using UnityEngine.SceneManagement;
using Enum;
using Manager;

public class Register02DataManager : MonoBehaviour
{
  public GameObject primButton, highsButton, certButton, bacButton, masButton, phdButton;
  public string nextScene;
  EducationalEnum educationalLevel;


  public void SubmitForm()
  {
    Debug.Log($"Educational Select: {educationalLevel}");

    DataManager.Instance.UserInfo.EducationalLevel = educationalLevel;

    SceneManager.LoadScene(nextScene);
  }

  public void primSelected()
  {
    educationalLevel = EducationalEnum.PRIMARY_OR_LOWER;
  }
  public void highsSelected()
  {
    educationalLevel = EducationalEnum.HIGH_SCHOOL;
  }
  public void certSelected()
  {
    educationalLevel = EducationalEnum.TECHNICAL;
  }
  public void bacSelected()
  {
    educationalLevel = EducationalEnum.BACHELOR_DEGREE;
  }
  public void masSelected()
  {
    educationalLevel = EducationalEnum.MASTER_DEGREE;
  }
  public void phdSelected()
  {
    educationalLevel = EducationalEnum.DOCTORAL;
  }
}
