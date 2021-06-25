using System;
using System.IO;

public static class LibFuncs
{
    public static string GetLibPath(string path, string end = "gil")
    {
        if (path.Contains(':'))
        {
            return path;
        } else 
        {
            string LibEnd = $"\\{path}.{end}";
            string InDir = Environment.CurrentDirectory + LibEnd;
            string Lib = GIL.Program.DataPath + "\\Libraries" + LibEnd;
            
            if (File.Exists(InDir))
            {
                return InDir;
            } else if (File.Exists(Lib))
            {
                return Lib;
            } else
            {
                HelperFunctions.WriteError($"TempError file {path} not found");
                return "";    //Will not be executed since WriteError exits the program
            }
        }
    }
}