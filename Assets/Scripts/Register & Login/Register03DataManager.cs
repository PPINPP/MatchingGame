using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;

public class Register03DataManager : MonoBehaviour
{
    public Toggle allToggle, htToggle, dmToggle, asdToggle, hlToggle, isToggle;
    private bool globalDebugMode = true;
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
        Debug.Log($"allToggle: {allToggle.isOn}, htToggle: {htToggle.isOn}, dmToggle: {dmToggle.isOn}, asdToggle: {asdToggle.isOn}, hlToggle: {hlToggle.isOn}, isToggle:{isToggle.isOn}");

        // Add push to DB here!
        DocumentReference docref = db.Collection("GamePlayHistory").Document(loginuser.DisplayName);
        Dictionary<string, object> user = new Dictionary<string, object>
        {
            {"Hypertension", htToggle.isOn },
            {"Diabetes", dmToggle.isOn },
            {"ASD", asdToggle.isOn },
            {"Hyperlipid", hlToggle.isOn },
            {"Stroke", isToggle.isOn }
        };
        docref.SetAsync(user, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to db successfully.");
        });

        // SceneManager.LoadScene(nextScene);
    }


    public void allSelection()
    {
        htToggle.isOn = allToggle.isOn;
        dmToggle.isOn = allToggle.isOn;
        asdToggle.isOn = allToggle.isOn;
        hlToggle.isOn = allToggle.isOn;
        isToggle.isOn = allToggle.isOn;
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
