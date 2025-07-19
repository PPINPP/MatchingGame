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
        public int ClickCount { get; set; }
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
                ClickCount = ClickCount,
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

            return firestoreModel;
        }
        
        public QLogResult ConvertToGameData(QLogResultFs qLogResultData)
        {
            return new QLogResult
            {
                Uuid = qLogResultData.Uuid,
                GameID = qLogResultData.GameID,
                Complete = qLogResultData.Complete,
                Difficulty = qLogResultData.Difficulty,
                GridMode = qLogResultData.GridMode,
                TimeUsed = qLogResultData.TimeUsed,
                GameLevel = qLogResultData.GameLevel,
                FalseMatch = qLogResultData.FalseMatch,
                TotalMatch = qLogResultData.TotalMatch,
                ClickCount = qLogResultData.ClickCount,
                PauseUsed = qLogResultData.PauseUsed,
                FirstMatchTime = qLogResultData.FirstMatchTime,
                Helper = new List<bool>(qLogResultData.Helper),
                Phase = new List<int>(qLogResultData.Phase),
                HelperSeq = new List<string>(qLogResultData.HelperSeq),
                Result = qLogResultData.Result,
                LogText = qLogResultData.LogText,
                PhaseSuccessPercent = qLogResultData.PhaseSuccessPercent,
                PhaseDataList = qLogResultData.PhaseDataList.Select(s=> new PhaseData
                {
                    Phase = (PhaseEnum)s.Phase,
                    ClockTime = s.ClockTime,
                    TimeUsed = s.TimeUsed
                }).ToList()
        
            };
        }
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
        [FirestoreProperty] public int ClickCount { get; set; }
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
        [FirestoreProperty] public int Phase { get; set; }
        [FirestoreProperty] public float ClockTime { get; set; }
        [FirestoreProperty] public float TimeUsed { get; set; }
    }
   
}
