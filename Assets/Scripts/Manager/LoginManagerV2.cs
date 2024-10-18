using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManagerV2 : MonoBehaviour
{
    FirebaseManagerV2 fbm;
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_Text usernameWarn;
    public TMP_Text passwordWarn;
    public GameObject loadbg;

    public void LogInWithInfo()
    {
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
            return;
        }
        else
        {
            fbm.GetUser(usernameField.text.ToString().Contains("@") ? true : false, usernameField.text.ToString(), passwordField.text.ToString());
        }

    }

    public void LogInWithGoogleAccount()
    {
        loadbg.SetActive(true);
        fbm.FBMGoogleSignIn();
    }
    public void OnSuccessSignInWithGoogleAccount(Firebase.Auth.FirebaseUser reg_info)
    {
        fbm.GetUser(true, reg_info.Email, "");
    }
    public void OnVerifiedUser()
    {
        SceneManager.LoadScene("Tutorial");
    }
    // Start is called before the first frame update
    void Start()
    {
        fbm = FirebaseManagerV2.GetInstance();
        fbm.lm_instance = this;
    }
}
