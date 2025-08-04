using System;
using System.Diagnostics;

public static class StackHelper
{

  public static String ReportError(string Message)
  {
    // Get the frame one step up the call tree
    StackFrame CallStack = new StackFrame(1, true);

    // These will now show the file and line number of the ReportError
    string SourceFile = CallStack.GetFileName();
    int SourceLine = CallStack.GetFileLineNumber();

    return "Error: " + Message + "\nFile: " + SourceFile + "\nLine: " + SourceLine.ToString();
  }

  public static int __LINE__
  {
    get
    {
      StackFrame CallStack = new StackFrame(1, true);
      int line = new int();
      line += CallStack.GetFileLineNumber();
      return line;
    }
  }

  public static string __FILE__
  {
    get
    {
      StackFrame CallStack = new StackFrame(1, true);
      string temp = CallStack.GetFileName();
      String file = String.Copy(String.IsNullOrEmpty(temp) ? "" : temp);
      return String.IsNullOrEmpty(file) ? "" : file;
    }
  }
}