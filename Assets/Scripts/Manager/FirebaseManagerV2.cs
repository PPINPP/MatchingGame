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
using Google;
using Manager;
using Unity.VisualScripting;
using Model;
using System.Data.OleDb;
using System.Configuration;
using UniRx;
using System.Linq;
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

    public string GoogleAPI = "415072983245-jbn838hn0mhq1s9h9t2cq8i67steeejl.apps.googleusercontent.com";
    // private GoogleSignInConfiguration configuration;
    private long _cacheSize = 314572800; //Default = 104857600 : New = 314572800

    /// Profile Parameter ///
    private bool syncnetwork = true;
    string curr_id;
    string prefix_locate = "fuzzy_demo";
    string prefix_time_locate = "game_information";
    public string curr_username;
    public bool passTutorial { get; set; }
    public Dictionary<string, List<string>> cardList = new Dictionary<string, List<string>>();
    public Dictionary<string, bool> gameData = new Dictionary<string, bool>();
    public Dictionary<string, List<int>> gameState = new Dictionary<string, List<int>>();
    public Dictionary<string, List<int>> gameScore = new Dictionary<string, List<int>>();
    public int curr_week = 1;
    List<string> timeRules = new List<string>();
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

        // #if UNITY_ANDROID || UNITY_EDITOR
        //         configuration = new GoogleSignInConfiguration
        //         {
        //             WebClientId = GoogleAPI,
        //             RequestIdToken = true,
        //         };
        //         GoogleSetConfiguration();
        // #endif

        InitializeApp();
        SetParameter();
    }
    public void SetParameter()
    {
        curr_username = "";
        curr_id = "";
        curr_week = 0;
        cardList.Clear();
        gameData.Clear();
        timeRules.Clear();
        gameScore.Clear();
        gameState.Clear();
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
    }
    public void UserLogout()
    {
        SetParameter();
        SceneManager.LoadScene("Main_P");
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
                    GameRuleTimeChecker(success);
                    Dictionary<string, object> fieldVal = documentSnapshot.ToDictionary();
                    int dp = 0;
                    List<int> FuzzyProperties = new List<int>();
                    List<int> CompleteGameID = new List<int>();
                    foreach (KeyValuePair<string, object> pair in fieldVal)
                    {
                        if (pair.Key == "Username")
                        {
                            curr_username = (string)pair.Value;

                        }
                        if (pair.Key == "DayPassed")
                        {
                            dp = (int)pair.Value;
                        }

                    }
                    if (documentSnapshot.TryGetValue("FuzzyProperties", out FuzzyProperties))
                    {
                        if (documentSnapshot.TryGetValue("CompleteGameID", out CompleteGameID))
                        {
                            FuzzyBrain.Instance.SetGameProperties(FuzzyProperties, CompleteGameID, dp);
                            GetFuzzyGameData(CompleteGameID);
                        }

                    }
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
    public void GetFuzzyGameData(List<int> CompleteGameID)
    {
        List<object> _tempIDList = new List<object>();
        if (CompleteGameID.Count >= 5)
        {
            for (int i = CompleteGameID.Count - 5; i < CompleteGameID.Count; i++)
            {
                _tempIDList.Add(CompleteGameID[i]);
            }

        }
        else
        {
            _tempIDList = CompleteGameID.Cast<object>().ToList();
        }
        Query GameDataQuery = db.Collection(prefix_locate + "/" + curr_id + "/FuzzyGameData").WhereIn("GameID", _tempIDList);
        GameDataQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot capitalQuerySnapshot = task.Result;
            if (capitalQuerySnapshot.Count == _tempIDList.Count)
            {
                foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
                {
                    FuzzyBrain.Instance.UserFuzzyData.Add(new FuzzyGameData().ConvertToGameData(documentSnapshot.ConvertTo<FuzzyGameDataFs>()));
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



        // db.Collection(prefix_locate + "/" + curr_id + "/FuzzyGameData")
        //   .WhereIn("GameID", _tempIDList) // Query where GameID is in the list
        //   .GetSnapshotAsync()
        //   .ContinueWithOnMainThread(task =>
        //   {
        //       if (task.IsCompleted && !task.IsFaulted)
        //       {
        //           QuerySnapshot snapshot = task.Result;
        //           foreach (DocumentSnapshot doc in snapshot.Documents)
        //           {
        //               Debug.Log($"Document ID: {doc.Id} | GameID: {doc.GetValue<int>("GameID")}");
        //           }

        //           if (snapshot.Documents.Count == 0)
        //           {
        //               Debug.Log("No matching documents found.");
        //           }
        //       }
        //       else
        //       {
        //           Debug.LogError("Error fetching documents: " + task.Exception);
        //       }
        //   });

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
                                Debug.Log(str);
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
            Debug.LogError($"Error querying Firestore: {task.Exception.Message}");
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
        DocumentReference gameDataRef = db.Collection(prefix_locate + "/" + curr_id + "/GameDataInformation").Document("W" + curr_week.ToString());

        Task<DocumentSnapshot> task = gameDataRef.GetSnapshotAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError($"Error querying Firestore: {task.Exception.Message}");
        }
        else if (task.Result != null)
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
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
                                gameState["W" + curr_week.ToString()].Add(int.Parse(state));
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
                                gameScore["W" + curr_week.ToString()].Add(int.Parse(state));
                            }
                        }
                    }
                }
                success?.Invoke();
            }
            else
            {
                CreateWeekUserGameData("W" + curr_week.ToString());
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
        DateTime checkpointDateTime;
        DateTime currentDateTime = DateTime.Now;
        List<int> tempTimeNow;
        bool isPass = false;
        for (int i = 0; i < timeRule.Count; i++)
        {
            tempTimeNow = timeRule[i].Split(',').Select(int.Parse).ToList();
            checkpointDateTime = new DateTime(tempTimeNow[0], tempTimeNow[1], tempTimeNow[2], tempTimeNow[3], tempTimeNow[4], tempTimeNow[5]);
            if (currentDateTime > checkpointDateTime)
            {
                curr_week = i + 1;
                isPass = true;
            }
        }

        tempTimeNow = timeRule[8].Split(',').Select(int.Parse).ToList();
        checkpointDateTime = new DateTime(tempTimeNow[0], tempTimeNow[1], tempTimeNow[2], tempTimeNow[3], tempTimeNow[4], tempTimeNow[5]);
        if (currentDateTime > checkpointDateTime || !isPass)
        {
            Debug.Log("EndTestingTime");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        foreach (var item in timeRule)
        {
            timeRules.Add(item);
        }
        StartCoroutine(GetCard());
    }
    public void checkTimeChange()
    {
        DateTime checkpointDateTime;
        DateTime currentDateTime = DateTime.Now;
        List<int> tempTimeNow;
        int tempWeek = 0;
        for (int i = 0; i < timeRules.Count; i++)
        {
            tempTimeNow = timeRules[i].Split(',').Select(int.Parse).ToList();
            checkpointDateTime = new DateTime(tempTimeNow[0], tempTimeNow[1], tempTimeNow[2], tempTimeNow[3], tempTimeNow[4], tempTimeNow[5]);
            if (currentDateTime > checkpointDateTime)
            {
                tempWeek = i + 1;
            }
        }

        tempTimeNow = timeRules[8].Split(',').Select(int.Parse).ToList();
        checkpointDateTime = new DateTime(tempTimeNow[0], tempTimeNow[1], tempTimeNow[2], tempTimeNow[3], tempTimeNow[4], tempTimeNow[5]);
        if (currentDateTime > checkpointDateTime || tempWeek != curr_week)
        {
            Debug.Log("End Testing Time or Time Change");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
    public async void GameRuleTimeChecker(Action success)
    {
        DocumentReference gameRuleRef = db.Collection(prefix_time_locate).Document("game_rules");
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
        gameState["W" + curr_week.ToString()] = new List<int>() { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        gameScore["W" + curr_week.ToString()] = new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
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
    public void UploadGameStateAndGameScore(List<int> game_state, List<int> game_score)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/GameDataInformation/W" + curr_week.ToString());
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
        // _ = docRef.SetAsync(fuzzy_result.ConvertDailyFeelingResultToDailyFeelingResultFs());
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
    public void UpdateDayPassed(int dp)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id);
        Dictionary<string, object> updates = new Dictionary<string, object>{
            {"DayPassed",dp}
        };
        docRef.UpdateAsync(updates);
    }
}


