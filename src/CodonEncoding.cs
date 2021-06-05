using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CodonEncoding
{
    //Dictionary to convert three letter amino acid abreviation to single letter code
    public static readonly Dictionary<string, char> CodeToLetter = new Dictionary<string, char>(){
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
    
    //Dictionary of codons and their corresponding amino acids
    public static readonly Dictionary<string, char> CodonToLetter = new Dictionary<string, char>() {
        {"gga", 'g'},
        {"ggt", 'g'},
        {"ggg", 'g'},
        {"ggc", 'g'},
        {"gca", 'a'},
        {"gct", 'a'},
        {"gcg", 'a'},
        {"gcc", 'a'},
        {"gta", 'v'},
        {"gtt", 'v'},
        {"gtg", 'v'},
        {"gtc", 'v'},
        {"cta", 'l'},
        {"ctt", 'l'},
        {"ctg", 'l'},
        {"ctc", 'l'},
        {"ttg", 'l'},
        {"tta", 'l'},
        {"att", 'i'},
        {"ata", 'i'},
        {"atc", 'i'},
        {"atg", 'm'},
        {"cca", 'p'},
        {"cct", 'p'},
        {"ccg", 'p'},
        {"ccc", 'p'},
        {"ttt", 'f'},
        {"ttc", 'f'},
        {"tgg", 'w'},
        {"tca", 's'},
        {"tct", 's'},
        {"tcg", 's'},
        {"tcc", 's'},
        {"agt", 's'},
        {"agc", 's'},
        {"aca", 't'},
        {"act", 't'},
        {"acg", 't'},
        {"acc", 't'},
        {"aat", 'n'},
        {"aac", 'n'},
        {"caa", 'q'},
        {"cag", 'q'},
        {"tat", 'y'},
        {"tac", 'y'},
        {"tgt", 'c'},
        {"tgc", 'c'},
        {"aaa", 'k'},
        {"aag", 'k'},
        {"aga", 'r'},
        {"agg", 'r'},
        {"cga", 'r'},
        {"cgt", 'r'},
        {"cgg", 'r'},
        {"cgc", 'r'},
        {"cat", 'h'},
        {"cac", 'h'},
        {"gat", 'd'},
        {"gac", 'd'},
        {"gaa", 'e'},
        {"gag", 'e'},
        {"taa", 'x'},
        {"tga", 'x'},
        {"tag", 'x'},
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
        encoding = encoding.Replace("\r", "");
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

    public (char, int) CodonToCode(string codon)
    {
        char Letter = CodonToLetter[codon];
        string[] PotentialCodes = Codons[Letter];

        for (int i = 0; i < PotentialCodes.Length; i++)
        {
            if (PotentialCodes[i].ToLower() == codon)
            {
                return (Letter, i);
            }
        }
        HelperFunctions.WriteError($"TempError codon {codon} not found");
        return (' ', -1);
    }
}