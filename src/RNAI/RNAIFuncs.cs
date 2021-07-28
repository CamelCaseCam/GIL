using System;
using System.Collections;
using System.Collections.Generic;

public static class RNAIFuncs
{
    public static string[] GetOverlap(string[] strings, int len)
    {
        int MinLength = strings[0].Length;
        foreach (string s in strings)
        {
            if (s.Length < MinLength)
            {
                MinLength = s.Length;
            }
        }
        
        List<string> Output = new List<string>();
        for (int i = 0; i < MinLength - len + 1; i++)    //I should rewrite this to use spans
        {
            string TestOverlap = strings[0].Substring(i, len);
            if (TestOverlap.Contains("tac"))
            {
                i += TestOverlap.IndexOf("tac");
                continue;
            }
            if (TestOverlap.Contains("uac"))
            {
                i += TestOverlap.IndexOf("uac");
                continue;
            }
            
            for (int s = 1; s < strings.Length; s++)
            {
                if (!strings[s].Contains(TestOverlap))
                {
                    goto NoMatch;
                }
            }
            Output.Add(TestOverlap);
            
            NoMatch:
            continue;
        }
        return Output.ToArray();
    }

    public static string ExcludeOverlaps(string[] Overlaps, string[] Exclusions)
    {
        foreach (string overlap in Overlaps)
        {
            foreach (string exclusion in Exclusions)
            {
                if (exclusion.Contains(overlap))
                {
                    goto match;
                }
            }
            return overlap;

            match:
            continue;
        }
        return "";
    }
}