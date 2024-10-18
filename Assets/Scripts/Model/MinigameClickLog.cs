using System;
using Firebase.Firestore;
using Utils;

public enum MinigameClickStatusEnum
{
    NORMAL,
    LATE,
    FALSE
}

namespace Model
{
    [Serializable]
    public class MinigameClickLog : Base
    {
        public MinigameClickLog()
        {
            
        }

        public MinigameClickLog(float clickScreenPosX, float clickScreenPosY,
            float clickTime,MinigameClickStatusEnum status)
        {
            ClickScreenPosX = clickScreenPosX;
            ClickScreenPosY = clickScreenPosY;
            ClickStatus = status;
        }
        
        public float ClickScreenPosX { get; set; }
        public float ClickScreenPosY { get; set; }
        public MinigameClickStatusEnum ClickStatus { get; set; }
        public bool isCorrect { get; set;}
        
        public MinigameClickLogFs ConvertToFirestoreModel()
        {
            MinigameClickLogFs firestoreModel = new MinigameClickLogFs
            {
                Uuid = this.Uuid,
                ClickScreenPosX = this.ClickScreenPosX,
                ClickScreenPosY = this.ClickScreenPosY,
                ClickStatus = this.ClickStatus,
                isCorrect = this.isCorrect
            };

            return firestoreModel;
        }
    }
    
    [FirestoreData]
    public struct MinigameClickLogFs
    {
        [FirestoreProperty] public string Uuid { get; set; }
        [FirestoreProperty] public float ClickScreenPosX { get; set; }
        [FirestoreProperty]  public float ClickScreenPosY { get; set; }
        [FirestoreProperty]  public MinigameClickStatusEnum ClickStatus { get; set; }
        [FirestoreProperty]  public bool isCorrect { get; set; }
        
        public override string ToString()
        {
            return StringHelper.ToStringObj(this);
        }
    }
}