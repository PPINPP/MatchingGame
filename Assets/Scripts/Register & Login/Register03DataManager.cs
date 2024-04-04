using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Manager;

namespace Register
{
  public class Register03DataManager : MonoBehaviour
  {
    public Toggle allToggle, htToggle, dmToggle, asdToggle, hlToggle, isToggle, cardiacToggle;

    public string nextScene;

    public void SubmitForm()
    {
      Debug.Log($"allToggle: {allToggle.isOn}, htToggle: {htToggle.isOn}, dmToggle: {dmToggle.isOn}, asdToggle: {asdToggle.isOn}, hlToggle: {hlToggle.isOn}, isToggle:{isToggle.isOn}, cardiacToggle:{cardiacToggle.isOn}");

      Dictionary<string, bool> medicalHistory = new Dictionary<string, bool>
        {
            {"Hypertension", htToggle.isOn },
            {"Diabetes", dmToggle.isOn },
            {"ASD", asdToggle.isOn },
            {"Hyperlipid", hlToggle.isOn },
            {"Stroke", isToggle.isOn },
            {"Cardiac", cardiacToggle.isOn },
        };

      DataManager.Instance.UserInfo.MedicalHistory = medicalHistory;

      SceneManager.LoadScene(nextScene);
    }

    public void allSelection()
    {
      htToggle.isOn = false;
      dmToggle.isOn = false;
      asdToggle.isOn = false;
      hlToggle.isOn = false;
      isToggle.isOn = false;
      cardiacToggle.isOn = false;
    }
  }
}
