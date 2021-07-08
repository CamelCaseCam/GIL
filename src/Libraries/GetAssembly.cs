using System;
using System.IO;

public static class GetAssembly
{
    public static void Get(string name)
    {
        string path = LibFuncs.GetLibPath(name, end:"");
        string LibPath = LibFuncs.GetLibPath(name, end:".dll");
        string[] manifest = File.ReadAllText(path).Replace("\r", "").Split('\n');

        foreach (string op in manifest)
        {
            string[] splitted = name.Split(new char[] {'\\', '/'});
            GetOp.Assemblies.Add(op, (splitted[splitted.Length - 1], LibPath));
            GetOp.Operations.Add(op);
        }
    }
}