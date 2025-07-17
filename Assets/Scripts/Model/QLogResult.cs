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
    public class QLogResult : Base
    {
        public string StageID { get; set; }
        public PairType CardPair { get; set; }
        public DateTime CompletedAt { get; set; }
        // TODO : Example For List Assign
        // public List<GameplayClickLog> GameplayClickLogList { get; set; }


        public QLogResult() : base()
        {

        }

        // TODO : Set Contractor
        public QLogResult(string stageID, PairType cardPair
            ) : base()
        {
            StageID = stageID;
            CardPair = cardPair;
        }

        public QLogResultFs ConvertToFirestoreModel()
        {
            QLogResultFs firestoreModel = new QLogResultFs
            {
                Uuid = this.Uuid,
                DateCreated = this.DateCreated.ToString("s"),
                DateUpdated = this.DateUpdated.ToString("s"),
                CompletedAt = this.CompletedAt.ToString("s"),
                StageID = this.StageID,
            };


            // TODO : Example For List Assign
            // if (this.GameplayClickLogList != null || this.GameplayClickLogList.Count > 0)
            // {
            //     
            //     foreach (var GameplayClickLog in GameplayClickLogList)
            //     {
            //         firestoreModel.GameplayClickLogList.Add(GameplayClickLog.ConverToFirestoreModel());
            //     }
            // }

            

            return firestoreModel;
        }
    }

    [FirestoreData]
    public struct QLogResultFs
    {
        [FirestoreProperty] public string Uuid { get; set; }
        [FirestoreProperty] public string DateCreated { get; set; }
        [FirestoreProperty] public string DateUpdated { get; set; }
        [FirestoreProperty] public string CompletedAt { get; set; }
        [FirestoreProperty] public string StageID { get; set; }

        public override string ToString()
        {
            return StringHelper.ToStringObj(this);
        }
    }
}