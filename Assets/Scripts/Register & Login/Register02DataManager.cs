using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;

public class Register02DataManager : MonoBehaviour
{
    public GameObject primButton, highsButton, certButton, bacButton, masButton, phdButton;
    public string nextScene;
    private bool globalDebugMode = true;
    string selected_button = "Null";

    FirebaseFirestore db;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser loginuser;

    void Start()
    {
        (db, auth) = initialize(globalDebugMode);
        loginuser = getUser(auth, globalDebugMode);
    }

    public void SubmitForm()
    {
        Debug.Log($"Sel: {selected_button}");

        // Add push to DB here!
        DocumentReference docref = db.Collection("GamePlayHistory").Document(loginuser.DisplayName);
        Dictionary<string, object> user = new Dictionary<string, object>
        {
            {"Education", selected_button },
        };
        docref.SetAsync(user, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to db successfully.");
        });

        SceneManager.LoadScene(nextScene);
    }

    public void primSelected()
    {
        selected_button = "prim";
    }
    public void highsSelected()
    {
        selected_button = "highs";
    }
    public void certSelected()
    {
        selected_button = "cert";
    }
    public void bacSelected()
    {
        selected_button = "bac";
    }
    public void masSelected()
    {
        selected_button = "mas";
    }
    public void phdSelected()
    {
        selected_button = "phd";
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
