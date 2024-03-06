using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;

public class Register04DataManager : MonoBehaviour
{
    public GameObject noButton, mciButton, alzButton;
    public string nextScene;
    private bool globalDebugMode = true;
    FirebaseFirestore db;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser loginuser;

    void Start()
    {
        (db, auth) = initialize(globalDebugMode);
        loginuser = getUser(auth, globalDebugMode);
    }

    string selected_button = "Null";


    public void SubmitForm()
    {
        Debug.Log($"Sel: {selected_button}");

        // Push to DB
        DocumentReference docref = db.Collection("GamePlayHistory").Document(loginuser.DisplayName);
        Dictionary<string, object> brainDiseaseDict = new Dictionary<string, object>()
        {
            { "brainDisease", selected_button }
        };

        Dictionary<string, Dictionary<string, object>> user = new Dictionary<string, Dictionary<string, object>>()
        {
            {
                "Register4Data", brainDiseaseDict
            }   
        };
        docref.SetAsync(user, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to db successfully.");
        });
        // Finish add to DB

        SceneManager.LoadScene(nextScene);
    }

    public void noSelected()
    {
        selected_button = "No";
    }
    public void alzSelected()
    {
        selected_button = "Alzheimer";
    }
    public void mciSelected()
    {
        selected_button = "MCI";
    }

    private (FirebaseFirestore, Firebase.Auth.FirebaseAuth) initialize(bool debugMode = false)
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
}
