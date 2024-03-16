using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Manager;
using Enum;
using System;
using Constant;

namespace Register
{
  public class Register01DataManager : MonoBehaviour
  {
    public TMP_InputField AgeField, DateField, MonthField, YearField;
    public GameObject MaleButton, FemaleButton;
    public string nextScene;

    public GendersEnum Gender;

    public void SubmitForm()
    {
      string age = AgeField.text;
      string date = DateField.text;
      string month = MonthField.text;
      string year = YearField.text;
      Debug.Log($"Age: {age}, DOB: {date}, Month: {month}, Year: {year}, Gender: {Gender}");
      string dateString = $"{date}/{month}/{int.Parse(year) - DateTimeConstant.BUDDHIST_ERA_YEAR}";

      if (DateTime.TryParseExact(dateString, DateTimeConstant.DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateResult))
      {
        DataManager.Instance.UserInfo.DateOfBirth = dateResult;
        DataManager.Instance.UserInfo.Gender = Gender;
      }

      SceneManager.LoadScene(nextScene);
    }

    public void MaleSelected()
    {
      Gender = GendersEnum.MALE;
    }
    public void FemaleSelected()
    {
      Gender = GendersEnum.FEMALE;
    }
  }
}