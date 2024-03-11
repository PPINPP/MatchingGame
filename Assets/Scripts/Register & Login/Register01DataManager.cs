using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using TMPro;

public class Register01DataManager : MonoBehaviour
{
    public TMP_InputField AgeField, DateField, MonthField, YearField;
    public GameObject MaleButton, FemaleButton;
    public string nextScene;
    private bool globalDebugMode = true;
    FirebaseFirestore db;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser loginuser;

    string selected_button = "Null";

    void Start()
    {
        (db, auth) = initialize(globalDebugMode);
        loginuser = getUser(auth, globalDebugMode);
    }

    public void SubmitForm()
    {
        string age = AgeField.text;
        string dob = DateField.text;
        string month = MonthField.text;
        string year = YearField.text;
        Debug.Log($"Age: {age}, DOB: {dob}, Month: {month}, Year: {year}, Sel: {selected_button}");

        // Add push to DB here!
        DocumentReference docref = db.Collection("GamePlayHistory").Document(loginuser.DisplayName);
        Dictionary<string, object> user = new Dictionary<string, object>
        {
            {"Age", age },
            {"DOB", dob },
            {"Month", month },
            {"Year", year },
            {"Gender", selected_button},
        };
        docref.SetAsync(user, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to db successfully.");
        });

        SceneManager.LoadScene(nextScene);
    }

    public void MaleSelected()
    {
        selected_button = "Male";
    }
    public void FemaleSelected()
    {
        selected_button = "Female";
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
