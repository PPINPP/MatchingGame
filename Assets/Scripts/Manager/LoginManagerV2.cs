using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manager;
using MatchingGame.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManagerV2 : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_Text usernameWarn;
    public TMP_Text passwordWarn;
    public GameObject loadbg;
    
    [Header("Login Method Button")]
    public Button googleButton;
    public Button facebookButton;
    public Button lineButton;

    public void LogInWithInfo()
    {
        loadbg.SetActive(true);
        if (usernameField.text.Count() < 1)
        {
            usernameWarn.text = "*กรุณากรอกชื่อผู้ใช้";
        }
        else
        {
            usernameWarn.text = "";
        }

        if (passwordField.text.Count() < 1)
        {
            passwordWarn.text = "*กรุณากรอกรหัสผ่าน";
        }
        else
        {
            passwordWarn.text = "";
        }

        if (passwordField.text.Count() < 1 || usernameField.text.Count() < 1)
        {
            loadbg.SetActive(false);
            return;
        }
        else
        {
            FirebaseManagerV2.Instance.GetUser(usernameField.text.ToString().Contains("@") ? true : false, usernameField.text.ToString(), passwordField.text.ToString(),OnVerifiedUser,OnFailedLogin);
        }

    }

    public void LogInWithGoogleAccount()
    {
        loadbg.SetActive(true);
        FirebaseManagerV2.Instance.FBMGoogleSignIn(OnSuccessSignInWithGoogleAccount,OnFailedSignInWithGoogleAccount);
    }
    public void OnSuccessSignInWithGoogleAccount(Firebase.Auth.FirebaseUser reg_info)
    {
        FirebaseManagerV2.Instance.GetUser(true, reg_info.Email, "",OnVerifiedUser,OnFailedLogin);
    }
    public void OnFailedSignInWithGoogleAccount(string error_log)
    {
        Debug.LogError(error_log);
    }
    public void OnVerifiedUser()
    {
        // SceneManager.LoadScene("Tutorial");
        GameObject dm = new GameObject("DataManager");
        dm.AddComponent<DataManager>();
        GameObject sm = new GameObject("SequenceManager");
        sm.AddComponent<SequenceManager>();

        SequenceManager.Instance.NextSequence();
    }
    // Start is called before the first frame update
    public void OnFailedLogin(){
        loadbg.SetActive(false);
        usernameWarn.text = "*ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง";
    }
    void Start()
    {
        #if UNITY_EDITOR
            googleButton.interactable = false;
            facebookButton.interactable = false;
            lineButton.interactable = false;
        #endif
    }
}
