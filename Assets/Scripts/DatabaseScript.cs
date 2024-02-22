using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;

public class DatabaseScript : MonoBehaviour
{
    public bool globalDebugMode = true;

    // Start is called before the first frame update
    void Start()
    {
        (FirebaseFirestore db, Firebase.Auth.FirebaseAuth auth) = initialize(globalDebugMode);
        Firebase.Auth.FirebaseUser loginuser = getUser(auth, globalDebugMode);
        Dictionary<string, object> userData = getUserData(db, "ddcc2imdZs4KcQxLTg8S", globalDebugMode);
            


        //DocumentReference docRef = db.Collection("GamePlayHistory").Document("Test");
        //Dictionary<string, object> user = new Dictionary<string, object>
        //{
        //    {"Sex","M" }
        //};
        //docRef.SetAsync(user);



    }
    // Function to initialize firestore and fireauth
    private (FirebaseFirestore,Firebase.Auth.FirebaseAuth) initialize(bool debugMode = false)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        if (debugMode)
        {
            Debug.Log($"Firestore object: {db}");
            Debug.Log($"Firebase Auth object: {auth}");
        }

        return (db, auth);
    }

    // Function to get logged in user
    private Firebase.Auth.FirebaseUser getUser(Firebase.Auth.FirebaseAuth auth, bool debugMode = false)
    {
        Firebase.Auth.FirebaseUser loginuser = auth.CurrentUser;
        if (debugMode)
        {
            if (loginuser != null)
            {
                string name = loginuser.DisplayName;
                string email = loginuser.Email;
                string uid = loginuser.UserId;
                Debug.Log($"Username {name}, Email: {email}, Uid: {uid}");
            }
            else
            {
                Debug.Log("User not found!");
            }
        }

        return loginuser;
    }
    // Function to get kept data (whole dict) for one user
    private Dictionary<string, object> getUserData(FirebaseFirestore db, string user, bool debugMode = false)
    {
        DocumentReference docRef = db.Collection("GamePlayHistory").Document(user);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            Dictionary<string, object> retrievedDict;
            if (snapshot.Exists)
            {
                retrievedDict = snapshot.ToDictionary();
                if (debugMode) 
                { 
                    Debug.Log($"Document ID {snapshot.Id} was retrieved. The following is the data inside...");
                    foreach (KeyValuePair<string, object> pair in retrievedDict)
                    {
                        Debug.Log($"Key: {pair.Key}, Value: {pair.Value}");
                    }
                }  
            }
            else
            {
                retrievedDict = new Dictionary<string, object> { { "Error", "No data" } };
                if (debugMode) { Debug.LogError($"Document ID {snapshot.Id} does not exist!"); }
            }
            return retrievedDict;
        });
        return new Dictionary<string, object> { { "Error", "No data" } };
    }
    // C

    // R: Reading data from Firebase store

        //CollectionReference usersRef = db.Collection("GamePlayHistory");
        //usersRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        //{
        //    QuerySnapshot snapshot = task.Result;
        //    foreach (DocumentSnapshot document in snapshot.Documents)
        //    {
        //        Debug.Log(string.Format("User: {0}", document.Id));
        //        Dictionary<string, object> documentDictionary = document.ToDictionary();
        //        Debug.Log(string.Format("Sex: {0}", documentDictionary["Sex"]));

        //    }

        //    Debug.Log("Read all data from the users collection.");
        //});

    // U

    // D
}
