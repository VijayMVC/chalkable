using System;

namespace Chalkable.Common
{
  public class Constants
  {
    public static readonly Type[] BuiltInTypes = new[]{
        typeof(bool), 
        typeof(byte), 
        typeof(sbyte), 
        typeof(char), 
        typeof(decimal), 
        typeof(double), 
        typeof(float), 
        typeof(int), 
        typeof(uint), 
        typeof(long), 
        typeof(ulong), 
        typeof(short), 
        typeof(ushort), 
        typeof(string), 
        typeof(DateTime),
        typeof(Guid)
      };

      public const string DATE_FORMAT = "yyyy-MM-dd";
  }
}
