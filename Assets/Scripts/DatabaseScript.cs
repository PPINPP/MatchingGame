using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;

using Firebase.Firestore;
using Firebase.Extensions;


public class DatabaseScript : MonoBehaviour
{
    FirebaseFirestore db;
    // Start is called before the first frame update
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        print(db);
        
        /*
        DocumentReference docRef = db.Collection("GamePlayHistory").Document("Test");
        Dictionary<string, object> user = new Dictionary<string, object>
        {
            {"Sex","M" }
        };
        docRef.SetAsync(user);
        */
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
    }
}
