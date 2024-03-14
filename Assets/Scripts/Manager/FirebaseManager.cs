using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Firestore;
using Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Manager
{
  class FirebaseManager : MonoSingleton<FirebaseManager>
  {
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseFirestore db;

    override protected void Awake()
    {
      Debug.Log("Initialize Firebase Manager");
      FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
      {
        dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available)
        {
          InitDB();
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

    // TODO Improve to handle error and wait for result
    public async Task CreateDataWithDoc<T>(string collectionName, string documentName, T data, SetOptions setOptions)
    {
      await WaitForDBInit(); // Wait for the database to be initialized

      Debug.Log($"FirebaseManager.CreateDataWithDoc: collection: {collectionName} document: {documentName} data: {data} setOptions {setOptions}");
      DocumentReference docRef = db.Collection(collectionName).Document(documentName);
      _ = docRef.SetAsync(data, setOptions);

    }

    private async Task WaitForDBInit()
    {
      while (db == null)
      {
        await Task.Delay(10);
      }
    }
  }
}