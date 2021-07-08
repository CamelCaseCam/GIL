using System;
using System.Reflection;
using System.Collections.Generic;

public static class GetOp
{
    public static List<string> Operations = new List<string>();

    public static Dictionary<string, Func<List<Token>, Compiler, RelativeFeature>> LoadedOps = 
        new Dictionary<string, Func<List<Token>, Compiler, RelativeFeature>>();

    
    public static Dictionary<string, (string, string)> Assemblies = new Dictionary<string, (string, string)>();

    public static RelativeFeature Call(string name, List<Token> tokens)
    {
        if (LoadedOps.ContainsKey(name))
        {
            return LoadedOps[name](tokens, Compiler.ExecutingCompiler);
        }
        
        (string AssemblyName, string AssemblyPath) = Assemblies[name];
        Assembly assem = Assembly.LoadFrom(AssemblyPath);
        Dictionary<string, Func<List<Token>, Compiler, RelativeFeature>> InAssembly = 
            (Dictionary<string, Func<List<Token>, Compiler, RelativeFeature>>) assem.GetType($"{AssemblyName}.Main").GetField(
                "Operations").GetValue(null);
        
        foreach (KeyValuePair<string, Func<List<Token>, Compiler, RelativeFeature>> op in InAssembly)
        {
            LoadedOps.Add(op.Key, op.Value);
        }
        return LoadedOps[name](tokens, Compiler.ExecutingCompiler);
    }
}