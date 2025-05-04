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
using Random = UnityEngine.Random;

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
            FirebaseManagerV2.Instance.GetUser(usernameField.text.ToString().Contains("@") ? true : false, usernameField.text.ToString(), passwordField.text.ToString(), OnVerifiedUser, OnFailedLogin);
        }

    }

    public void LogInWithGoogleAccount()
    {
        loadbg.SetActive(true);
        FirebaseManagerV2.Instance.FBMGoogleSignIn(OnSuccessSignInWithGoogleAccount, OnFailedSignInWithGoogleAccount);
    }
    public void OnSuccessSignInWithGoogleAccount(Firebase.Auth.FirebaseUser reg_info)
    {
        usernameField.text = reg_info.Email;
        FirebaseManagerV2.Instance.GetUser(true, reg_info.Email, "", OnVerifiedUser, OnFailedLogin);
    }
    public void OnFailedSignInWithGoogleAccount(string error_log)
    {
        Debug.LogError(error_log);
    }
    public void OnVerifiedUser()
    {
        //hfelab.come
        GameObject dm = new GameObject("DataManager");
        dm.AddComponent<DataManager>();
        GameObject sm = new GameObject("SequenceManager");
        sm.AddComponent<SequenceManager>();
        LevelSelectorManager.Instance.GetGameConponent();
        LevelSelectorManager.Instance.UpdateGameData();
        if (FirebaseManagerV2.Instance.gameData["TTR1"] == false )
        {
            SequenceManager.Instance.ReloadSequence(CreateTutorialSequence("1"));
        }
        else if (FirebaseManagerV2.Instance.gameData["TTR2"] == false)
        {
            SequenceManager.Instance.ReloadSequence(CreateTutorialSequence("2"));
        }
        else if (FirebaseManagerV2.Instance.gameData["TTR3"] == false)
        {
            SequenceManager.Instance.ReloadSequence(CreateTutorialSequence("3"));
        }
        else if (FirebaseManagerV2.Instance.gameData["TTR4"] == false)
        {
            SequenceManager.Instance.ReloadSequence(CreateTutorialSequence("4"));
        }
        else
        {
            GameplaySequenceSO gameplaySequenceSO = ScriptableObject.CreateInstance<GameplaySequenceSO>();
            var daily_key = "daily_check_" + FirebaseManagerV2.Instance.curr_username;
            if (PlayerPrefs.HasKey(daily_key))
            {
                if (DateTime.Now.Day != PlayerPrefs.GetInt(daily_key))
                {
                    gameplaySequenceSO.sequences.Add(new SequenceDetail()
                    {
                        isDailyFeeling = true,
                    });
                    PlayerPrefs.SetInt(daily_key, DateTime.Now.Day);
                    SequenceManager.Instance._test2mode = true;
                    SequenceManager.Instance.ReloadSequence(gameplaySequenceSO);
                    SequenceManager.Instance.NextSequence();
                    return;
                }
                else
                {
                    SceneManager.LoadScene("LevelSelector");
                    return;
                }
            }
            else
            {
                gameplaySequenceSO.sequences.Add(new SequenceDetail()
                {
                    isDailyFeeling = true,
                });
                PlayerPrefs.SetInt(daily_key, DateTime.Now.Day);
            }
            SequenceManager.Instance._test2mode = true;
            SequenceManager.Instance.ReloadSequence(gameplaySequenceSO);
            SequenceManager.Instance.NextSequence();
            return;
            // SequenceManager.Instance._test2mode = true;

        }
        // else if (FirebaseManagerV2.Instance.gameData["TTR4"] == false)
        // {
        //     SequenceManager.Instance.ReloadSequence(CreateTutorialSequence("4"));
        // }
        SequenceManager.Instance._test2mode = true;
        SequenceManager.Instance.NextSequence();
        return;
        // else
        // {
        //     SceneManager.LoadScene("LevelSelector");
        //     return;
        // }
        if (usernameField.text.ToString() == "hfelab.come")
        {
            SceneManager.LoadScene("SequenceScriptTester");
            return;
        }
        //REMOVE
        if (usernameField.text.ToString() == "levelselector")
        {
            SceneManager.LoadScene("LevelSelector");
            return;
        }

        // SceneManager.LoadScene("Tutorial");



    }
    GameplaySequenceSO CreateTutorialSequence(string start_seq)
    {
        var daily_key = "daily_check_" + FirebaseManagerV2.Instance.curr_username;
        GameplaySequenceSO gameplaySequenceSO = ScriptableObject.CreateInstance<GameplaySequenceSO>();
        for (int i = int.Parse(start_seq); i < 4; i++)
        {
            SequenceDetail sequenceDetail = new SequenceDetail()
            {
                stageID = "tutorial_" + i.ToString(),
                isGamePlay = true,
                gameplay = RandomTutorial(i == 1 ? i - 1 : i),
            };
            gameplaySequenceSO.sequences.Add(sequenceDetail);
        }
        SequenceManager.Instance._ttr4 = true;
        // SequenceDetail sequenceDetail = new SequenceDetail()
        // {
        //     stageID = "tutorial_4",
        //     isGamePlay = true,
        //     gameplay = RandomTutorial(0, false),
        // };
        gameplaySequenceSO.sequences.Add(new SequenceDetail()
        {
            stageID = "tutorial_4",
            isGamePlay = true,
            gameplay = RandomTutorial(0, false),
        });
        if (PlayerPrefs.HasKey(daily_key))
        {
            if (DateTime.Now.Day != PlayerPrefs.GetInt(daily_key))
            {
                gameplaySequenceSO.sequences.Add(new SequenceDetail()
                {
                    isDailyFeeling = true,
                });
                PlayerPrefs.SetInt(daily_key, DateTime.Now.Day);
            }
        }
        else
        {
            gameplaySequenceSO.sequences.Add(new SequenceDetail()
            {
                isDailyFeeling = true,
            });
            PlayerPrefs.SetInt(daily_key, DateTime.Now.Day);
        }
        return gameplaySequenceSO;
        // gameplaySequenceSO.ReloadSequence(gameplaySequenceSO);
    }

    GameplaySequenceSetting RandomTutorial(int difficulty, bool bypass_tutorial = true)
    {
        GameplaySequenceSetting gameplaySequenceSetting = new GameplaySequenceSetting();
        gameplaySequenceSetting.isTutorial = bypass_tutorial;
        gameplaySequenceSetting.categoryTheme = (CategoryTheme)Random.Range(0, 4);
        gameplaySequenceSetting.pairType = PairType.TWO;
        gameplaySequenceSetting.GameDifficult = (GameDifficult)difficulty;
        gameplaySequenceSetting.layout = GameLayout.GRID;
        gameplaySequenceSetting.isForceCardID = true;
        var all_card = new List<string>();
        if ((int)gameplaySequenceSetting.GameDifficult == 0)
        {
            all_card.Add(RandomTutorialCard(1, 51, gameplaySequenceSetting.categoryTheme.ToString()));
            string temp_card = "";
            do
            {
                temp_card = RandomTutorialCard(1, 51, gameplaySequenceSetting.categoryTheme.ToString());
            } while (all_card.Contains(temp_card));
            all_card.Add(temp_card);
        }
        else if ((int)gameplaySequenceSetting.GameDifficult == 1)
        {
            all_card.Add(RandomTutorialCard(51, 100, gameplaySequenceSetting.categoryTheme.ToString()));
            string temp_card = "";
            do
            {
                temp_card = RandomTutorialCard(51, 100, gameplaySequenceSetting.categoryTheme.ToString());
            } while (all_card.Contains(temp_card));
            all_card.Add(temp_card);
        }
        else
        {
            all_card.Add(RandomTutorialCard(1, 100, gameplaySequenceSetting.categoryTheme.ToString()));
            string temp_card = "";
            do
            {
                temp_card = RandomTutorialCard(1, 100, gameplaySequenceSetting.categoryTheme.ToString());
            } while (all_card.Contains(temp_card));
            all_card.Add(temp_card);
        }
        gameplaySequenceSetting.cardIDList = all_card;
        return gameplaySequenceSetting;
    }

    string RandomTutorialCard(int startr, int stopr, string cardType)
    {
        return FirstCharToUpper(cardType + "_0") + Random.Range(startr, stopr).ToString("D3");
    }
    string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

    public void OnFailedLogin()
    {
        loadbg.SetActive(false);
        usernameWarn.text = "*ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง";
    }
    void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        googleButton.interactable = false;
        facebookButton.interactable = false;
        lineButton.interactable = false;
#endif
    }
}
