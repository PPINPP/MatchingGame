using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace Model
{

    [Serializable]
    public class FuzzyGameData : Base
    {
        public string GameID { get; set; }
        public bool Complete { get; set; }
        public int Difficulty { get; set; }
        public bool GridMode { get; set; }
        public float TimeUsed { get; set; }
        public int IdealMatch { get; set; }
        public int FalseMatch { get; set; }
        public int TotalMatch { get; set; }
        public int MatchCount { get; set; }
        public bool PauseUsed { get; set; }
        public float FirstMatchTime { get; set; }
        public List<bool> Helper { get; set; }
        public List<int> Phase { get; set; }
        public List<string> HelperSeq { get; set; }
        public int GameLevel { get; set; }

        public FuzzyGameData() : base()
        {

        }

        public FuzzyGameDataFs ConvertToFirestoreModel()
        {
            FuzzyGameDataFs _fuzzygameData = new FuzzyGameDataFs
            {
                GameID = this.GameID,
                Complete = this.Complete,//
                Difficulty = this.Difficulty,
                GridMode = this.GridMode,
                TimeUsed = this.TimeUsed, // 
                IdealMatch = this.IdealMatch,
                TotalMatch = this.TotalMatch,
                GameLevel = this.GameLevel,
                FalseMatch = this.FalseMatch,
                MatchCount = this.MatchCount,
                PauseUsed = this.PauseUsed,//
                FirstMatchTime = this.FirstMatchTime,
                Helper = this.Helper, // 
                Phase = this.Phase,
                HelperSeq = this.HelperSeq,
                Uuid = this.Uuid, //
                DateCreated = DateTime.Now.ToString("s")
            };

            return _fuzzygameData;
        }

        public FuzzyGameData ConvertToGameData(FuzzyGameDataFs fuzzyGameData)
        {
            return new FuzzyGameData
            {
                GameID = fuzzyGameData.GameID,
                Complete = fuzzyGameData.Complete,
                Difficulty = fuzzyGameData.Difficulty,
                GridMode = fuzzyGameData.GridMode,
                TimeUsed = fuzzyGameData.TimeUsed,
                IdealMatch = fuzzyGameData.IdealMatch,
                GameLevel = fuzzyGameData.GameLevel,
                FalseMatch = fuzzyGameData.FalseMatch,
                TotalMatch = fuzzyGameData.TotalMatch,
                MatchCount = fuzzyGameData.MatchCount,
                PauseUsed = fuzzyGameData.PauseUsed,
                FirstMatchTime = fuzzyGameData.FirstMatchTime,
                Helper = new List<bool>(fuzzyGameData.Helper),
                Phase = new List<int>(fuzzyGameData.Phase),
                HelperSeq = new List<string>(fuzzyGameData.HelperSeq),
                Uuid = fuzzyGameData.Uuid,

            };
        }

    }

    [FirestoreData]
    public struct FuzzyGameDataFs
    {
        [FirestoreProperty] public string GameID { get; set; }
        [FirestoreProperty] public bool Complete { get; set; }
        [FirestoreProperty] public int Difficulty { get; set; }
        [FirestoreProperty] public bool GridMode { get; set; }
        [FirestoreProperty] public float TimeUsed { get; set; }
        [FirestoreProperty] public int IdealMatch { get; set; }
        [FirestoreProperty] public int FalseMatch { get; set; }
        [FirestoreProperty] public int TotalMatch { get; set; }
        [FirestoreProperty] public int MatchCount { get; set; }
        [FirestoreProperty] public int GameLevel { get; set; }
        [FirestoreProperty] public bool PauseUsed { get; set; }
        [FirestoreProperty] public float FirstMatchTime { get; set; }
        [FirestoreProperty] public List<bool> Helper { get; set; }
        [FirestoreProperty] public List<int> Phase { get; set; }
        [FirestoreProperty] public List<string> HelperSeq { get; set; }
        [FirestoreProperty] public string Uuid { get; set; }
        [FirestoreProperty] public string DateCreated { get; set; }


        public override string ToString()
        {
            return StringHelper.ToStringObj(this);
        }
    }

    [Serializable]
    public class SpecialFuzzyData : Base
    {
        public string GameID { get; set; }
        public List<float> TimeUsed { get; set; }
        public List<int> ClickTypeList { get; set; }
        public List<int> GameScore {get; set;}

        public SpecialFuzzyData() : base()
        {

        }
        public SpecialFuzzyDataFs ConvertToFirestoreModel()
        {
            SpecialFuzzyDataFs _fuzzygameData = new SpecialFuzzyDataFs
            {
                GameID = this.GameID,
                TimeUsed = this.TimeUsed,
                ClickTypeList = this.ClickTypeList,
                GameScore = this.GameScore
            };

            return _fuzzygameData;
        }

        public SpecialFuzzyData ConvertToGameData(SpecialFuzzyDataFs specialgameData)
        {
            return new SpecialFuzzyData
            {
                GameID = specialgameData.GameID,
                TimeUsed = specialgameData.TimeUsed,
                ClickTypeList = specialgameData.ClickTypeList,
                GameScore = specialgameData.GameScore

            };
        }

    }
    [FirestoreData]
    public struct SpecialFuzzyDataFs
    {
        [FirestoreProperty] public string GameID { get; set; }
        [FirestoreProperty] public List<float> TimeUsed { get; set; }
        [FirestoreProperty] public List<int> ClickTypeList { get; set; }
        [FirestoreProperty] public List<int> GameScore {get; set;}
        public override string ToString()
        {
            return StringHelper.ToStringObj(this);
        }

    }
}