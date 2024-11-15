using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Constant;
using Enum;
using RegistryForm;
using System;
using Utils;
using System.Linq;
using Unity.VisualScripting;
using Google;
using UnityEngine.SceneManagement;


public class RegisterManagerV2 : MonoBehaviour
{
    [Header("First Page")]
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_Text usernameWarningText;
    public TMP_Text passwordWarningText;
    public GameObject firstPnext;

    [Header("Second Page")]
    public TMP_InputField DateField;
    public TMP_InputField MonthField;
    public TMP_InputField YearField;
    public GameObject MaleButton, FemaleButton;
    string gender = null;
    public GameObject secondPnext;

    [Header("Third Page")]
    public GameObject thirdPnext;
    string educationalLevel;
    private string[] buttonIndex = new string[6] { "MASTER_DEGREE", "BACHELOR_DEGREE", "DOCTORAL", "HIGH_SCHOOL", "TECHNICAL", "PRIMARY_OR_LOWER" };
    [Header("Fourth Page")]
    public GameObject fourthPnext;

    [Header("Fifth Page")]
    public GameObject fifthPnext;
    public GameObject noButton;
    public GameObject mciButton;
    public GameObject alzButton;
    string dementiaStage = null;

    [Header("Script Variable")]
    public GameObject registerScene;
    public GameObject loadbg;
    List<GameObject> pages = new List<GameObject>();
    int curr_page = 0;

    [Header("Login Method Button")]
    public Button googleButton;
    public Button facebookButton;
    public Button lineButton;

    private string global_uuid = "";
    private Firebase.Auth.FirebaseUser curr_user;
    private bool googlesignin_mode = false;


    //////////////////////FIRST PAGE//////////////////////////
    public void LoginInfoInput(GameObject obj)
    {
        if (obj.name == "UsernameField")
        {
            string username = usernameField.text.Trim();
            if (username.Length == 0)
            {
                usernameWarningText.text = ErrorMessageConstant.Register.EMAIL_EMPTY;
            }
            else
            {
                usernameWarningText.text = "";
            }
        }
        else if (obj.name == "PasswordField")
        {
            string password = passwordField.text.Trim();
            if (password.Length == 0)
            {
                passwordWarningText.text = ErrorMessageConstant.Register.PASSWORD_EMPTY;
            }
            else
            {
                passwordWarningText.text = "";
            }
        }
        else
        {
            //next on first page
            loadbg.SetActive(true);
            FirebaseManagerV2.Instance.RegisterUsernameCheck(usernameField.text.Trim(),OnResultUsernameCheck);
        }
        if (usernameField.text.Trim().Length > 0 && passwordField.text.Trim().Length > 0)
        {
            firstPnext.GetComponent<Button>().interactable = true;
        }
        else
        {
            firstPnext.GetComponent<Button>().interactable = false;
        }
    }
    public void OnResultUsernameCheck(bool result)
    {
        if (result)
        {
            NextPage();
        }
        else
        {
            usernameWarningText.text = "เกิดข้อผิดพลาด มีผู้ใช้งานในระบบแล้ว";
        }
        loadbg.SetActive(false);
    }
    //////////////////////////////////////////////////////////
    //////////////////////SECOND PAGE/////////////////////////
    public void SecondPageInputChange()
    {
        if (gender != null && DateField.text.Length > 0 && MonthField.text.Length > 0 && YearField.text.Length > 0)
        {
            secondPnext.GetComponent<Button>().interactable = true;
        }
        else
        {
            secondPnext.GetComponent<Button>().interactable = false;
        }
    }
    public void GenderSelect(string val)
    {
        gender = val;
        if (val == "MALE")
        {
            var maleButtonNormalColor = MaleButton.GetComponent<Button>().colors;
            var femaleButtonNormalColor = FemaleButton.GetComponent<Button>().colors;

            maleButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.SELECTED_COLOR); ;
            femaleButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.UNSELECTED_COLOR); ;

            MaleButton.GetComponent<Button>().colors = maleButtonNormalColor;
            FemaleButton.GetComponent<Button>().colors = femaleButtonNormalColor;
        }
        else
        {
            var maleButtonNormalColor = MaleButton.GetComponent<Button>().colors;
            var femaleButtonNormalColor = FemaleButton.GetComponent<Button>().colors;

            maleButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.UNSELECTED_COLOR); ;
            femaleButtonNormalColor.normalColor = new Color(1f, 1f, 1f, ButtonColorConstant.SELECTED_COLOR); ;

            MaleButton.GetComponent<Button>().colors = maleButtonNormalColor;
            FemaleButton.GetComponent<Button>().colors = femaleButtonNormalColor;
        }
        SecondPageInputChange();
    }
    //////////////////////////////////////////////////////////
    //////////////////////THIRD PAGE//////////////////////////
    public void EducationSelect(string level)
    {
        educationalLevel = level;
        thirdPnext.GetComponent<Button>().interactable = true;
        for (int i = 2; i < 8; i++)
        {
            if (i == Array.IndexOf(buttonIndex, level) + 2)
            {
                var normalColor = pages[2].transform.GetChild(i).GetComponent<Button>().colors;
                Color newColor = new Color(0.59f, 0.74f, 1f, 1f);
                normalColor.normalColor = newColor;
                normalColor.selectedColor = newColor;
                pages[2].transform.GetChild(i).GetComponent<Button>().colors = normalColor;
            }
            else
            {
                var normalColor = pages[2].transform.GetChild(i).GetComponent<Button>().colors;
                Color newColor = new Color(1f, 1f, 1f, 1f);
                normalColor.normalColor = newColor;
                normalColor.selectedColor = newColor;
                pages[2].transform.GetChild(i).GetComponent<Button>().colors = normalColor;
            }
        }
    }
    //////////////////////////////////////////////////////////
    //////////////////////FOURTH PAGE/////////////////////////
    public void FourthPageInputChange()
    {
        bool selectCheck = false;
        bool noneButton = false;
        for (int i = 4; i < 11; i++)
        {
            if (registerScene.transform.GetChild(3).GetChild(i).GetComponent<Toggle>().isOn == true)
            {
                selectCheck = true;
                if (i == 4 && selectCheck == true)
                {
                    noneButton = true;
                }
            }

        }
        if (noneButton)
        {
            for (int i = 5; i < 11; i++)
            {
                registerScene.transform.GetChild(3).GetChild(i).GetComponent<Toggle>().isOn = false;
                registerScene.transform.GetChild(3).GetChild(i).GetComponent<Toggle>().interactable = false;

            }
        }
        else
        {
            for (int i = 5; i < 11; i++)
            {
                registerScene.transform.GetChild(3).GetChild(i).GetComponent<Toggle>().interactable = true;
            }
        }
        if (selectCheck)
        {
            fourthPnext.GetComponent<Button>().interactable = true;
        }
        else
        {
            fourthPnext.GetComponent<Button>().interactable = false;
        }
    }


    //////////////////////////////////////////////////////////
    //////////////////////FIFTH PAGE/////////////////////////
    public void FifthPageInputChange(string val)
    {
        dementiaStage = val;
        fifthPnext.GetComponent<Button>().interactable = true;
        if (val == "HEALTHY")
        {

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
        else if (val == "ALZHEIMER")
        {
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
        else
        {
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
    public void SubmitForm()
    {
        loadbg.SetActive(true);
        string date = DateField.text;
        string month = MonthField.text;
        string year = YearField.text;
        string dateString = $"{date}/{month}/{int.Parse(year) - DateTimeConstant.BUDDHIST_ERA_YEAR}";
        bool[] mdch = new bool[] { false, false, false, false, false, false };
        for (int i = 5; i < 11; i++)
        {
            if (registerScene.transform.GetChild(3).GetChild(i).GetComponent<Toggle>().isOn == true)
            {
                mdch[i - 5] = true;
            }

        }
        // string[] ct = new string[] { "q_type", "fuzzy_type" }; // COMMENT OUT
        // Classify = ct[UnityEngine.Random.Range(0, 2)] Insert this below
        UserRegisterForm userInfo = new UserRegisterForm()
        {
            Username = usernameField.text.Trim().ToString().Contains("@") ? null : usernameField.text.Trim().ToString(),
            Password = passwordField.text.Trim().ToString(),
            Email = usernameField.text.Trim().ToString().Contains("@") ? usernameField.text.Trim().ToString() : null,
            Classify = null,
            DateOfBirth = dateString,
            Gender = gender,
            DementiaStage = dementiaStage,
            EducationalLevel = educationalLevel,
            Uuid = global_uuid == "" ? Guid.NewGuid().ToString() : curr_user.UserId,
            DateCreated = DateTime.Now.ToString("s"),
            DateUpdated = DateTime.Now.ToString("s"),
            MedicalHistory = new Dictionary<string, bool>
        {
            {"Hypertension", mdch[0] },
            {"Diabetes", mdch[1] },
            {"ASD", mdch[2] },
            {"Hyperlipid", mdch[3] },
            {"Stroke", mdch[4]},
            {"Cardiac", mdch[5]},
        }

        };
        FirebaseManagerV2.Instance.NewRegister(userInfo,OnCompleteRegister);
    }
    public void OnCompleteRegister()
    {
        if (googlesignin_mode)
        {
            FirebaseManagerV2.Instance.FBMGoogleSignOut();
        }
        IEnumerator WaitForSignOut()
        {
            yield return new WaitForSeconds(1);// Wait a bit
            SceneManager.LoadScene("Main_P");
        }
        StartCoroutine(WaitForSignOut());
    }
    ///////////////////////////////////////////////////////
    ////////////////////Page Manager///////////////////////
    public void NextPage()
    {
        curr_page++;
        UpdatePage();
    }
    public void PreviousPage()
    {
        curr_page--;
        UpdatePage();
    }
    void UpdatePage()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == curr_page)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }
        if (curr_page == 0)
        {
            if (googlesignin_mode)
            {
                usernameField.text = "";
                global_uuid = "";
                curr_user = null;
                googlesignin_mode = false;
                FirebaseManagerV2.Instance.FBMGoogleSignOut();
            }
        }
    }
    ////////////////////////Google Interface//////////////////////////////////
    public void GoogleSignInClick()
    {
        FirebaseManagerV2.Instance.FBMGoogleSignUp(OnSuccessRegisterWithGoogleAccount,OnFailedRegisterWithGoogleAccount);
    }
    public void OnSuccessRegisterWithGoogleAccount(Firebase.Auth.FirebaseUser reg_info)
    {
        usernameField.text = reg_info.Email.ToString();
        global_uuid = reg_info.UserId;
        curr_user = reg_info;
        googlesignin_mode = true;
        NextPage();
    }
    public void OnFailedRegisterWithGoogleAccount(string error_log)
    {
        Debug.LogError(error_log);
    }
    void Start()
    {
#if UNITY_EDITOR
        googleButton.interactable = false;
        facebookButton.interactable = false;
        lineButton.interactable = false;
#endif
        for (int i = 0; i < 5; i++)
        {
            pages.Add(registerScene.transform.GetChild(i).gameObject);

        }
        UpdatePage();
    }
}
