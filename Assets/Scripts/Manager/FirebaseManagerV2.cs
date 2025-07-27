using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Firebase;
using Firebase.Firestore;
using Firebase.Auth;
using Firebase.Extensions;
using RegistryForm;
using System;
using System.Threading.Tasks;
// using Google;
using Manager;
using Unity.VisualScripting;
using Model;
using System.Data.OleDb;
using System.Configuration;
using UniRx;
using System.Linq;
using Enum;
using Experiment;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class FirebaseManagerV2 : MonoSingleton<FirebaseManagerV2>
{
    /// Class Parameter ///
    private FirebaseApp app;
    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private FirebaseUser user;

    // public string GoogleAPI = "415072983245-jbn838hn0mhq1s9h9t2cq8i67steeejl.apps.googleusercontent.com";
    // private GoogleSignInConfiguration configuration;
    private long _cacheSize = 314572800; //Default = 104857600 : New = 314572800

    /// Profile Parameter ///
    private bool syncnetwork = true;
    string curr_id;
    string prefix_locate = "q_test";
    // string prefix_locate = "Debug";
    string prefix_time_locate = "game_information_test";
    public string curr_username;
    public bool isFirstLogin;
    public string lastLogin;
    public bool passTutorial { get; set; }
    public Dictionary<string, List<string>> cardList = new Dictionary<string, List<string>>();
    public Dictionary<string, bool> gameData = new Dictionary<string, bool>();
    public Dictionary<string, List<int>> gameState = new Dictionary<string, List<int>>();
    public Dictionary<string, List<int>> gameScore = new Dictionary<string, List<int>>();
    public Dictionary<string, List<string>> week_day = new Dictionary<string, List<string>>();
    public int curr_week = 1;
    public List<string> timeRules = new List<string>();
    /// This will be reset on signout ///
    public override void Init()
    {
        base.Init();
        if (syncnetwork)
        {
            FirebaseFirestore.DefaultInstance.EnableNetworkAsync();
        }
        else
        {
            FirebaseFirestore.DefaultInstance.DisableNetworkAsync();
        }

        // #if UNITY_ANDROID || UNITY_EDITOR || UNITY_IOS
        //         var configuration = new GoogleSignInConfiguration
        //         {
        //             WebClientId = GoogleAPI,
        //             RequestIdToken = true,
        //         };
        // GoogleSetConfiguration();
        // #endif

        InitializeApp();
        SetParameter();
    }
    public void SetParameter()
    {
        curr_username = "";
        curr_id = "";
        curr_week = 1;
        cardList.Clear();
        gameData.Clear();
        timeRules.Clear();
        gameScore.Clear();
        gameState.Clear();
        week_day.Clear();
        cardList.Add("HOME", new List<string>());
        cardList.Add("MARKET", new List<string>());
        cardList.Add("STORE", new List<string>());
        cardList.Add("CLOTH", new List<string>());
        gameData.Add("TTR1", false);
        gameData.Add("TTR2", false);
        gameData.Add("TTR3", false);
        gameData.Add("TTR4", false);
        gameData.Add("PASSIVE", false);
        for (int i = 1; i < 9; i++)
        {
            gameScore["W" + i.ToString()] = new List<int>();
            gameState["W" + i.ToString()] = new List<int>();
        }
        for (int i = 0; i < 8; i++)
        {
            week_day.Add((i + 1).ToString(), new List<string>());
        }
        //FuzzyBrain.Instance.ClearParameter();
        QBrain.Instance.ClearParameter();

        //FIX
    }
    public void UserLogout()
    {
        SetParameter();
        SceneManager.LoadScene("Main_P");
        PlayerPrefs.SetString("autologinname", "");
        PlayerPrefs.SetString("autologinpassword", "");
    }

    // void GoogleSetConfiguration()
    // {
    //     GoogleSignIn.Configuration = configuration;
    //     GoogleSignIn.Configuration.UseGameSignIn = false;
    //     GoogleSignIn.Configuration.RequestIdToken = true;
    //     GoogleSignIn.Configuration.RequestEmail = true;
    // }
    public void FBMGoogleSignUp(Action<Firebase.Auth.FirebaseUser> success, Action<string> failed)
    {
        // Firebase.Auth.Credential credential =
        //     Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
        // auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task =>
        // {
        //     if (task.IsCanceled)
        //     {
        //         Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
        //         return;
        //     }
        //     if (task.IsFaulted)
        //     {
        //         Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
        //         return;
        //     }

        //     Firebase.Auth.AuthResult result = task.Result;
        //     Debug.LogFormat("User signed in successfully: {0} ({1})",
        //         result.User.DisplayName, result.User.UserId);
        // });

        // GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
        // {
        //     if (task.IsFaulted)
        //     {
        //         Debug.LogError("Faulted");
        //     }
        //     else if (task.IsCanceled)
        //     {
        //         Debug.LogError("Cancelled");
        //     }
        //     else
        //     {
        //         Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
        //         auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        //         {
        //             if (task.IsCanceled)
        //             {
        //                 failed?.Invoke("Task cancaled");
        //             }

        //             if (task.IsFaulted)
        //             {
        //                 failed?.Invoke("SignInWithCredentialAsync encountered an error: " + task.Exception);
        //             }

        //             user = auth.CurrentUser;
        //             success?.Invoke(user);
        //         });
        //     }
        // });
    }
    public void FBMGoogleSignIn(Action<Firebase.Auth.FirebaseUser> success, Action<string> failed)
    {
        // GoogleSetConfiguration();
        // GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
        // {
        //     if (task.IsFaulted)
        //     {
        //         Debug.LogError("Faulted");
        //     }
        //     else if (task.IsCanceled)
        //     {
        //         Debug.LogError("Cancelled");
        //     }
        //     else
        //     {
        //         Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

        //         auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        //         {
        //             if (task.IsCanceled)
        //             {
        //                 failed?.Invoke("Cancel");
        //             }

        //             if (task.IsFaulted)
        //             {
        //                 failed?.Invoke("SignInWithCredentialAsync encountered an error: " + task.Exception);
        //             }

        //             user = auth.CurrentUser;
        //             curr_id = user.UserId;


        //             success?.Invoke(user);
        //         });
        //     }
        // });
    }
    public void FBMGoogleSignOut()
    {
        auth.SignOut();
    }
    public void RegisterUsernameCheck(string uname, Action<bool> callback)
    {
        string[] lType = new string[] { "Username", "Email" };
        Query capitalQuery = db.Collection(prefix_locate).WhereEqualTo(lType[uname.Contains("@") ? 1 : 0], uname);
        capitalQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot capitalQuerySnapshot = task.Result;
            if (capitalQuerySnapshot.Count > 0)
            {
                callback?.Invoke(false);
            }
            else
            {
                callback?.Invoke(true);
            }

        });
    }
    void InitializeApp()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                app = Firebase.FirebaseApp.DefaultInstance;
                Debug.Log("Initialze App Successfully");

                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                Debug.Log("Initialize Authenticator Successfully");

                db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Initialize Database Successfully");
                // Debug.Log(db.Settings.CacheSizeBytes);
                // if (db.Settings.CacheSizeBytes != _cacheSize)
                // {
                //     db.Settings.CacheSizeBytes = _cacheSize;
                // }
                // Debug.Log(db.Settings.CacheSizeBytes);
                // FirebaseFirestore.GetInstance(app).Settings.CacheSizeBytes = _cacheSize;

            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }
    public void NewRegister(UserRegisterForm form, Action callback)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(form.Uuid);
        docRef.SetAsync(form);
        callback?.Invoke();
    }
    public void GetUser(bool loginType, string nkey, string pkey, Action success, Action failed)
    {
        string[] lType = new string[] { "Username", "Email" };
        Query capitalQuery = db.Collection(prefix_locate).WhereEqualTo(lType[loginType ? 1 : 0], nkey).WhereEqualTo("Password", pkey);
        capitalQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot capitalQuerySnapshot = task.Result;
            if (capitalQuerySnapshot.Count == 1)
            {
                foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
                {
                    curr_id = documentSnapshot.Id;
                    StartCoroutine(GetUserGameData());
                    Dictionary<string, object> fieldVal = documentSnapshot.ToDictionary();
                    int dp = 0;
                    List<int> FuzzyProperties = new List<int>();
                    List<int> QProperties = new List<int>();
                    List<int> CompleteGameID = new List<int>();
                    List<int> QCompleteGameID = new List<int>();
                    curr_username = fieldVal["Username"].ToString();
                    dp = Convert.ToInt32(fieldVal["DayPassed"]);
                    isFirstLogin = Convert.ToBoolean(fieldVal["IsFirstLogin"]);
                    lastLogin = fieldVal["LastLogin"].ToString();
                    PlayerPrefs.SetString("autologinname", fieldVal["Username"].ToString());
                    PlayerPrefs.SetString("autologinpassword", fieldVal["Password"].ToString());
                    if (documentSnapshot.TryGetValue<Dictionary<string, object>>("WeekDays", out var rawDict))
                    {
                        foreach (var entry in rawDict)
                        {
                            if (entry.Value is List<object> objList)
                            {
                                List<string> strList = objList.ConvertAll(obj => obj.ToString());
                                week_day[entry.Key] = strList;
                            }
                        }
                        foreach (var kvp in week_day)
                        {
                            Debug.Log($"Key: {kvp.Key}, Value: {string.Join(", ", kvp.Value)}");
                        }
                    }
                    // if (documentSnapshot.TryGetValue("FuzzyProperties", out FuzzyProperties))
                    // {
                    //     if (documentSnapshot.TryGetValue("CompleteGameID", out CompleteGameID))
                    //     {
                    //         if (CompleteGameID.Count > 0)
                    //         {
                    //             GetFuzzyGameData(CompleteGameID);
                    //         }
                    //         else
                    //         {
                    //             CompleteGameID = new List<int>();
                    //         }
                    //
                    //     }
                    //     FuzzyBrain.Instance.SetGameProperties(FuzzyProperties, CompleteGameID, dp);
                    // }
                    if (documentSnapshot.TryGetValue("QProperties", out QProperties))
                    {
                        if (documentSnapshot.TryGetValue("QCompleteGameID", out QCompleteGameID))
                        {
                            if (QCompleteGameID.Count > 0)
                            {
                                GetQGameData(QCompleteGameID, QProperties[2]);
                            }
                            else
                            {
                                QCompleteGameID = new List<int>();
                            }

                        }
                        QBrain.Instance.SetGameProperties(QProperties, QCompleteGameID, dp);
                    }
                    if (documentSnapshot.TryGetValue<List<QTableFs>>("QTable", out var QTableList))
                    {
                        if (QTableList.Count > 0)
                            QBrain.Instance.QTableList = QTableList.Select(s => new QTable()
                            {
                                GameplayState =
                                    (QGameplayState)System.Enum.Parse(typeof(QGameplayState), s.GameplayState),
                                CardNumberIncreaseQValue = s.CardNumberIncreaseQValue,
                                CardNumberMaintainQValue = s.CardNumberMaintainQValue,
                                CardNumberDecreaseQValue = s.CardNumberDecreaseQValue,
                                ChangeGameDifficultQValue = s.ChangeGameDifficultQValue,
                                ChangeGridModeQValue = s.ChangeGridModeQValue
                            }).ToList();
                        else
                        {
                            QBrain.Instance.SetDefaultQTable();
                        }
                    }
                    else
                    {
                        QBrain.Instance.SetDefaultQTable();
                    }
                    
                    GetSpecialGameData();
                    GameRuleTimeChecker(success);
                }
                return;
            }
            else if (capitalQuerySnapshot.Count == 0)
            {
                Debug.Log("Username doesn't exist or Password is wrong");
                failed?.Invoke();
                return;
            }
            else
            {
                Debug.Log("Something went wrong");
                failed?.Invoke();
                return;
            }
        });
        return;
    }
    // public void GetFuzzyGameData(List<int> CompleteGameID)
    // {
    //     List<object> _tempIDList = new List<object>();
    //     if (CompleteGameID.Count >= 5)
    //     {
    //         for (int i = CompleteGameID.Count - 5; i < CompleteGameID.Count; i++)
    //         {
    //             _tempIDList.Add(CompleteGameID[i].ToString());
    //         }
    //
    //     }
    //     else
    //     {
    //         for (int i = 0; i < CompleteGameID.Count; i++)
    //         {
    //             _tempIDList.Add(CompleteGameID[i].ToString());
    //         }
    //
    //     }
    //     Query GameDataQuery = db.Collection(prefix_locate + "/" + curr_id + "/FuzzyGameData").WhereIn("GameID", _tempIDList);
    //     GameDataQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
    //     {
    //         QuerySnapshot capitalQuerySnapshot = task.Result;
    //
    //         if (capitalQuerySnapshot.Count == _tempIDList.Count)
    //         {
    //             foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
    //             {
    //                 FuzzyBrain.Instance.UserFuzzyData.Add(new FuzzyGameData().ConvertToGameData(documentSnapshot.ConvertTo<FuzzyGameDataFs>()));
    //             }
    //             return;
    //         }
    //         else
    //         {
    //             Debug.Log("Something went wrong");
    //             return;
    //         }
    //
    //     });
    //     return;
    // }
    
    public void GetQGameData(List<int> CompleteGameID,int lastGameID)
    {
        List<object> _tempIDList = new List<object>();
        if (CompleteGameID.Count >= 3)
        {
            for (int i = CompleteGameID.Count - 3; i < CompleteGameID.Count; i++)
            {
                _tempIDList.Add(CompleteGameID[i].ToString());
            }

        }
        else
        {
            for (int i = 0; i < CompleteGameID.Count; i++)
            {
                _tempIDList.Add(CompleteGameID[i].ToString());
            }

        }
        Query GameDataQuery = db.Collection(prefix_locate + "/" + curr_id + "/QLog").WhereIn("GameID", _tempIDList);
        GameDataQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot capitalQuerySnapshot = task.Result;

            if (capitalQuerySnapshot.Count == _tempIDList.Count)
            {
                foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
                {
                    var data = new QLogResult().ConvertToGameData(documentSnapshot.ConvertTo<QLogResultFs>());
                    QBrain.Instance.UserQLogCompleteData.Add(data);
                }
            }
            else
            {
                Debug.Log("Something went wrong");
                return;
            }

        });
        Query LastGameDataQuery = db.Collection(prefix_locate + "/" + curr_id + "/QLog").WhereEqualTo("GameID", lastGameID.ToString());
        LastGameDataQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot capitalQuerySnapshot = task.Result;

            if (capitalQuerySnapshot != null && capitalQuerySnapshot.Count > 0)
            {
                foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
                {
                    var data = new QLogResult().ConvertToGameData(documentSnapshot.ConvertTo<QLogResultFs>());
                    if (QBrain.Instance.LastUserQLogResult == null || data.GameID == lastGameID.ToString())
                    {
                        QBrain.Instance.LastUserQLogResult = data;
                    }
                }
                return;
            }
            else
            {
                Debug.Log("Something went wrong");
                return;
            }

        });
        return;
    }

    public void GetSpecialGameData()
    {
        Query SpecialDataQuery = db.Collection(prefix_locate + "/" + curr_id + "/SpecialGameData");
        SpecialDataQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot capitalQuerySnapshot = task.Result;
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                //FuzzyBrain.Instance.UserSpecialData.Add(new SpecialFuzzyData().ConvertToGameData(documentSnapshot.ConvertTo<SpecialFuzzyDataFs>()));
                QBrain.Instance.UserSpecialData.Add(new SpecialFuzzyData().ConvertToGameData(documentSnapshot.ConvertTo<SpecialFuzzyDataFs>()));
            }
            return;
        });
        return;
    }
    public void SaveCard(string cardType, List<string> cardNames)
    {
        foreach (var item in cardNames)
        {
            if (!cardList[cardType].Contains(item))
            {
                cardList[cardType].Add(item);
            }
        }
        int use_week = 0;
        if (curr_week % 2 == 0)
        {
            use_week = curr_week - 1;
        }
        else
        {
            use_week = curr_week;
        }
        DocumentReference docRef = db.Collection(prefix_locate + "/" + curr_id + "/GameDataInformation/W" + use_week.ToString() + "/CardLog").Document(cardType);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Dictionary<string, object> snapshotData = snapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in snapshotData)
                {
                    if (pair.Key == "AllCard")
                    {
                        if (pair.Value is List<object> objList)
                        {
                            List<string> stringList = objList.Cast<string>().ToList();
                            foreach (var str in stringList)
                            {
                                if (!cardNames.Contains(str))
                                {
                                    cardNames.Add(str);

                                }

                            }
                        }
                    }
                }
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
        { "AllCard", cardNames },
};
                docRef.UpdateAsync(updates);

            }
            else
            {
                Dictionary<string, object> updates = new Dictionary<string, object>
{
        { "AllCard", cardNames },
};
                docRef.SetAsync(updates);
            }
        });

    }
    private IEnumerator GetCard()
    {
        int use_week = 0;
        if (curr_week % 2 == 0)
        {
            use_week = curr_week - 1;
        }
        else
        {
            use_week = curr_week;
        }
        Query allCardQuery = db.Collection(prefix_locate + "/" + curr_id + "/GameDataInformation/W" + use_week.ToString() + "/CardLog");

        Task<QuerySnapshot> task = allCardQuery.GetSnapshotAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError($"Error querying Firestore: {task.Exception.Message}");
        }
        else if (task.Result != null)
        {
            QuerySnapshot allCardQuerySnapshot = task.Result;
            foreach (DocumentSnapshot documentSnapshot in allCardQuerySnapshot.Documents)
            {
                Dictionary<string, object> data = documentSnapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in data)
                {
                    if (pair.Key == "AllCard")
                    {
                        if (pair.Value is List<object> objList)
                        {
                            List<string> stringList = objList.Cast<string>().ToList();
                            foreach (var str in stringList)
                            {
                                cardList[documentSnapshot.Id].Add(str);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("No data found.");
        }
    }

    private IEnumerator GetUserGameData()
    {
        DocumentReference gameDataRef = db.Collection(prefix_locate + "/" + curr_id + "/GameDataInformation").Document("Tutorial-State");

        Task<DocumentSnapshot> task = gameDataRef.GetSnapshotAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            // Debug.LogError($"Error querying Firestore: {task.Exception.Message}");
            CreateTTRState();
        }
        else if (task.Result != null)
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Dictionary<string, object> data = snapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in data)
                {
                    gameData[pair.Key] = (bool)pair.Value;
                }
            }
            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
                List<string> keysCopy = new List<string>(gameData.Keys);
                foreach (var key in keysCopy)
                {
                    gameData[key] = false;
                }
                CreateTTRState();
            }
        }
        else
        {
            Debug.Log("No data found.");
        }
    }
    private void CreateTTRState()
    {
        DocumentReference docRef = db.Collection(prefix_locate + "/" + curr_id + "/GameDataInformation").Document("Tutorial-State");
        Dictionary<string, bool> docData = new Dictionary<string, bool>
        {
            { "TTR1", false },
            { "TTR2", false },
            { "TTR3", false },
            { "TTR4", false },
            { "PASSIVE", false },
        };
        docRef.SetAsync(docData);
    }
    public void SetTutorial(bool isPass)
    {
        DocumentReference userRef = db.Collection(prefix_locate).Document(curr_id + "/GameDataInformation/Tutorial-State");
        db.RunTransactionAsync(transaction =>
            {
                return transaction.GetSnapshotAsync(userRef).ContinueWith((snapshotTask) =>
                {
                    DocumentSnapshot snapshot = snapshotTask.Result;
                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                { "PASSIVE", isPass}
                    };
                    transaction.Update(userRef, updates);
                });
            });
    }
    public void SaveTutorialUserGameData(string fkey, bool fval)
    {
        DocumentReference dataRef = db.Collection(prefix_locate + "/" + curr_id + "/GameDataInformation").Document("Tutorial-State");
        Dictionary<string, object> updates = new Dictionary<string, object>
{
    { "TTR" + fkey, fval }
};

        dataRef.UpdateAsync(updates);
    }
    private IEnumerator GetGameState(Action success)
    {
        string doc_name = "W" + curr_week.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd");
        DocumentReference gameDataRef = db.Collection(prefix_locate + "/" + curr_id + "/GameDataInformation").Document(doc_name);

        Task<DocumentSnapshot> task = gameDataRef.GetSnapshotAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            // Debug.LogError($"Error querying Firestore: {task.Exception.Message}");
            CreateWeekUserGameData(doc_name);
            success?.Invoke();
        }
        else if (task.Result != null)
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                gameState[doc_name] = new List<int>();
                gameScore[doc_name] = new List<int>();
                Dictionary<string, object> data = snapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in data)
                {
                    if (pair.Key == "game_state")
                    {
                        if (pair.Value is List<object> objList)
                        {
                            List<string> intList = objList.Cast<string>().ToList();
                            foreach (var state in intList)
                            {
                                gameState[doc_name].Add(int.Parse(state));
                            }
                        }
                    }
                    if (pair.Key == "game_score")
                    {
                        if (pair.Value is List<object> objList)
                        {
                            List<string> intList = objList.Cast<string>().ToList();
                            foreach (var state in intList)
                            {
                                gameScore[doc_name].Add(int.Parse(state));
                            }
                        }
                    }
                }
                success?.Invoke();
            }
            else
            {
                CreateWeekUserGameData(doc_name);
                success?.Invoke();
            }
        }
        else
        {
            Debug.Log("No data found.");
        }

    }
    public void CompareTime(List<string> timeRule)
    {
        DateTime currentDateTime = DateTime.Now.Date;
        List<DateTime> tempDateTime = new List<DateTime>();
        foreach (var item in timeRule)
        {
            tempDateTime.Add(DateTime.ParseExact(item, "yyyyMMdd", null));
        }
        if (currentDateTime > tempDateTime[^1])
        {
            Debug.Log("EndTestingTime");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        else
        {
            curr_week = (int)Mathf.Floor(tempDateTime.IndexOf(currentDateTime) / 7) + 1;
        }
        StartCoroutine(GetCard());
    }

    public void checkTimeChange()
    {
        if (lastLogin != DateTime.Now.ToString("yyyyMMdd"))
        {
            Debug.Log("End Testing Time or Time Change");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        //FIX
    }
    public async void GameRuleTimeChecker(Action success)
    {
        if (isFirstLogin)
        {
            isFirstLogin = false;
            UpdateFirstLogin(isFirstLogin);
            CompareTime(CreateGameRules());
            StartCoroutine(GetGameState(success));
        }
        else
        {
            DocumentReference gameRuleRef = db.Collection(prefix_locate + "/" + curr_id + "/" + "GameRules").Document("game_rules");
            using (var cts = new CancellationTokenSource())
            {
                cts.CancelAfter(TimeSpan.FromSeconds(10));
                try
                {
                    var snapshot = await gameRuleRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                            {
                                if (task.IsCanceled)
                                {
                                    return null;
                                }
                                if (task.IsFaulted)
                                {
                                    Debug.Log("You're Offline. The App may exit");
#if UNITY_EDITOR
                                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                                    Application.Quit();
                                    return null;
                                }
                                return task.Result;
                            });
                    if (snapshot != null && snapshot.Exists)
                    {
                        List<string> stringList = new List<string>();
                        Dictionary<string, object> data = snapshot.ToDictionary();
                        foreach (KeyValuePair<string, object> pair in data)
                        {
                            if (pair.Value is List<object> objList)
                            {
                                List<string> tempStringList = objList.Cast<string>().ToList();
                                foreach (var item in tempStringList)
                                {
                                    stringList.Add(item);
                                    timeRules.Add(item.ToString());
                                }

                            }
                        }
                        CompareTime(stringList);
                        StartCoroutine(GetGameState(success));
                    }
                    else
                    {
                        Debug.LogWarning("Document does not exist or timeout occurred.");
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("You're Offline. The App may exit");
                    Application.Quit();
                    return;
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }
        }


    }

    // public void GetFuzzyGameData(){
    //     DocumentReference docRef = db.Collection(prefix_locate + "/" + curr_id + "/GameDataInformation").Document("FuzzyData");
    //     docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
    //     {
    //         DocumentSnapshot snapshot = task.Result;
    //         if (snapshot.Exists)
    //         {
    //         }
    //     });
    // }


    public void CreateWeekUserGameData(string fkey)
    {
        gameState[fkey] = new List<int>() { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        gameScore[fkey] = new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        DocumentReference dataRef = db.Collection(prefix_locate + "/" + curr_id + "/GameDataInformation").Document(fkey);
        Dictionary<string, object> updates = new Dictionary<string, object>
{
    { "game_score", new List<object>(){"1","1","1","1","1","1","1","1","1","1","1"} },
    { "game_state", new List<object>(){"1","0","0","0","0","0","0","0","0","0","0"} }
};
        dataRef.SetAsync(updates).ContinueWithOnMainThread(task =>
        {
            Debug.Log(
                "Updated " + fkey + " State.");
        });
    }

    public List<string> CreateGameRules()
    {
        List<object> dateList = new List<object>();
        DateTime today = DateTime.Now;

        for (int i = 0; i < 56; i++)
        {
            string formattedDate = today.AddDays(i).ToString("yyyyMMdd");
            dateList.Add(formattedDate);
        }
        DocumentReference gameRuleRef = db.Collection(prefix_locate + "/" + curr_id + "/" + "GameRules").Document("game_rules");
        Dictionary<string, object> updates = new Dictionary<string, object>
{
    { "DateTime", dateList },
};
        gameRuleRef.SetAsync(updates);
        List<string> dateListString = new List<string>();
        foreach (var item in dateList)
        {
            dateListString.Add(item.ToString());
            timeRules.Add(item.ToString());
        }
        return dateListString;
    }


    public void UploadGameStateAndGameScore(List<int> game_state, List<int> game_score)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/GameDataInformation/W" + curr_week.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd"));
        List<string> stateList = game_state.Select(number => number.ToString()).ToList();
        List<string> scoreList = game_score.Select(number => number.ToString()).ToList();
        Dictionary<string, object> updates = new Dictionary<string, object>
{
    { "game_score", scoreList },
    { "game_state", stateList }
};
        docRef.UpdateAsync(updates);
    }
    public void UploadMiniGameResult(MinigameResult mini_result, int idnum)
    {
        // ConvertToFirestoreModel()
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/SpecialTask/task" + idnum.ToString("00") + "_" + mini_result.CompletedAt.ToString("s"));
        _ = docRef.SetAsync(mini_result.ConverToFirestoreModel());
    }
    public void UploadGamePlayResult(GamePlayResult game_result)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/GameplayLog/" + game_result.StageID + "_" + game_result.CompletedAt.ToString("s"));
        _ = docRef.SetAsync(game_result.ConvertToFirestoreModel());
    }
    public void UploadSmileyoMeterResult(SmileyoMeterResult smile_result, int idnum)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/SmileyoMeter/task_" + DateTime.Now.ToString("s"));
        _ = docRef.SetAsync(smile_result.ConvertSmileyoMeterResultToSmileyoMeterResultFs());
    }
    public void UploadTutorialResult(GamePlayResult tutorial_result)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/TutorialLog/" + tutorial_result.StageID + "_" + tutorial_result.CompletedAt.ToString("s"));
        _ = docRef.SetAsync(tutorial_result.ConvertToFirestoreModel());
    }
    public void UploadDailyFeelingResult(DailyFeelingResult daily_result)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/DailyFeeling/task_" + DateTime.Now.ToString("s"));
        _ = docRef.SetAsync(daily_result.ConvertDailyFeelingResultToDailyFeelingResultFs());
    }
    public void UploadFuzzyGameData(FuzzyGameData fuzzy_result)
    {
        DocumentReference dataRef = db.Collection(prefix_locate + "/" + curr_id + "/FuzzyGameData").Document(fuzzy_result.GameID);
        _ = dataRef.SetAsync(fuzzy_result.ConvertToFirestoreModel());
    }
    public void UploadSpecialGameData(SpecialFuzzyData special_result)
    {
        DocumentReference dataRef = db.Collection(prefix_locate + "/" + curr_id + "/SpecialGameData").Document(special_result.GameID);
        _ = dataRef.SetAsync(special_result.ConvertToFirestoreModel());
    }
    
    public void UploadGameQLearningData(QLogResult qLog)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/QLog/" + qLog.GameID);
        _ = docRef.SetAsync(qLog.ConvertToFirestoreModel());
    }
    
    public void UpdateQValue(QLogResult qLog)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/QLog/" + qLog.GameID);
        Dictionary<string, object> updates = new Dictionary<string, object>{
            {"QValue",qLog.QValue},
        };
        _ = docRef.UpdateAsync(updates);
    }
    
    public void UpdateFuzzyPostGameStage(List<int> fuzzyProp, List<int> completeGameID)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id);
        Dictionary<string, object> updates = new Dictionary<string, object>{
            {"FuzzyProperties",fuzzyProp},
            {"CompleteGameID",completeGameID}
        };
        docRef.UpdateAsync(updates);
    }
    
    public void UpdateQPostGameStage(List<int> qProp, List<int> completeGameID)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id);
        Dictionary<string, object> updates = new Dictionary<string, object>{
            {"QProperties",qProp},
            {"QCompleteGameID",completeGameID}
        };
        docRef.UpdateAsync(updates);
    }
    
    public void UpdateFirstLogin(bool val)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id);
        Dictionary<string, object> updates = new Dictionary<string, object>{
            {"IsFirstLogin",val},
        };
        docRef.UpdateAsync(updates);
    }

    public void UpdateLastLogin()
    {
        lastLogin = DateTime.Now.ToString("yyyyMMdd");
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id);
        Dictionary<string, object> updates = new Dictionary<string, object>{
            {"LastLogin", lastLogin},
        };
        docRef.UpdateAsync(updates);
    }
    public void UpdateWeekDays()
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id);
        Dictionary<string, object> updates = new Dictionary<string, object>{
            {"WeekDays", week_day},
        };
        docRef.UpdateAsync(updates);
    }

    public void UpdateDayPassed(int dp)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id);
        Dictionary<string, object> updates = new Dictionary<string, object>{
            {"DayPassed",dp}
        };
        docRef.UpdateAsync(updates);
    }
    
    public void UpdateQTable(List<QTable> QTableList)
    {
        List<QTableFs> QTableFs = QTableList.Select(s => new QTableFs
        {
            GameplayState = s.GameplayState.ToString(),
            CardNumberIncreaseQValue = s.CardNumberIncreaseQValue,
            CardNumberMaintainQValue = s.CardNumberMaintainQValue,
            CardNumberDecreaseQValue = s.CardNumberDecreaseQValue,
            ChangeGameDifficultQValue = s.ChangeGameDifficultQValue,
            ChangeGridModeQValue = s.ChangeGridModeQValue
        }).ToList();
        
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id);
        Dictionary<string, object> updates = new Dictionary<string, object>{
            {"QTable", QTableFs},
        };
        docRef.UpdateAsync(updates);
    }
    
    public void SyncData()
    {
        DocumentReference docRef = db.Collection("sync").Document(curr_id);
        Dictionary<string, object> updates = new Dictionary<string, object>{
            {"LastUpdate",DateTime.Now}
        };
        docRef.SetAsync(updates);
        AudioController.SetnPlay("audio/SFX/Correct_SpecialT");
    }
}


