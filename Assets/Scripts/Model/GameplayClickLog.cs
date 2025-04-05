using System;
using Enum;
using Firebase.Firestore;
using UniRx;
using Utils;
public enum GameplayClickStatusEnum
{
    OUT_CARD,
    ON_CARD,
    OTHER
}

public enum GameplayClickResultEnum
{
    REPEAT,
    UNMATCH, 
    MATCHED,
    FALSE_MATCH
}

namespace Model
{
    [Serializable]
    public class GameplayClickLog : Base
    {
        public float ClickScreenPosX { get; set; }
        public float ClickScreenPosY { get; set; }
        public float ClickTime { get; set; }
        public GameplayClickStatusEnum ClickStatus { get; set; }
        public GameplayClickResultEnum ClickResult { get; set; }

        public GameplayClickLog() : base()
        {

        }
        public GameplayClickLog(float clickScreenPosX, float clickScreenPosY,
            float clickTime, GameplayClickStatusEnum status, 
            GameplayClickResultEnum result) : base()
        { 
            ClickScreenPosX = clickScreenPosX;
            ClickScreenPosY = clickScreenPosY;
            ClickTime = clickTime;
            ClickStatus = status;
            ClickResult = result;
        }

        public GameplayClickLogFs ConverToFirestoreModel()
        {
            GameplayClickLogFs firestoreModel = new GameplayClickLogFs
            {
                Uuid = this.Uuid,
                ClickScreenPosX = this.ClickScreenPosX,
                ClickScreenPosY = this.ClickScreenPosY,
                ClickTime = this.ClickTime,
                ClickStatus = this.ClickStatus,
                ClickResult = this.ClickResult
            };

            return firestoreModel;
        }
    }

    [FirestoreData]
    public struct GameplayClickLogFs
    {
        [FirestoreProperty] public string Uuid { get; set; }
        [FirestoreProperty] public float ClickScreenPosX { get; set; }
        [FirestoreProperty]  public float ClickScreenPosY { get; set; }
        [FirestoreProperty]  public float ClickTime { get; set; }
        [FirestoreProperty]  public GameplayClickStatusEnum ClickStatus { get; set; }
        [FirestoreProperty]  public GameplayClickResultEnum ClickResult { get; set; }

        public override string ToString()
        {
            return StringHelper.ToStringObj(this);
        }
    }
}