using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Manager;
using Constant;
using Sirenix.Utilities;

namespace Register
{
  public class RegisterManager : MonoBehaviour
  {
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    [Header("Register")]
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_Text usernameWarningText;
    public TMP_Text passwordWarningText;

    [Header("Destination")]
    public string targetScene;

    public void RegisterButton()
    {
      string username = usernameField.text.Trim();
      string password = passwordField.text.Trim();

      if (username.Length == 0)
      {
        usernameWarningText.text = ErrorMessageConstant.Register.EMAIL_EMPTY;
      }
      else if (password.Length == 0)
      {
        passwordWarningText.text = ErrorMessageConstant.Register.PASSWORD_EMPTY;
      }
      else
      {
        usernameWarningText.text = "";
        passwordWarningText.text = "";
        PreRegister(username);
      }
    }

    private async void PreRegister(string username)
    {
      string email = username + FirebaseConstant.EMAIL_SUFFIX;
      Debug.Log($"RegisterManager.PreRegister: Email {email}");

      if (await FirebaseManager.Instance.CheckIsEmailExisted(email))
      {
        usernameWarningText.text = ErrorMessageConstant.Register.EMAIL_ALREADY_EXIST;
      }
      else
      {
        DataManager.Instance.UserInfo.Username = username;
        DataManager.Instance.UserInfo.Password = passwordField.text;

        await Task.Delay(3000);

        SceneManager.LoadSceneAsync(targetScene);
      }
    }
  }
}