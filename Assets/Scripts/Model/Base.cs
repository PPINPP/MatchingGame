using System;
using Utils;

namespace Model
{
  [Serializable]
  public class Base
  {
    public string Uuid;
    public DateTime DateCreated;
    public DateTime DateUpdated;

    public Base()
    {
      Uuid = Guid.NewGuid().ToString();
      DateCreated = DateTime.Now;
      DateUpdated = DateTime.Now;
    }

    public override string ToString()
    {
      return StringHelper.ToStringObj(this);
    }
  }
}