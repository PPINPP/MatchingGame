using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Utils;
using MatchingGame.Gameplay;

namespace Model
{
    [Serializable]
    public class GamePlayResult : Base
    {
        public string StageID {  get; set; }
        public PairType CardPair {  get; set; }
        public GameLayout CardPatternLayout { get; set; }
        public GameDifficult GameDifficult { get; set; }
        public float TimeUsed { get; set; }
        public int ClickCount { get; set; }
        public int MatchFalseCount { get; set; }
        public int ScreenHeight { get; set; }
        public int ScreenWidth { get; set; }
        public List<CardPosLog> CardPosLogList { get; set; }    
        public List<GameplayClickLog> GameplayClickLogList { get; set; }


        public GamePlayResult() : base()
        {

        }

        public GamePlayResult(string stageID,PairType cardPair,GameLayout cardParrenLayout,
            GameDifficult gameDifficult,float timeUsed,int clickCount, int matchFalseCount,
            int screenHeight,int screenWidth, List<CardPosLog> cardPosLogList,
            List<GameplayClickLog> gameplayClickLogList) : base()
        {
            StageID = stageID;
            CardPair = cardPair;
            CardPatternLayout = cardParrenLayout;
            GameDifficult = gameDifficult;
            TimeUsed = timeUsed;
            ClickCount = clickCount;
            MatchFalseCount = matchFalseCount;
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;
            CardPosLogList = cardPosLogList;
            GameplayClickLogList = gameplayClickLogList;
        }

        public GamePlayResultFs ConverToFirestoreModel()
        {
            GamePlayResultFs firestoreModel = new GamePlayResultFs
            {
                Uuid = this.Uuid,
                DateCreated = this.DateCreated.ToString("s"),
                DateUpdated = this.DateUpdated.ToString("s"),
                StageID = this.StageID,
                CardPair = (int)this.CardPair,
                CardPatternLayout = this.CardPatternLayout.ToString(),
                GameDifficult = this.GameDifficult.ToString(),
                TimeUsed = this.TimeUsed,
                ClickCount = this.ClickCount,
                MatchFalseCount = this.MatchFalseCount,
                ScreenHeight = this.ScreenHeight,
                ScreenWidth = this.ScreenWidth,
                CardPosLogList = new List<CardPosLogFs>(),
                GameplayClickLogList = new List<GameplayClickLogFs>()
            };

            if (this.CardPosLogList !=null || this.CardPosLogList.Count > 0 )
            {
                foreach (var cardPosLog in CardPosLogList)
                {
                    firestoreModel.CardPosLogList.Add(cardPosLog.ConverToFirestoreModel());
                }
            }

            if (this.GameplayClickLogList != null || this.GameplayClickLogList.Count > 0)
            {
                //GameplayClickLogList.Sort((log1, log2) => {
                //    if (log1.Timestamp > log2.Timestamp)
                //        return -1;
                //    else return 1;
                //});

                foreach (var GameplayClickLog in GameplayClickLogList)
                {
                    firestoreModel.GameplayClickLogList.Add(GameplayClickLog.ConverToFirestoreModel());
                }
            }

            return firestoreModel;
        }
    }

    [FirestoreData]
    public struct GamePlayResultFs
    {
        [FirestoreProperty] public string Uuid { get; set; }
        [FirestoreProperty] public string DateCreated { get; set; }
        [FirestoreProperty] public string DateUpdated { get; set; }
        [FirestoreProperty] public string StageID { get; set; }
        [FirestoreProperty] public int CardPair { get; set; }
        [FirestoreProperty] public string CardPatternLayout { get; set; }
        [FirestoreProperty] public string GameDifficult { get; set; }
        [FirestoreProperty] public float TimeUsed { get; set; }
        [FirestoreProperty] public int ClickCount { get; set; }
        [FirestoreProperty] public int MatchFalseCount { get; set; }
        [FirestoreProperty] public int ScreenHeight { get; set; }
        [FirestoreProperty] public int ScreenWidth { get; set; }
        [FirestoreProperty] public List<CardPosLogFs> CardPosLogList { get; set; }
        [FirestoreProperty] public List<GameplayClickLogFs> GameplayClickLogList { get; set; }

        public override string ToString()
        {
            return StringHelper.ToStringObj(this);
        }
    }
}