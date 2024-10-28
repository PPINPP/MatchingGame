using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Auth;
using Firebase.Extensions;
using Manager;
using MatchingGame.Gameplay;



public class SettingFirebase : MonoBehaviour
{
     private GameplaySequenceSO _sequenceSO;
    void Start()
    {
        _sequenceSO = ScriptableObject.CreateInstance<GameplaySequenceSO>();
        // _sequenceSO.sequences.Add();
        SequenceDetail _sequenceDetail = new SequenceDetail();
        _sequenceDetail.isGamePlay = true;
        _sequenceDetail.stageID = "112233";
        GameplaySequenceSetting _gameplay = new GameplaySequenceSetting(){
            categoryTheme = 0,
            pairType = 0,
            GameDifficult =0,
            layout =0
        };
        _sequenceDetail.gameplay = _gameplay;
        // firestore = FirebaseFirestore.GetInstance()

    }
}
