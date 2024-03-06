using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    [Header("Login")]
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_Text usernameWarningText;
    public TMP_Text passwordWarningText;

    [Header("Destination")]
    public string targetScene;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
    }

    public void LoginButton()
    {
        string emailForm = usernameField.text.Trim();
        if (emailForm.Length == 0)
        {
            usernameWarningText.text = "*กรุณากรอกชื่อผู้ใช้";
        }
        else
        {
            usernameWarningText.text = "";
            StartCoroutine(Login(emailForm+"@tester.com", passwordField.text));
        }
    }

    private IEnumerator Login(string _username, string _password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_username, _password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null) 
        {
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorcode = (AuthError)firebaseEx.ErrorCode;

            string usernameMessage = ""; // This message will be shown at username box
            string passwordMessage = ""; // This message will be shown at password box
            switch (errorcode)
            {
                case AuthError.MissingEmail:
                    usernameMessage = "*กรุณากรอกชื่อผู้ใช้";
                    break;
                case AuthError.MissingPassword:
                    passwordMessage = "*กรุณากรอกรหัสผ่าน";
                    break;
                case AuthError.WrongPassword:
                    passwordMessage = "*รหัสผ่านไม่ถูกต้อง";
                    break;
                case AuthError.UserNotFound:
                    usernameMessage = "*ไม่พบชื่อผู้ใช้งาน";
                    break;
            }
            usernameWarningText.text = usernameMessage;
            passwordWarningText.text = passwordMessage;
        }
        else
        {
            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            usernameWarningText.text = "";
            passwordWarningText.text = "";

            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(targetScene);
        }
    }
}
