using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Diagnostics;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;



public class UITest : MonoBehaviour
{
  // Start is called before the first frame update
  private bool globalDebugMode = true;

  Firebase.Firestore.FirebaseFirestore db;
  Firebase.Auth.FirebaseAuth auth;
  Firebase.Auth.FirebaseUser loginuser;

  public GameObject intoClick;
  public GameObject background;
  private string question;
  private int qNum = 0;
  private Dictionary<string, List<Dictionary<string, string>>> user = new()
    {
        {"UITest", new() }
    };

  [SerializeField] private UI_Question[] all_question;
  DateTime startTime = DateTime.Now;
  DateTime endTime = DateTime.Now;


  // Push to DB
  // DocumentReference docref = db.Collection("GamePlayHistory").Document(loginuser.DisplayName);
  // docref.SetAsync(user, SetOptions.MergeAll).ContinueWithOnMainThread(task => { Debug.Log("Added data to db successfully."); });
  // Finish push to DB

  private bool TryShowQuestion() //ควรต้ั้งชื่อฟังก์ชันให้สื่อถึงการทำงานของฟังก์ชัน
  {
    // Check is more question?
    if (qNum >= all_question.Length)
    {
      return false;
    }

    // Show Next Question
    all_question[qNum].ShowQuestion(true);
    return true;

    /*
    string test = "1";
    bool test2 = int.TryParse(test, out int result); // out คือ return (สามารถ return ได้หลายค่าโดยการใส่วงเล็บ)
    */
  }


  public void SwitchUITest()
  {
    Debug.Log("SwitchUITest button pressed");
    Debug.Log("CheckName: " + background.name);
    background.SetActive(false);
    intoClick.SetActive(false);
    startTime = DateTime.Now;

    TryShowQuestion();

  }



  public void CollectAnswer(Dictionary<string, string> result)
  {

    //user.Add("UITest", result);
    user["UITest"].Add(result);
    all_question[qNum].ShowQuestion(false);
    qNum++;

    if (!TryShowQuestion())
    {
      var test = user;
      Debug.Log("End!");
    }

  }

  void Start()
  {
    // (db, auth) = initialize(globalDebugMode);
    // loginuser = getUser(auth, globalDebugMode);
    foreach (UI_Question question in all_question) // ถ้าใช้ var จะหาให้ว่า datatype คืออะไร แต่คนเขียนจะไม่รู้
    {
      question.ShowQuestion(false);
      question.Init(this);
    }
  }

  // private (FirebaseFirestore, Firebase.Auth.FirebaseAuth) initialize(bool debugMode = false)
  // {
  //   FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
  //   Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

  //   if (debugMode)
  //   {
  //     Debug.Log($"Firestore object: {db}");
  //     Debug.Log($"Firebase Auth object: {auth}");
  //   }

  //   return (db, auth);
  // }

  // private Firebase.Auth.FirebaseUser getUser(Firebase.Auth.FirebaseAuth auth, bool debugMode = false)
  // {
  //   Firebase.Auth.FirebaseUser loginuser = auth.CurrentUser;
  //   if (debugMode)
  //   {
  //     if (loginuser != null)
  //     {
  //       string name = loginuser.DisplayName;
  //       string email = loginuser.Email;
  //       string uid = loginuser.UserId;
  //       Debug.Log($"Username {name}, Email: {email}, Uid: {uid}");
  //     }
  //     else
  //     {
  //       Debug.Log("User not found!");
  //     }
  //   }

  //   return loginuser;
  // }

}
