using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

namespace Manager
{
  class FirebaseManager : MonoSingleton<FirebaseManager>
  {
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseFirestore db;
    public FirebaseAuth auth;

    override protected void Awake()
    {
      Debug.Log("Initialize Firebase Manager");
      FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
      {
        dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available)
        {
          InitDB();
          InitAuth();
        }
        else
        {
          Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
        }
      });
    }

    private void InitDB()
    {
      Debug.Log("Initialize Database");
      db = FirebaseFirestore.DefaultInstance;
    }

    private void InitAuth()
    {
      Debug.Log("Initialize Database");
      auth = FirebaseAuth.DefaultInstance;
    }

    // TODO Improve to handle error and wait for result
    public async Task CreateDataWithDoc<T>(string collectionName, string documentName, T data, SetOptions setOptions)
    {
      await WaitForFirebaseInit(); // Wait for the database to be initialized

      Debug.Log($"FirebaseManager.CreateDataWithDoc: collection: {collectionName} document: {documentName} data: {data} setOptions {setOptions}");
      DocumentReference docRef = db.Collection(collectionName).Document(documentName);
      _ = docRef.SetAsync(data, setOptions);

    }

    public async Task<bool> CheckIsEmailExisted(string email)
    {
      await WaitForFirebaseInit(); // Wait for Firebase to initialize

      try
      {
        Debug.Log(email);
        var result = await auth.FetchProvidersForEmailAsync(email);
        if (result != null && result.Count() > 0)
        {
          Debug.Log($"Email '{email}' is already registered.");
          return true; // Email exists
        }
        else
        {
          Debug.Log($"Email '{email}' is not registered.");
          return false; // Email doesn't exist
        }
      }
      catch (FirebaseException e)
      {
        Debug.LogError($"Error checking email existence: {e.Message}");
        return false; // An error occurred
      }
    }
    private async Task WaitForFirebaseInit()
    {
      while (db == null || auth == null)
      {
        await Task.Delay(10);
      }
    }
  }
}