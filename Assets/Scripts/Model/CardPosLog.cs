using System;
using Enum;
using Firebase.Firestore;
using Utils;
namespace Model
{
    [Serializable]
    public class CardPosLog : Base
    {
        public string ItemID { get; set; }
        public float ScreenPosX { get; set; }
        public float ScreenPosY { get; set; }

        public CardPosLog() : base()
        {

        }

        public CardPosLog(string itemID,float screenPosX,float screenPosY) :base() { 
            ItemID = itemID;
            ScreenPosX = screenPosX;
            ScreenPosY = screenPosY;
        }

        public CardPosLogFs ConverToFirestoreModel()
        {
            CardPosLogFs firestoreModel = new CardPosLogFs { 
                Uuid = this.Uuid,
                ItemID = this.ItemID, 
                ScreenPosX = this.ScreenPosX, 
                ScreenPosY = this.ScreenPosY 
            };

            return firestoreModel;
        }
    }

    [FirestoreData]
    public struct CardPosLogFs
    {
        [FirestoreProperty] public string Uuid { get; set; }
        [FirestoreProperty] public string ItemID { get; set; }
        [FirestoreProperty] public float ScreenPosX { get; set; }
        [FirestoreProperty] public float ScreenPosY { get; set; }

        public override string ToString()
        {
            return StringHelper.ToStringObj(this);
        }
    }
}