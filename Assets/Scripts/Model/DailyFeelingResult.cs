using System;
using Firebase.Firestore;
using Utils;

namespace Model
{
  [Serializable]
  public class DailyFeelingResult : Base
  {
    public string Feeling { get; set; }

    public DailyFeelingResult(string feeling) : base()
    {
      Feeling = feeling;
    }

    public DailyFeelingResult() : base()
    {
    }

    public DailyFeelingResultFs ConvertDailyFeelingResultToDailyFeelingResultFs()
    {
      DailyFeelingResultFs SmileyoMeterResultFs = new DailyFeelingResultFs
      {
        Uuid = this.Uuid,
        DateCreated = this.DateCreated.ToString("s"),
        DateUpdated = this.DateUpdated.ToString("s"),
        Feeling = this.Feeling,
      };

      return SmileyoMeterResultFs;
    }
  }

  [FirestoreData]
  public struct DailyFeelingResultFs
  {
    [FirestoreProperty] public string Uuid { get; set; }
    [FirestoreProperty] public string DateCreated { get; set; }
    [FirestoreProperty] public string DateUpdated { get; set; }
    [FirestoreProperty] public string Feeling { get; set; }

    public override string ToString()
    {
      return StringHelper.ToStringObj(this);
    }
  }
}