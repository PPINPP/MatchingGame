using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Auth;
using Firebase.Extensions;
using Manager;



public class SettingFirebase : MonoBehaviour
{
    void Start()
    {
        DebugObject.Instance.Checker();
        // firestore = FirebaseFirestore.GetInstance()

    }
}
