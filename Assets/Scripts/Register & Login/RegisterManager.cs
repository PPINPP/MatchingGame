using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;

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

    public void RegisterButton()
    {
        string emailForm = usernameField.text.Trim();
        if (emailForm.Length == 0)
        {
            usernameWarningText.text = "*กรุณากรอกชื่อผู้ใช้";
        }
        else
        {
            usernameWarningText.text = "";
            StartCoroutine(Register(emailForm+"@tester.com", passwordField.text, emailForm));
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

        if (RegisterTask.Exception != null) 
        {
            Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
            FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
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
                case AuthError.WeakPassword:
                    passwordMessage = "*รหัสผ่านต้องยาว 6 ตัวขึ้นไป";
                    break;
                case AuthError.EmailAlreadyInUse:
                    usernameMessage = "*ชื่อผู้ใช้งานนี้มีอยู่แล้ว";
                    break;
            }
            usernameWarningText.text = usernameMessage;
            passwordWarningText.text = passwordMessage;
        }
        else
        {
            User = RegisterTask.Result.User;
            UserProfile profile = new UserProfile { DisplayName = _username };

            var ProfileTask = User.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

            if (ProfileTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                usernameWarningText.text = "Username Set Failed!";
            }
            else
            {
                usernameWarningText.text = "";
                passwordWarningText.text = "";
                yield return new WaitForSeconds(3);
                SceneManager.LoadScene(targetScene);
            }
        }
    }
}
