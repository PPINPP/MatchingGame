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


public class FirebaseManagerV2 : MonoSingleton<FirebaseManagerV2>
{
    /// Class Parameter ///
    private FirebaseApp app;
    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private FirebaseUser user;

    public string GoogleAPI = "415072983245-jbn838hn0mhq1s9h9t2cq8i67steeejl.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;
    private static FirebaseManagerV2 fbm_instance = null;
    private long _cacheSize = 314572800; //Default = 104857600 : New = 314572800

    /// Callback Interface ///
    DataManager dataManager;

    /// Profile Parameter ///
    // [SerializeField] private bool syncnetwork;
    private bool syncnetwork = true;
    /// 
    /// This will be reset on signout ///
    string curr_username;
    string curr_id;
    string prefix_locate = "all_user_test";

    void Awake()
    {
        /// Keep Script Alive ///
        // DontDestroyOnLoad(this.gameObject);
        // if (fbm_instance == null)
        // {
        //     fbm_instance = this;
        // }
        // else
        // {
        //     Destroy(this.gameObject);
        // }
        /// Configure Google Service ///
        return;
    }
    void Start()
    {
        if (syncnetwork)
        {
            FirebaseFirestore.DefaultInstance.EnableNetworkAsync();
        }
        else
        {
            FirebaseFirestore.DefaultInstance.DisableNetworkAsync();
        }
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleAPI,
            RequestIdToken = true,
        };
        InitializeApp();
        GoogleSetConfiguration();
    }
    
    public void FakeInitial(){
        
    }

    public static FirebaseManagerV2 GetInstance()
    {
        return fbm_instance;
    }

    void GoogleSetConfiguration()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
    }
    public void FBMGoogleSignUp()
    {
        GoogleSetConfiguration();
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(this.OnGoogleAuthenticatedFinishedForSignUp);
    }
    public void FBMGoogleSignIn()
    {
        GoogleSetConfiguration();
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(this.OnGoogleAuthenticatedFinishedForSignIn);
    }
    public void FBMGoogleSignOut()
    {
        auth.SignOut();
    }
    public void RegisterUsernameCheck(string uname)
    {
        string[] lType = new string[] { "Username", "Email" };
        Query capitalQuery = db.Collection(prefix_locate).WhereEqualTo(lType[uname.Contains("@") ? 1 : 0], uname);
        capitalQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot capitalQuerySnapshot = task.Result;
            if (capitalQuerySnapshot.Count > 0)
            {
                RegisterManagerV2.Instance.OnResultUsernameCheck(false);
            }
            else
            {
                RegisterManagerV2.Instance.OnResultUsernameCheck(true);
            }

        });
    }
    public void OnGoogleAuthenticatedFinishedForSignUp(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Faulted");
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Cancelled");
        }
        else
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    RegisterManagerV2.Instance.OnFailedRegisterWithGoogleAccount("Task cancaled");
                    return;
                }

                if (task.IsFaulted)
                {
                    RegisterManagerV2.Instance.OnFailedRegisterWithGoogleAccount("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                user = auth.CurrentUser;
                RegisterManagerV2.Instance.OnSuccessRegisterWithGoogleAccount(user);
            });
        }
    }
    public void OnGoogleAuthenticatedFinishedForSignIn(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Faulted");
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Cancelled");
        }
        else
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                user = auth.CurrentUser;
                curr_id = user.UserId;
                LoginManagerV2.Instance.OnSuccessSignInWithGoogleAccount(user);
            });
        }
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


                db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Initialize Database Successfully");
                // Debug.Log(db.Settings.CacheSizeBytes);
                if (db.Settings.CacheSizeBytes != _cacheSize)
                {
                    db.Settings.CacheSizeBytes = _cacheSize;
                }
                // Debug.Log(db.Settings.CacheSizeBytes);

                // FirebaseFirestore.GetInstance(app).Settings.CacheSizeBytes = _cacheSize;

                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                Debug.Log("Initialize Authenticator Successfully");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
    public void NewRegister(UserRegisterForm form)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(form.Uuid);
        docRef.SetAsync(form);
        Debug.Log("Added User to " + prefix_locate);
        RegisterManagerV2.Instance.OnCompleteRegister();
    }

    public void GetUser(bool loginType, string nkey, string pkey)
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
                }
                LoginManagerV2.Instance.OnVerifiedUser();
            }
            else if (capitalQuerySnapshot.Count == 0)
            {
                Debug.Log("Username doesn't exist or Password is wrong");
                LoginManagerV2.Instance.OnFailedLogin();
            }
            else
            {
                Debug.Log("Something went wrong");
                LoginManagerV2.Instance.OnFailedLogin();
            }
        });
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
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/SmileyoMeter/task" + idnum.ToString("00"));
        _ = docRef.SetAsync(smile_result.ConvertSmileyoMeterResultToSmileyoMeterResultFs());
    }
    public void UploadTutorialResult(GamePlayResult tutorial_result)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/TutorialLog/" + tutorial_result.StageID + "_" + tutorial_result.CompletedAt.ToString("s"));
        _ = docRef.SetAsync(tutorial_result.ConvertToFirestoreModel());
    }
    public void UploadUxTestResult(UxTestResultFs ux_result, int idnum)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/UX Test/task" + idnum.ToString("00"));
        _ = docRef.SetAsync(ux_result);
    }
    public void UploadUiTestResult(UiTestResultFs ui_result, int idnum)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/UI Test/task" + idnum.ToString("00"));
        _ = docRef.SetAsync(ui_result);
    }

    public void UploadDailyFeelingResult(DailyFeelingResult daily_result, int idnum)
    {
        DocumentReference docRef = db.Collection(prefix_locate).Document(curr_id + "/DailyFeeling/task" + idnum.ToString("00"));
        _ = docRef.SetAsync(daily_result.ConvertDailyFeelingResultToDailyFeelingResultFs());
    }
}


