using System;
using System.Collections.Generic;
using Firebase.Firestore;
using Utils;

namespace Model
{
    [Serializable]
    public class MinigameResult : Base
    {
        public List<float> TimeUsed { get; set; } = new List<float>();
        public int ScreenHeight { get; set; }
        public int ScreenWidth { get; set; }
        public List<float> TargetPosX { get; set; }
        public List<float> TargetPosY { get; set; }
        public List<MinigameClickLog> MinigameClickLogList { get; set; }
        public DateTime CompletedAt { get; set; }
        public int ObjectID { get; set; }
        public List<int> RandomIDLogList { get; set;}


        public MinigameResult() : base()
        {

        }

        public MinigameResultFs ConverToFirestoreModel()
        {
            MinigameResultFs firestoreModel = new MinigameResultFs
            {
                Uuid = this.Uuid,
                DateCreated = this.DateCreated.ToString("s"),
                DateUpdated = this.DateUpdated.ToString("s"),
                CompletedAt = this.CompletedAt.ToString("s"),
                TimeUsed = this.TimeUsed,
                ScreenHeight = this.ScreenHeight,
                ScreenWidth = this.ScreenWidth,
                TargetPosX = this.TargetPosX,
                TargetPosY = this.TargetPosY,
                ObjectID = this.ObjectID,
                RandomIDLogList = this.RandomIDLogList,
                MinigameClickLogList = new List<MinigameClickLogFs>()
            };


            if (this.MinigameClickLogList != null || this.MinigameClickLogList.Count > 0)
            {
                //GameplayClickLogList.Sort((log1, log2) => {
                //    if (log1.Timestamp > log2.Timestamp)
                //        return -1;
                //    else return 1;
                //});

                foreach (var MinigameClickLog in MinigameClickLogList)
                {
                    firestoreModel.MinigameClickLogList.Add(MinigameClickLog.ConvertToFirestoreModel());
                }
            }

            return firestoreModel;
        }
    }

    [FirestoreData]
    public struct MinigameResultFs
    {
        [FirestoreProperty] public string Uuid { get; set; }
        [FirestoreProperty] public string DateCreated { get; set; }
        [FirestoreProperty] public string DateUpdated { get; set; }
        [FirestoreProperty] public string CompletedAt { get; set; }
        [FirestoreProperty] public List<float> TimeUsed { get; set; }
        [FirestoreProperty] public int ScreenHeight { get; set; }
        [FirestoreProperty] public int ScreenWidth { get; set; }
        [FirestoreProperty] public List<float> TargetPosX { get; set; }
        [FirestoreProperty] public List<float> TargetPosY { get; set; }
        [FirestoreProperty] public int ObjectID { get; set; }
        [FirestoreProperty] public List<int> RandomIDLogList { get; set;}

        [FirestoreProperty] public List<MinigameClickLogFs> MinigameClickLogList { get; set; }

        public override string ToString()
        {
            return StringHelper.ToStringObj(this);
        }
    }
}