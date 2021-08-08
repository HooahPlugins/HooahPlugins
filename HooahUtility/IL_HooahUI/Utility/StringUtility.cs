// Decompiled with JetBrains decompiler
// Type: Utility.StringUtility
// Assembly: HooahComponents, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 36F76C18-36CE-4019-97EF-FF9FBC728F0D
// Assembly location: D:\projects\HooahPlugins\HooahComponents\AI_Hooah\bin\Release\final\AI_Hooah.dll

using System.Text.RegularExpressions;

namespace Utility
{
  public static class StringUtility
  {
    public static string ToProperCase(this string text)
    {
      string str = Regex.Replace(text, "(?<=\\w)(?=[A-Z])", " ", RegexOptions.None);
      return str.Substring(0, 1).ToUpper() + str.Substring(1);
    }
  }
}
