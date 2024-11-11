using System;
using Enum;
using Firebase.Firestore;
using Utils;
namespace Model
{
    [Serializable]
    public class PauseLog : Base
    {
        public string StopAt { get; set; }
        public string StopDuration { get; set; }

        public PauseLog() : base()
        {

        }

        public PauseLog(string stopAt,string stopDuration) :base() { 
            StopAt = stopAt;
            StopDuration = stopDuration;
        }

        public PauseLogFs ConverToFirestoreModel()
        {
            PauseLogFs firestoreModel = new PauseLogFs { 
                Uuid = this.Uuid,
                StopAt = this.StopAt, 
                StopDuration = this.StopDuration
            };

            return firestoreModel;
        }
    }

    [FirestoreData]
    public struct PauseLogFs
    {
        [FirestoreProperty] public string Uuid { get; set; }
        [FirestoreProperty] public string StopAt { get; set; }
        [FirestoreProperty] public string StopDuration { get; set; }

        public override string ToString()
        {
            return StringHelper.ToStringObj(this);
        }
    }
}