using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CodonEncoding
{
    public readonly Dictionary<string, char> CodeToLetter = new Dictionary<string, char>(){
        {"gly", 'g'},
        {"ala", 'a'},
        {"val", 'v'},
        {"leu", 'l'},
        {"ile", 'i'},
        {"met", 'm'},
        {"pro", 'p'},
        {"phe", 'f'},
        {"trp", 'w'},
        {"ser", 's'},
        {"thr", 't'},
        {"asn", 'n'},
        {"gln", 'q'},
        {"tyr", 'y'},
        {"cys", 'c'},
        {"lys", 'k'},
        {"arg", 'r'},
        {"his", 'h'},
        {"asp", 'd'},
        {"glu", 'e'},
        {"end", 'x'}
    };

    public Dictionary<char, string[]> Codons = new Dictionary<char, string[]>();

    public CodonEncoding(string Organism)
    {
        CheckFile:
        if (Organism == "")
        {
            HelperFunctions.WriteError("TempError no target specified");
        }
        if (!File.Exists(GIL.Program.DataPath + $"\\CompilationTargets\\{Organism}.gilEncoding"))
        {
            HelperFunctions.WriteError("TempError Invalid compilation target: no gilEncoding file found at " + 
                GIL.Program.DataPath + $"\\CompilationTargets\\{Organism}.gilEncoding");
        }
        GIL.Program.Target = Organism;

        string encoding = File.ReadAllText(GIL.Program.DataPath + $"\\CompilationTargets\\{Organism}.gilEncoding");
        if (encoding.Substring(0, 4) == "goto")
        {
            Organism = encoding.Split("goto ")[1];
            goto CheckFile;
        }

        //Get encoding file
        string[] lines = encoding.Split('\n');
        foreach (string line in lines)
        {
            string[] sides = line.Split(" : ");
            string[] codons = sides[1].Split(' ');
            Codons.Add(sides[0][0], codons);
        }
    }

    public string GetLetter(char l, int index = 0)
    {
        return Codons[Char.ToLower(l)][index];
    }

    public string GetCode(string s, int idx = 0)
    {
        return GetLetter(CodeToLetter[s.ToLower()], index: idx);
    }
}