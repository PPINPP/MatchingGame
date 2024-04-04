using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Manager;
using Enum;
using System;
using Constant;
using UnityEngine.UI;

namespace Register
{
  public class Register01DataManager : MonoBehaviour
  {
    public TMP_InputField DateField, MonthField, YearField;
    public GameObject MaleButton, FemaleButton;
    public string nextScene;

    public GendersEnum Gender;

    private void Start()
    {
      MaleSelected();
    }

    public void SubmitForm()
    {
      string date = DateField.text;
      string month = MonthField.text;
      string year = YearField.text;
      Debug.Log($"Date: {date}, Month: {month}, Year: {year}, Gender: {Gender}");
      string dateString = $"{date}/{month}/{int.Parse(year) - DateTimeConstant.BUDDHIST_ERA_YEAR}";

      if (DateTime.TryParseExact(dateString, DateTimeConstant.DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateResult))
      {
        DataManager.Instance.UserInfo.DateOfBirth = dateResult;
        DataManager.Instance.UserInfo.Gender = Gender;
        SceneManager.LoadScene(nextScene);
      }
      else
      {
        Debug.LogError($"DateTime.TryParseExact({dateString}) error");
      }
    }

    public void MaleSelected()
    {
      Gender = GendersEnum.MALE;
      var maleButtonNormalColor = MaleButton.GetComponent<Button>().colors;
      var femaleButtonNormalColor = FemaleButton.GetComponent<Button>().colors;

      maleButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.SELECTED_COLOR); ;
      femaleButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.UNSELECTED_COLOR); ;

      MaleButton.GetComponent<Button>().colors = maleButtonNormalColor;
      FemaleButton.GetComponent<Button>().colors = femaleButtonNormalColor;
    }
    public void FemaleSelected()
    {
      Gender = GendersEnum.FEMALE;
      var maleButtonNormalColor = MaleButton.GetComponent<Button>().colors;
      var femaleButtonNormalColor = FemaleButton.GetComponent<Button>().colors;

      maleButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.UNSELECTED_COLOR); ;
      femaleButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.SELECTED_COLOR); ;

      MaleButton.GetComponent<Button>().colors = maleButtonNormalColor;
      FemaleButton.GetComponent<Button>().colors = femaleButtonNormalColor;
    }
  }
}