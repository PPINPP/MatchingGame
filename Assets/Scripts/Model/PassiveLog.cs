using System;
using Enum;
using Firebase.Firestore;
using Utils;
namespace Model
{
    [Serializable]
    public class PassiveLog : Base
    {
        public float StartTime { get; set; }
        public float StopTime { get; set; }
        public float Duration { get; set; }
        public string CardKey { get; set; }

        public PassiveLog() : base()
        {

        }

        public PassiveLog(float startTime,float stopTime,float duration,string cardKey) :base() { 
            StartTime = startTime;
            StopTime = stopTime;
            Duration = duration;
            CardKey = cardKey;
        }

        public PassiveLogFs ConverToFirestoreModel()
        {
            PassiveLogFs firestoreModel = new PassiveLogFs { 
                Uuid = this.Uuid,
                StartTime = this.StartTime, 
                StopTime = this.StopTime, 
                Duration = this.Duration,
                CardKey = this.CardKey
            };

            return firestoreModel;
        }
    }

    [FirestoreData]
    public struct PassiveLogFs
    {
        [FirestoreProperty] public string Uuid { get; set; }
        [FirestoreProperty] public float StartTime { get; set; }
        [FirestoreProperty] public float StopTime { get; set; }
        [FirestoreProperty] public float Duration { get; set; }
        [FirestoreProperty] public string CardKey { get; set; }

        public override string ToString()
        {
            return StringHelper.ToStringObj(this);
        }
    }
}