using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enum;
using Experiment.QModel;
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
        public float Difficulty { get; set; }
        public string DifficultyMeaning { get; set; }
        public bool GridMode { get; set; }
        public int GameLevel { get; set; }
        public float TimeUsed { get; set; }
        public int FalseMatch { get; set; }
        public float FalseMatchPercent { get; set; }
        public int TotalMatch { get; set; }
        public int ClickCount { get; set; }
        public bool PauseUsed { get; set; }
        public float FirstMatchTime { get; set; }
        public List<float> PhaseSuccessPercent { get; set; }
        public List<PhaseData> PhaseDataList { get; set; }
        public List<bool> Helper { get; set; }
        public List<int> Phase { get; set; }
        public List<string> HelperSeq { get; set; }
        public int Result { get; set; }
        public string LogText { get; set; }
        public float Reward { get; set; }
        public float CompareEpsilonValue { get; set; }

        public MemoryPhase SelectMemoryPhase { get; set; }
        public float QValue { get; set; }
        public string QValueAction { get; set; }
        public QGameplayState GameplayState { get; set; }
        public SpeedCategoryEnum SpeedCatIRM { get; set; }
        public SpeedCategoryEnum SpeedCatSPM { get; set; }
        public FailMatchResultEnum FailMatchResult { get; set; }
        public List<QTable> QTableList { get; set; }


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
                DifficultyMeaning = DifficultyMeaning,
                GridMode = GridMode,
                GameLevel = GameLevel,
                TimeUsed = TimeUsed,
                FalseMatch = FalseMatch,
                FalseMatchPercent = FalseMatchPercent,
                TotalMatch = TotalMatch,
                ClickCount = ClickCount,
                PauseUsed = PauseUsed,
                FirstMatchTime = FirstMatchTime,
                PhaseSuccessPercent = PhaseSuccessPercent,
                Helper = Helper,
                Phase = Phase,
                HelperSeq = HelperSeq,
                Result = Result,
                LogText = LogText,
            };
            firestoreModel.PhaseDataList = PhaseDataList.Select(s => new PhaseDataFs
                {
                    Phase = (int)s.Phase,
                    ClockTime = s.ClockTime,
                    TimeUsed = s.TimeUsed
                }
            ).ToList();
            firestoreModel.SelectMemoryPhase = SelectMemoryPhase != null
                ? new MemoryPhaseFs
                {
                    Phase = SelectMemoryPhase.Phase.ToString(),
                    InPhasePercentage = SelectMemoryPhase.InPhasePercentage
                }
                : new MemoryPhaseFs();
            firestoreModel.Reward = Reward;
            firestoreModel.CompareEpsilonValue = CompareEpsilonValue;
            firestoreModel.QValue = QValue;
            firestoreModel.QValueAction = QValueAction;
            firestoreModel.GameplayState = GameplayState.ToString();
            firestoreModel.SpeedCatIRM = SpeedCatIRM.ToString();
            firestoreModel.SpeedCatSPM = SpeedCatSPM.ToString();
            firestoreModel.FailMatchResult = FailMatchResult.ToString();
            firestoreModel.QTableList = QTableList.Select(s => new QTableFs
            {
                GameplayState = s.GameplayState.ToString(),
                CardNumberIncreaseQValue = s.CardNumberIncreaseQValue,
                CardNumberMaintainQValue = s.CardNumberMaintainQValue,
                CardNumberDecreaseQValue = s.CardNumberDecreaseQValue,
                ChangeGameDifficultQValue = s.ChangeGameDifficultQValue,
                ChangeGridModeQValue = s.ChangeGridModeQValue
            }).ToList();

            return firestoreModel;
        }

        public QLogResult ConvertToGameData(QLogResultFs qLogResultData)
        {
            try
            {
                QLogResult data = new QLogResult
                {
                    Uuid = qLogResultData.Uuid,
                    GameID = qLogResultData.GameID,
                    Complete = qLogResultData.Complete,
                    Difficulty = qLogResultData.Difficulty,
                    DifficultyMeaning = qLogResultData.DifficultyMeaning,
                    GridMode = qLogResultData.GridMode,
                    TimeUsed = qLogResultData.TimeUsed,
                    GameLevel = qLogResultData.GameLevel,
                    FalseMatch = qLogResultData.FalseMatch,
                    FalseMatchPercent = qLogResultData.FalseMatchPercent,
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
                    PhaseDataList = qLogResultData.PhaseDataList.Select(s => new PhaseData
                    {
                        Phase = (PhaseEnum)s.Phase,
                        ClockTime = s.ClockTime,
                        TimeUsed = s.TimeUsed
                    }).ToList(),
                    SelectMemoryPhase = new MemoryPhase
                    {
                        Phase = (PhaseEnum)System.Enum.Parse(typeof(PhaseEnum), qLogResultData.SelectMemoryPhase.Phase),
                        InPhasePercentage = qLogResultData.SelectMemoryPhase.InPhasePercentage
                    },
                    GameplayState =
                        (QGameplayState)System.Enum.Parse(typeof(QGameplayState), qLogResultData.GameplayState),
                    SpeedCatIRM =
                        (SpeedCategoryEnum)System.Enum.Parse(typeof(SpeedCategoryEnum), qLogResultData.SpeedCatIRM),
                    SpeedCatSPM =
                        (SpeedCategoryEnum)System.Enum.Parse(typeof(SpeedCategoryEnum), qLogResultData.SpeedCatSPM),
                    FailMatchResult = (FailMatchResultEnum)System.Enum.Parse(typeof(FailMatchResultEnum),
                        qLogResultData.FailMatchResult),
                    Reward = qLogResultData.Reward,
                    CompareEpsilonValue = qLogResultData.CompareEpsilonValue,
                    QValue = qLogResultData.QValue,
                    QValueAction = qLogResultData.QValueAction,
                    QTableList = qLogResultData.QTableList.Select(s => new QTable()
                    {
                        GameplayState = (QGameplayState)System.Enum.Parse(typeof(QGameplayState), s.GameplayState),
                        CardNumberIncreaseQValue = s.CardNumberIncreaseQValue,
                        CardNumberMaintainQValue = s.CardNumberMaintainQValue,
                        CardNumberDecreaseQValue = s.CardNumberDecreaseQValue,
                        ChangeGameDifficultQValue = s.ChangeGameDifficultQValue,
                        ChangeGridModeQValue = s.ChangeGridModeQValue
                    }).ToList()
                };
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
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
        [FirestoreProperty] public float Difficulty { get; set; }
        [FirestoreProperty] public string DifficultyMeaning { get; set; }
        [FirestoreProperty] public bool GridMode { get; set; }
        [FirestoreProperty] public int GameLevel { get; set; }
        [FirestoreProperty] public float TimeUsed { get; set; }
        [FirestoreProperty] public int FalseMatch { get; set; }
        [FirestoreProperty] public float FalseMatchPercent { get; set; }
        [FirestoreProperty] public int TotalMatch { get; set; }
        [FirestoreProperty] public int ClickCount { get; set; }
        [FirestoreProperty] public bool PauseUsed { get; set; }
        [FirestoreProperty] public float FirstMatchTime { get; set; }
        [FirestoreProperty] public List<float> PhaseSuccessPercent { get; set; }
        [FirestoreProperty] public List<PhaseDataFs> PhaseDataList { get; set; }
        [FirestoreProperty] public List<bool> Helper { get; set; }
        [FirestoreProperty] public List<int> Phase { get; set; }
        [FirestoreProperty] public List<string> HelperSeq { get; set; }
        [FirestoreProperty] public int Result { get; set; }
        [FirestoreProperty] public string LogText { get; set; }
        [FirestoreProperty] public float Reward { get; set; }
        [FirestoreProperty] public float CompareEpsilonValue { get; set; }
        [FirestoreProperty] public float QValue { get; set; }
        [FirestoreProperty] public string QValueAction { get; set; }

        [FirestoreProperty] public MemoryPhaseFs SelectMemoryPhase { get; set; }
        [FirestoreProperty] public string GameplayState { get; set; }
        [FirestoreProperty] public string SpeedCatIRM { get; set; }
        [FirestoreProperty] public string SpeedCatSPM { get; set; }
        [FirestoreProperty] public string FailMatchResult { get; set; }
        [FirestoreProperty] public List<QTableFs> QTableList { get; set; }

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

    [FirestoreData]
    public struct MemoryPhaseFs
    {
        [FirestoreProperty] public string Phase { get; set; }
        [FirestoreProperty] public float InPhasePercentage { get; set; }
    }

    public class QTable
    {
        public QGameplayState GameplayState { get; set; }
        public float CardNumberIncreaseQValue { get; set; }
        public float CardNumberMaintainQValue { get; set; }
        public float CardNumberDecreaseQValue { get; set; }
        public float ChangeGameDifficultQValue { get; set; }
        public float ChangeGridModeQValue { get; set; }
    }

    [FirestoreData]
    public struct QTableFs
    {
        [FirestoreProperty] public string GameplayState { get; set; }
        [FirestoreProperty] public float CardNumberIncreaseQValue { get; set; }
        [FirestoreProperty] public float CardNumberMaintainQValue { get; set; }
        [FirestoreProperty] public float CardNumberDecreaseQValue { get; set; }
        [FirestoreProperty] public float ChangeGameDifficultQValue { get; set; }
        [FirestoreProperty] public float ChangeGridModeQValue { get; set; }
    }
}