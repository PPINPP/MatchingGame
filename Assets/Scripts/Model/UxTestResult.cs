using System;
using System.Collections.Generic;
using Firebase.Firestore;
using Utils;


namespace Model
{
  [Serializable]
  class UxTestResult : Base
  {
    public string TotalClicked { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public float TimeUsed { get; set; }
    public List<UxClickLog> UxClickLogs { get; set; }

    public UxTestResult() : base()
    {

    }

    public UxTestResult(string totalClicked,
     DateTime startedAt,
     DateTime completedAt,
     float timeUsed) : base()
    {
      TotalClicked = totalClicked;
      StartedAt = startedAt;
      CompletedAt = completedAt;
      TimeUsed = timeUsed;
    }

    public UxTestResultFs ConvertUxTestResultToUxTestResultFs()
    {
      UxTestResultFs uxTestResultFs = new UxTestResultFs
      {
        Uuid = this.Uuid,
        DateCreated = this.DateCreated.ToString("s"),
        DateUpdated = this.DateUpdated.ToString("s"),
        TotalClicked = this.TotalClicked,
        StartedAt = this.StartedAt.ToString("s"),
        CompletedAt = this.CompletedAt.ToString("s"),
        TimeUsed = this.TimeUsed,
        UxClickLogFs = new List<UxClickLogFs>()
      };

      // Convert each UxClickLog to UxClickLogFs
      if (this.UxClickLogs != null)
      {
        foreach (var uxClickLog in this.UxClickLogs)
        {
          uxTestResultFs.UxClickLogFs.Add(uxClickLog.ConvertUxClickLogToUxClickLogFs());
        }
      }

      return uxTestResultFs;
    }
  }

  [FirestoreData]
  public struct UxTestResultFs
  {
    [FirestoreProperty] public string Uuid { get; set; }
    [FirestoreProperty] public string DateCreated { get; set; }
    [FirestoreProperty] public string DateUpdated { get; set; }
    [FirestoreProperty] public string TotalClicked { get; set; }
    [FirestoreProperty] public string StartedAt { get; set; }
    [FirestoreProperty] public string CompletedAt { get; set; }
    [FirestoreProperty] public float TimeUsed { get; set; }
    [FirestoreProperty] public List<UxClickLogFs> UxClickLogFs { get; set; }

    public override string ToString()
    {
      return StringHelper.ToStringObj(this);
    }
  }
}