using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enum;
using UnityEngine;
using Firebase.Firestore;
using Utils;
using MatchingGame.Gameplay;

namespace Model
{
    [Serializable]
    public class QLogResult : Base
    {
        public string GameID { get; set; }
        public bool Complete { get; set; }
        public int Difficulty { get; set; }
        public bool GridMode { get; set; }
        public int GameLevel { get; set; }
        public float TimeUsed { get; set; }
        public int FalseMatch { get; set; }
        public int TotalMatch { get; set; }
        public bool PauseUsed { get; set; }
        public float FirstMatchTime { get; set; }
        public List<float> PhaseSuccessPercent { get; set; }
        public List<PhaseData> PhaseDataList { get; set; }
        public List<bool> Helper { get; set; }
        public List<int> Phase { get; set; }
        public List<string> HelperSeq { get; set; }
        public int Result { get; set;}
        public string LogText { get; set; }

        public QLogResult() : base()
        {

        }

        public QLogResultFs ConvertToFirestoreModel()
        {
            Debug.Log("============== Check Model ========");
            foreach (var phaseData in PhaseDataList)
            {
                Debug.Log(phaseData);
            }
            Debug.Log("============== End Check Model ========");
           
            QLogResultFs firestoreModel = new QLogResultFs
            {
                Uuid = this.Uuid,
                DateCreated = this.DateCreated.ToString("s"),
                DateUpdated = this.DateUpdated.ToString("s"),
                GameID = GameID,
                Complete = Complete,
                Difficulty = Difficulty,
                GridMode = GridMode,
                GameLevel = GameLevel,
                TimeUsed = TimeUsed,
                FalseMatch = FalseMatch,
                TotalMatch = TotalMatch,
                PauseUsed = PauseUsed,
                FirstMatchTime = FirstMatchTime,
                PhaseSuccessPercent = PhaseSuccessPercent,
                PhaseDataList = PhaseDataList.Select(s=> new PhaseDataFs
                    {
                        Phase = (int)s.Phase,
                        ClockTime = s.ClockTime,
                        TimeUsed = s.TimeUsed
                    }
                ).ToList(),
                Helper = Helper,
                Phase = Phase,
                HelperSeq = HelperSeq,
                Result = Result,
                LogText = LogText,

            };
            Debug.Log("============== Check struc ========");
            foreach (var phaseData in firestoreModel.PhaseDataList)
            {
                Debug.Log(phaseData);
            }
            Debug.Log("============== End Check struc ========");

            return firestoreModel;
        }
        
        // public QLogResult ConvertToGameData(QLogResultFs fuzzyGameData)
        // {
        //     return new QLogResult
        //     {
        //         GameID = fuzzyGameData.GameID,
        //         Complete = fuzzyGameData.Complete,
        //         Difficulty = fuzzyGameData.Difficulty,
        //         GridMode = fuzzyGameData.GridMode,
        //         TimeUsed = fuzzyGameData.TimeUsed,
        //         IdealMatch = fuzzyGameData.IdealMatch,
        //         GameLevel = fuzzyGameData.GameLevel,
        //         FalseMatch = fuzzyGameData.FalseMatch,
        //         TotalMatch = fuzzyGameData.TotalMatch,
        //         MatchCount = fuzzyGameData.MatchCount,
        //         PauseUsed = fuzzyGameData.PauseUsed,
        //         FirstMatchTime = fuzzyGameData.FirstMatchTime,
        //         Helper = new List<bool>(fuzzyGameData.Helper),
        //         Phase = new List<int>(fuzzyGameData.Phase),
        //         HelperSeq = new List<string>(fuzzyGameData.HelperSeq),
        //         Uuid = fuzzyGameData.Uuid,
        //         Result = fuzzyGameData.Result,
        //         LogText = fuzzyGameData.LogText,
        //
        //     };
        // }
    }
    
    public class PhaseData
    {
        public PhaseData()
        {
            
        }
        
        public PhaseData(PhaseEnum phase, float clockTime, float timeUsed)
        {
            Phase = phase;
            ClockTime = clockTime;
            TimeUsed = timeUsed;
        }
        public PhaseEnum Phase { get; set; }
        public float ClockTime { get; set; }
        public float TimeUsed { get; set; }
    }

    [FirestoreData]
    public struct QLogResultFs
    {
        [FirestoreProperty] public string Uuid { get; set; }
        [FirestoreProperty] public string DateCreated { get; set; }
        [FirestoreProperty] public string DateUpdated { get; set; }
        [FirestoreProperty] public string GameID { get; set; }
        [FirestoreProperty] public bool Complete { get; set; }
        [FirestoreProperty] public int Difficulty { get; set; }
        [FirestoreProperty] public bool GridMode { get; set; }
        [FirestoreProperty] public int GameLevel { get; set; }
        [FirestoreProperty] public float TimeUsed { get; set; }
        [FirestoreProperty] public int FalseMatch { get; set; }
        [FirestoreProperty] public int TotalMatch { get; set; }
        [FirestoreProperty] public bool PauseUsed { get; set; }
        [FirestoreProperty] public float FirstMatchTime { get; set; }
        [FirestoreProperty] public List<float> PhaseSuccessPercent { get; set; }
        [FirestoreProperty] public List<PhaseDataFs> PhaseDataList { get; set; }
        [FirestoreProperty] public List<bool> Helper { get; set; }
        [FirestoreProperty] public List<int> Phase { get; set; }
        [FirestoreProperty] public List<string> HelperSeq { get; set; }
        [FirestoreProperty] public int Result { get; set;}
        [FirestoreProperty] public string LogText { get; set; }

        public override string ToString()
        {
            return StringHelper.ToStringObj(this);
        }
    }

    [FirestoreData]
    public struct PhaseDataFs
    {
        public int Phase { get; set; }
        public float ClockTime { get; set; }
        public float TimeUsed { get; set; }
    }
   
}