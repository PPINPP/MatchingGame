using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Firebase.Firestore;
using Firebase.Extensions;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Manager;
using Model;
using MatchingGame.Gameplay;

enum SEX
{
    MALE,
    FEMALE,
    LQBTQ
}

class DataTestStruct
{
    int age;
    string nickname;
    public SEX sex;
}

public class FirebaseMockUpTester : SerializedMonoBehaviour
{
    FirebaseFirestore db;

    private void InitDB()
    {
        db = FirebaseFirestore.DefaultInstance;

        print(db);
    }

    [Title("Write Test")]
    [SerializeField] string nickname;
    [SerializeField] SEX sex;
    [SerializeField] int age;

    #region Button
    [Button]
    void TestCreateWithTargetDoc(string collectionName = "GamePlayHistory", string documentName = "Test")
    {
        InitDB();
        CreateDataWithDoc(collectionName,documentName);
    }

    [Button]
    void TestCreateWithOutTargetDoc(string collectionName = "GamePlayHistory")
    {
        InitDB();
        CreateDataWithOutDoc(collectionName);
    }

    [Title("Read Test")]
    [Button]
    void TestRead(string collectionName = "GamePlayHistory")
    {
        InitDB();
        if (collectionName == "GamePlayHistory")
        {
            ReadGameplayHistoryDoc(collectionName);
        }
        //else if (collectionName == "")
        //{

        //}
        //else
        //{

        //}
    }

    [Title("Auth")]
    [Button]
    void TestSignupUser()
    {
        InitDB();
        string email = "r@mail.com";
        string password = "123456";
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });
    }
    #endregion

    async void ReadGameplayHistoryDoc(string collectionName)
    {
        var snapshot = await ReadDataWithTargetCollection(collectionName);
        if (snapshot != null && snapshot.Count > 0)
        {
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Debug.Log($"User: {document.Id}");
                Dictionary<string, object> documentDictionary = document.ToDictionary();

                var personalData = documentDictionary["PersonalDetail"];
                var json = JsonConvert.SerializeObject(personalData);
                Dictionary<string, object> personalDataDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                DataTestStruct dataTestStruct = JsonConvert.DeserializeObject<DataTestStruct>(json);
                Debug.Log($"{dataTestStruct.sex}");
            }

            Debug.Log($"Read all data from {collectionName} collection.");
        }
    }

    void CreateDataWithDoc(string collectionName, string documentName)
    {
        DocumentReference docRef = db.Collection(collectionName).Document(documentName);

        Dictionary<string, object> user = SetupCreateData();

        docRef.SetAsync(user, SetOptions.MergeAll);
    }

    void CreateDataWithOutDoc(string collectionName)
    {
        CollectionReference colRef = db.Collection(collectionName);
        Dictionary<string, object> user = SetupCreateData();

        colRef.AddAsync(user);
    }

    Dictionary<string, object> SetupCreateData()
    {
        Dictionary<string, object> user = new Dictionary<string, object>
        {
            {"PersonalDetail", new Dictionary<string, object>
                {
                    {nameof(nickname), nickname },
                    {nameof(sex), sex },
                    {nameof(age), age }
                }
            }
        };
        return user;
    }

    async Task<QuerySnapshot> ReadDataWithTargetCollection(string collectionName)
    {
        CollectionReference usersRef = db.Collection(collectionName);
        var task = usersRef.GetSnapshotAsync();
        await task;
        if (task.IsCompletedSuccessfully)
        {
            return task.Result;
        }
        else { return null; }
    }

    //void ReadData(string collectionName)
    //{
    //    CollectionReference usersRef = db.Collection(collectionName);
    //    usersRef.GetSnapshotAsync().ContinueWithOnMainThread(async task =>
    //    {
    //        QuerySnapshot snapshot = task.Result; 
    //        foreach (DocumentSnapshot document in snapshot.Documents)
    //        {
    //            Debug.Log($"User: {document.Id}");
    //            Dictionary<string, object> documentDictionary = document.ToDictionary();

    //            var personalData = documentDictionary["PersonalDetail"];
    //            var json = JsonConvert.SerializeObject(personalData);
    //            Dictionary<string, object> personalDataDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
    //            DataTestStruct dataTestStruct = JsonConvert.DeserializeObject<DataTestStruct>(json);
    //            Debug.Log($"{dataTestStruct.sex}");
    //        }

    //        Debug.Log("Read all data from the users collection.");
    //    });
    //}


    [Title("Test Data Gameplay")]
    [Button]
    void TestCreateGameplayResultWithTargetDoc()
    {
        InitDB();
        string collectionName = "GamePlayHistory";
        string documentName = "GameplayResult";
        List<GamePlayResult> results = new List<GamePlayResult>();
        List<CardPosLog> cardPosLogList = new List<CardPosLog>() { 
            new CardPosLog("home_001", 1.0f, 2.0f),
            new CardPosLog("home_002",3.0f,2.0f)
        };
        List<GameplayClickLog> gameplayClickLogList = new List<GameplayClickLog>()
        {
            new GameplayClickLog(2.0f,3.0f,3.4f,GameplayClickStatusEnum.ON_CARD,GameplayClickResultEnum.MATCHED),
            new GameplayClickLog(2.0f,4.0f,3.5f,GameplayClickStatusEnum.ON_CARD,GameplayClickResultEnum.UNMATCH),
        };
        GamePlayResult result1 = new GamePlayResult("stage1", PairType.FOUR, GameLayout.GRID,
            GameDifficult.HARD, 30f, 150, 5, 1920, 1080, cardPosLogList, gameplayClickLogList);
       
        GamePlayResult result2 = new GamePlayResult("stage2", PairType.EIGHT, GameLayout.RANDOM,
            GameDifficult.HARD, 40f, 160, 5, 1920, 1080, cardPosLogList, gameplayClickLogList);

        results.Add(result1);
        results.Add(result2);

        for (int i = 0; i < results.Count; i++)
        {
            string doc = $"{documentName}/GameplayClickLogs/task{i:D2}";
            DocumentReference docRef = db.Collection(collectionName).Document(doc);
            _ = docRef.SetAsync(results[i].ConverToFirestoreModel(), SetOptions.Overwrite);
            //_ = FirebaseManager.Instance.CreateDataWithDoc(collectionName, $"{documentName}/task{i:D2}", results[i].ConverToFirestoreModel(), SetOptions.Overwrite);
        }
        
    }
}
