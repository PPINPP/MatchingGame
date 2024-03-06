using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;

public class UXTest : MonoBehaviour
{
    public GameObject UXtest_Title, UXtest_Q1, UXtest_Q2, UXtest_Q3;
    private bool globalDebugMode = true;
    float startTime;
    int totalClick = 0, currentTask = 0;

    FirebaseFirestore db;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser loginuser;

    Dictionary<string, object> sessionData = new Dictionary<string, object>()
    {
        {"totalClicks", 0},
        {"totalTime", 0 },
    };

// Start is called before the first frame update
    void Start()
    { 
        StartCoroutine(waiter());
        (db, auth) = initialize(globalDebugMode);
        loginuser = getUser(auth, globalDebugMode);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            totalClick = totalClick + 1;
            sessionData.Add($"Click{totalClick}_taskNum", currentTask);
            sessionData.Add($"Click{totalClick}_location", $"{Input.mousePosition.x},{Input.mousePosition.y}");
            sessionData.Add($"Click{totalClick}_timestamp", Time.time);
        }
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(3);
        toUXtest_Q1();
    }

    public void toUXtest_Q1()
    {
        startTime = Time.time;
        currentTask = 1;
        UXtest_Q1.SetActive(true);
        UXtest_Q2.SetActive(false);
        UXtest_Q3.SetActive(false);
        UXtest_Title.SetActive(false);
    }

    public void toUXtest_Q2()
    {
        currentTask = 2;
        UXtest_Q2.SetActive(true); 
        UXtest_Q3.SetActive(false); 
        UXtest_Title.SetActive(false);
        UXtest_Q1.SetActive(false);
    }

    public void toUXtest_Q3()
    {
        currentTask = 3;
        UXtest_Q3.SetActive(true);
        UXtest_Q1.SetActive(false);
        UXtest_Q2.SetActive(false);
        UXtest_Title.SetActive(false);
    }

    public void finishTest(string target)
    {
        sessionData["totalTime"] = Time.time - startTime;
        Debug.Log($"Time taken {sessionData["totalTime"]}");

        sessionData["totalClicks"] = totalClick;
        Debug.Log($"Total clicks: {sessionData["totalClicks"]}");

        Debug.Log($"Session data: {sessionData}");
        foreach(var kvp in  sessionData)
            Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");

        // Push to DB
        DocumentReference docref = db.Collection("GamePlayHistory").Document(loginuser.DisplayName);
        Dictionary<string, Dictionary<string, object>> clickData = new Dictionary<string, Dictionary<string, object>>()
        {
            { "ClickData", sessionData }
        };
        docref.SetAsync(clickData, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to db successfully.");
        });
        // Finish push to DB

        SceneManager.LoadScene(target);
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
