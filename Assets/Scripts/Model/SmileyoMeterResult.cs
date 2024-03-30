using System;
using Firebase.Firestore;
using Utils;

namespace Model
{
  [Serializable]
  public class SmileyoMeterResult : Base
  {
    public int Enjoyable { get; set; }
    public int Fatigue { get; set; }

    public SmileyoMeterResult(int enjoyable, int fatigue) : base()
    {
      Enjoyable = enjoyable;
      Fatigue = fatigue;
    }

    public SmileyoMeterResult() : base()
    {
    }

    public SmileyoMeterResultFs ConvertSmileyoMeterResultToSmileyoMeterResultFs()
    {
      SmileyoMeterResultFs SmileyoMeterResultFs = new SmileyoMeterResultFs
      {
        Uuid = this.Uuid,
        DateCreated = this.DateCreated.ToString("s"),
        DateUpdated = this.DateUpdated.ToString("s"),
        Enjoyable = this.Enjoyable,
        Fatigue = this.Fatigue,
      };

      return SmileyoMeterResultFs;
    }
  }

  [FirestoreData]
  public struct SmileyoMeterResultFs
  {
    [FirestoreProperty] public string Uuid { get; set; }
    [FirestoreProperty] public string DateCreated { get; set; }
    [FirestoreProperty] public string DateUpdated { get; set; }
    [FirestoreProperty] public int Enjoyable { get; set; }
    [FirestoreProperty] public int Fatigue { get; set; }

    public override string ToString()
    {
      return StringHelper.ToStringObj(this);
    }
  }
}