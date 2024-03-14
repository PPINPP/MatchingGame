using System;
using System.Reflection;

namespace Utils
{
  public static class StringHelper
  {
    public static string ToStringObj(object obj)
    {
      Type type = obj.GetType();
      PropertyInfo[] properties = type.GetProperties();

      string result = $"{type.Name}:\t";
      foreach (PropertyInfo property in properties)
      {
        object value = property.GetValue(obj);
        result += $"{property.Name}: {value}\n";
      }

      return result;
    }
  }
}