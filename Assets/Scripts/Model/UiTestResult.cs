using System;
using Firebase.Firestore;

namespace Model
{
  [Serializable]
  class UiTestResult : Base
  {
    public string SelectedChoice { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public float TimeUsed { get; set; }

    public UiTestResult() : base()
    {

    }

    public UiTestResult(string selectedChoice,
     DateTime startedAt,
     DateTime completedAt,
     float timeUsed) : base()
    {
      SelectedChoice = selectedChoice;
      StartedAt = startedAt;
      CompletedAt = completedAt;
      TimeUsed = timeUsed;
    }

    public UiTestResultFs ConvertUiTestResultToUiTestResultFs()
    {
      UiTestResultFs uiTestResultFs = new UiTestResultFs
      {
        Uuid = this.Uuid,
        DateCreated = this.DateCreated.ToString("s"),
        DateUpdated = this.DateUpdated.ToString("s"),
        SelectedChoice = this.SelectedChoice,
        StartedAt = this.StartedAt.ToString("s"),
        CompletedAt = this.CompletedAt.ToString("s"),
        TimeUsed = this.TimeUsed
      };

      return uiTestResultFs;
    }
  }

  [FirestoreData]
  public struct UiTestResultFs
  {
    [FirestoreProperty] public string Uuid { get; set; }
    [FirestoreProperty] public string DateCreated { get; set; }
    [FirestoreProperty] public string DateUpdated { get; set; }
    [FirestoreProperty] public string SelectedChoice { get; set; }
    [FirestoreProperty] public string StartedAt { get; set; }
    [FirestoreProperty] public string CompletedAt { get; set; }
    [FirestoreProperty] public float TimeUsed { get; set; }
  }
}