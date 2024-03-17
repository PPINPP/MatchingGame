using System;
using Enum;
using Firebase.Firestore;
using Utils;

namespace Model
{

  [Serializable]
  class UxClickLog : Base
  {
    public float PosX { get; set; }
    public float PosY { get; set; }
    public DateTime Timestamp { get; set; }
    public UxClickStatusEnum ClickStatus { get; set; }


    public UxClickLog() : base()
    {

    }

    public UxClickLog(float posX,
     float posY,
     DateTime timestamp,
     UxClickStatusEnum clickStatus
) : base()
    {
      PosX = posX;
      PosY = posY;
      Timestamp = timestamp;
      ClickStatus = clickStatus;
    }

    public UxClickLogFs ConvertUxClickLogToUxClickLogFs()
    {
      UxClickLogFs uxClickLogFs = new UxClickLogFs
      {
        Uuid = this.Uuid,
        DateCreated = this.DateCreated.ToString("s"),
        DateUpdated = this.DateUpdated.ToString("s"),
        PosX = this.PosX,
        PosY = this.PosY,
        Timestamp = this.Timestamp.ToString("s"),
        ClickStatus = this.ClickStatus.ToString()
      };

      return uxClickLogFs;
    }
  }

  [FirestoreData]
  public struct UxClickLogFs
  {
    [FirestoreProperty] public string Uuid { get; set; }
    [FirestoreProperty] public string DateCreated { get; set; }
    [FirestoreProperty] public string DateUpdated { get; set; }
    [FirestoreProperty] public float PosX { get; set; }
    [FirestoreProperty] public float PosY { get; set; }
    [FirestoreProperty] public string Timestamp { get; set; }
    [FirestoreProperty] public string ClickStatus { get; set; }

    public override string ToString()
    {
      return StringHelper.ToStringObj(this);
    }
  }
}